import { Command, unknownArg, TreeNode, validatePath, output, serializeFilesSystemToTree } from "./utils";
import { IntelliTerm } from "./intelliterm";
import { CommandProcessor } from "./decorators";

// All commands for the IntelliTerm are handled here
// via the CommandProcessor decorator. Singleton static class since decorators can't access instance variables.
export class ITCHelper {

    // Path we are on
    static pathTree: TreeNode = new TreeNode("/", false, null, []);

    // Present working directory
    static pwd: string = "/";

    static term: IntelliTerm

    static init(term: IntelliTerm): ITCHelper {
        ITCHelper.term = term;
        return new ITCHelper();
    }

    // Update the path we are on and display hostname correctly
    static updatePath(location: TreeNode): void {

        // Traverse up the tree until root to get the pwd.
        ITCHelper.pwd = "";
        let traverse: TreeNode = location;
        while (traverse.Parent != null) {
            ITCHelper.pwd = ("/" + traverse.Name) + ITCHelper.pwd;
            traverse = traverse.Parent;
        }

        ITCHelper.pathTree = location;
        ITCHelper.term.hostname = `[\x1b[34mintellitect\x1B[0m@localuser \x1b[34m${location.Name}\x1b[0m]$ `;
    }

    @CommandProcessor.on(Command.HELP)
    help() {
        ITCHelper.term.writeln(" help - Displays ITCHelper message");
        ITCHelper.term.writeln(" ls - Lists all files in the current directory");
        ITCHelper.term.writeln(" cd - Navigate to a directory");
        ITCHelper.term.writeln(" cat - Displays the contents of a file");
        ITCHelper.term.writeln(" clear - Clear the terminal");
        ITCHelper.term.writeln(" pwd - Display present working directory");
        ITCHelper.term.writeln(" request - Requests a challenge");
        ITCHelper.term.writeln(" submit - Submits a challenge");
        ITCHelper.term.writeln(" progress - Displays your progress");
        ITCHelper.term.writeln(" verify - Verifies a challenge");
    }

    @CommandProcessor.on(Command.LS)
    ls(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.LS, ITCHelper.term, args[0]);
            return;
        }
        ITCHelper.pathTree.Children.forEach((child: TreeNode) =>

            // append a ./ to show it is a folder
            ITCHelper.term.writeln(`${child.isFile ? "" : "./"}${child.Name}`)
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

            ITCHelper.updatePath(validatePath(ITCHelper.term.fileSystemTree, directions, ITCHelper.term, args[0]));
            return;
        }

        // Directory level navigation. Endpoint must be inside of ITCHelper.path
        // Get each direction we need from the arg[0]. EX ./user/file [user,file]
        let directions = args[0].split("/").filter(element => element == "" || element == "." ? false : true);

        ITCHelper.updatePath(validatePath(ITCHelper.pathTree, directions, ITCHelper.term, args[0]));
    }

    // TODO: Cat into any dir?
    @CommandProcessor.on(Command.CAT)
    async cat(args: string[]) {
        // Arg[0] is required
        if (args[0] == undefined) {
            output(Command.CAT, ITCHelper.term, "Missing file argument path");
            return;
        }

        // Find the file
        let file: TreeNode | null = null;
        ITCHelper.pathTree.Children.forEach((child) => file = (child.Name == args[0]) ? child : null);
        if (file == null) {
            output(Command.CAT, ITCHelper.term, `File not found '${args[0]}'`)
            return;
        }
        if (!(file as TreeNode).isFile) {
            output(Command.CAT, ITCHelper.term, "Argument is a directory and not a file");
        }
        await ITCHelper.term.api.command.cat(ITCHelper.term.user?.userId!, (file as TreeNode)!.Name);
        let catouput = ITCHelper.term.api.command.cat.result;
        ITCHelper.term.writeln("");
        ITCHelper.term.writeln(`${catouput}`);
    }

    @CommandProcessor.on(Command.CLEAR)
    clear(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.HELP, ITCHelper.term, args[0]);
            return;
        }
        ITCHelper.term.clear();
    }

    @CommandProcessor.on(Command.PWD)
    pwdCommand(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.PWD, ITCHelper.term, args[0]);
            return;
        }
        ITCHelper.term.writeln(ITCHelper.pwd);
    }

    @CommandProcessor.on(Command.REQUEST)
    async request(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.REQUEST, ITCHelper.term, args[0]);
            return;
        }
        ITCHelper.term.writeln("intelliterm: mounting a challenge in /home/localuser/challenges");

        // Grab the result from the server
        await ITCHelper.term.api.command.request(ITCHelper.term.user?.userId!);

        // cache new file system
        let updatedFileSystem = ITCHelper.term.api.command.request.result;
        if (updatedFileSystem != null) {

            // Serialize the file system
            ITCHelper.term.fileSystemTree = serializeFilesSystemToTree(JSON.parse(updatedFileSystem));

            // WARNING: Kinda weird.
            // Use PWD to navigate to the folder that we were on when the filesystem is updated.
            let tempPwd = ITCHelper.pwd;
            ITCHelper.updatePath(ITCHelper.term.fileSystemTree);
            ITCHelper.updatePath(
                validatePath(
                    ITCHelper.term.fileSystemTree,
                    tempPwd.split("/")
                        .filter(element => element == "" ? false : true),
                    ITCHelper.term, tempPwd)
            );
            return;
        }
    }

    @CommandProcessor.on(Command.SUBMIT)
    submit(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.SUBMIT, ITCHelper.term, args[0]);
            return;
        }

        // Click the filesharing input element
        (document.getElementById("file") as HTMLInputElement).click();
    }

    @CommandProcessor.on(Command.PROGRESS)
    async progress(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.PROGRESS, ITCHelper.term, args[0]);
            return;
        }
        await ITCHelper.term.api.command.progress(ITCHelper.term.user?.userId!);
        let progressoutput = ITCHelper.term.api.command.progress.result;
        ITCHelper.term.writeln(`${progressoutput}`);
    }

    @CommandProcessor.on(Command.VERIFY)
    async verify(args: string[]) {
        if (args.length > 0) {
            unknownArg(Command.VERIFY, ITCHelper.term, args[0]);
            return;
        }
        await ITCHelper.term.api.command.verify(ITCHelper.term.user!.userId!);
        if (ITCHelper.term.api.command.verify.result) {
            output("intelliterm", ITCHelper.term, "Successful output! Run request to get another challenge");
            ITCHelper.term.write(ITCHelper.term.hostname);
            return;
        }
        output("intelliterm", ITCHelper.term, "Incorrect output. Submit another file ");
        ITCHelper.term.write(ITCHelper.term.hostname);
    }
}
