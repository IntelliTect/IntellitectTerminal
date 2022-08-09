import { Key, Command, unknownArg, TreeNode, validatePath, output, serializeFilesSystemToTree, KeydownEvent } from "./utils";
import { IntelliTerm } from "./intelliterm";
import { CommandProcessor, KeyListener } from "./decorators";

// All On Key events for the IntelliTerm are handled here
// via the KeyListener decorator
export class IntelliTermKeyHelper {

    term: IntelliTerm;

    constructor(intelliTerm: IntelliTerm) {
        this.term = intelliTerm;
        this.cursorPosition = this.term.hostname.length;
    }

    // Integer index the cursor is currently at. This is needed for back spaces.
    cursorPosition: number;

    // The stored string the user is typing
    userInput: string = "";

    // On any key pressed that isnt already mapped
    default(event: KeydownEvent) {
        this.term.write(event.key);
        this.userInput += event.key;
        this.cursorPosition++;
    }

    @KeyListener.on(Key.BACKSPACE)
    backspacePressed(event: KeydownEvent) {
        // If the cursor is going to delete from our path... dont
        if (this.cursorPosition == this.term.hostname.length) { return; }

        // Write backspace
        this.term.write("\b \b");
        this.userInput = this.userInput.substring(0, this.userInput.length - 1);
        this.cursorPosition--;
        return;
    }

    @KeyListener.on(Key.ENTER)
    enterPressed(event: KeydownEvent) {
        this.term.write(Key.ENTER);
        this.userInput += Key.ENTER;

        // Newline
        this.term.write("\n");

        // Do not allow a command that is blank to be ran.
        if (this.userInput.trim() != "") {
            var cmd: string[] = this.userInput.trim().split(" ");
            CommandProcessor.process(cmd[0] as Command, cmd.slice(1, cmd.length), () => output("intelliterm", this.term, `Command not found '${cmd}'`));
        }

        this.term.write(this.term.hostname);
        this.userInput = "";
        this.cursorPosition = this.term.hostname.length;
        return;
    }

    @KeyListener.on(Key.CTRL_L)
    ctrlLPressed(event: KeydownEvent) {
        this.term.clear();
    }
}

// All commands for the IntelliTerm are handled here
// via the CommandProcessor decorator.
export class IntelliTermCommandHelper {
    term: IntelliTerm;
    constructor(term: IntelliTerm) {
        this.term = term;
    }

    // Path we are on
    pathTree: TreeNode = new TreeNode("/", false, null, []);

    // Present working directory
    pwd: string = "/";

    // Update the path we are on and display hostname correctly
    updatePath(location: TreeNode): void {

        // Traverse up the tree until root to get the pwd.
        this.pwd = "";
        let traverse: TreeNode = location;
        while (traverse.Parent != null) {
            this.pwd = ("/" + traverse.Name) + this.pwd;
            traverse = traverse.Parent;
        }

        this.pathTree = location;
        this.term.hostname = `[\x1b[34mintellitect\x1B[0m@localuser \x1b[34m${location.Name}\x1b[0m]$ `;
    }

    @CommandProcessor.on(Command.HELP)
    help() {
        this.term.writeln(" help - Displays this message");
        this.term.writeln(" ls - Lists all files in the current directory");
        this.term.writeln(" cd - Navigate to a directory");
        this.term.writeln(" cat - Displays the contents of a file");
        this.term.writeln(" clear - Clear the terminal");
        this.term.writeln(" pwd - Display present working directory");
        this.term.writeln(" request - Requests a challenge");
        this.term.writeln(" submit - Submits a challenge");
        this.term.writeln(" progress - Displays your progress");
        this.term.writeln(" verify - Verifies a challenge");
    }


    @CommandProcessor.on(Command.LS)
    ls(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.LS, this.term, args[0]);
            return;
        }
        this.pathTree.Children.forEach((child: TreeNode) =>

            // append a ./ to show it is a folder
            this.term.writeln(`${child.isFile ? "" : "./"}${child.Name}`)
        );
    }

    @CommandProcessor.on(Command.CD)
    cd(args: string[]) {
        // Arg[0] is required
        if (args[0] == undefined || args[0] == ".") {
            return;
        }

        // Root level navigation
        if (args[0].startsWith("/")) {

            // Get each direction we need from the arg[0]. EX /home/user/file [home,user,file]
            let directions = args[0].split("/").filter(element => element == "" ? false : true);

            this.updatePath(validatePath(this.term.fileSystemTree, directions, this.term, args[0]));
            return;
        }

        // Directory level navigation. Endpoint must be inside of this.path
        // Get each direction we need from the arg[0]. EX ./user/file [user,file]
        let directions = args[0].split("/").filter(element => element == "" || element == "." ? false : true);

        this.updatePath(validatePath(this.pathTree, directions, this.term, args[0]));
    }

    // TODO: Cat into any dir?
    @CommandProcessor.on(Command.CAT)
    async cat(args: string[]) {
        // Arg[0] is required
        if (args[0] == undefined) {
            output(Command.CAT, this.term, "Missing file argument path");
            return;
        }

        // Find the file
        let file: TreeNode | null = null;
        this.pathTree.Children.forEach((child) => file = (child.Name == args[0]) ? child : null);
        if (file == null) {
            output(Command.CAT, this.term, `File not found '${args[0]}'`)
            return;
        }
        if (!(file as TreeNode).isFile) {
            output(Command.CAT, this.term, "Argument is a directory and not a file");
        }
        await this.term.api.command.cat(this.term.user?.userId!, (file as TreeNode)!.Name);
        let catouput = this.term.api.command.cat.result;
        this.term.writeln("");
        this.term.writeln(`${catouput}`);
    }

    @CommandProcessor.on(Command.CLEAR)
    clear(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.HELP, this.term, args[0]);
            return;
        }
        this.term.clear();
    }

    @CommandProcessor.on(Command.PWD)
    pwdCommand(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.PWD, this.term, args[0]);
            return;
        }
        this.term.writeln(this.pwd);
    }

    @CommandProcessor.on(Command.REQUEST)
    async request(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.REQUEST, this.term, args[0]);
            return;
        }
        this.term.writeln("intelliterm: mounting a challenge in /home/localuser/challenges");

        // Grab the result from the server
        await this.term.api.command.request(this.term.user?.userId!);

        // cache new file system
        let updatedFileSystem = this.term.api.command.request.result;
        if (updatedFileSystem != null) {

            // Serialize the file system
            this.term.fileSystemTree = serializeFilesSystemToTree(JSON.parse(updatedFileSystem));

            // WARNING: Kinda weird.
            // Use PWD to navigate to the folder that we were on when the filesystem is updated.
            let tempPwd = this.pwd;
            this.updatePath(this.term.fileSystemTree);
            this.updatePath(
                validatePath(
                    this.term.fileSystemTree,
                    tempPwd.split("/")
                        .filter(element => element == "" ? false : true),
                    this.term, tempPwd)
            );
            return;
        }
    }

    @CommandProcessor.on(Command.SUBMIT)
    submit(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.SUBMIT, this.term, args[0]);
            return;
        }

        // Click the filesharing input element
        (document.getElementById("file") as HTMLInputElement).click();
    }

    @CommandProcessor.on(Command.PROGRESS)
    async progress(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.PROGRESS, this.term, args[0]);
            return;
        }
        await this.term.api.command.progress(this.term.user?.userId!);
        let progressoutput = this.term.api.command.progress.result;
        this.term.writeln(`${progressoutput}`);
    }

    @CommandProcessor.on(Command.VERIFY)
    async verify(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.VERIFY, this.term, args[0]);
            return;
        }
        await this.term.api.command.verify(this.term.user!.userId!);
        if (this.term.api.command.verify.result) {
            output("intelliterm", this.term, "Successful output! Run request to get another challenge");
            this.term.write(this.term.hostname);
            return;
        }
        output("intelliterm", this.term, "Incorrect output. Submit another file ");
        this.term.write(this.term.hostname);
    }
}
