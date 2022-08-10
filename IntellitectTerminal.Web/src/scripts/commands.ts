import { Command, unknownArg, TreeNode, validatePath, output, serializeFilesSystemToTree } from "./utils";
import { IntelliTerm } from "./intelliterm";
import { CommandProcessor } from "./decorators";

/**
 * All commands for the IntelliTerm are handled here via the CommandProcessor decorator. 
 */
export class Commands {

    // Path we are on
    static pathTree: TreeNode = TreeNode.fromRootNode();

    // Present working directory
    static pwd: string = "/";

    static term: IntelliTerm

    /** Initializes all of the values of @class Commands */
    static init(term: IntelliTerm) {
        Commands.term = term;
    }

    // Update the path we are on and display hostname correctly
    static updatePath(location: TreeNode): void {

        // If the path is the same, don't change everything.
        if (location == this.pathTree) {
            return;
        }

        // Traverse up the tree until root to get the pwd output.
        Commands.pwd = "";
        let traverse: TreeNode = location;
        while (traverse.Parent != null) {
            Commands.pwd = ("/" + traverse.Name) + Commands.pwd;
            traverse = traverse.Parent;
        }

        Commands.pathTree = location;
        Commands.term.hostname = `[\x1b[34mintellitect\x1B[0m@localuser \x1b[34m${location.Name}\x1b[0m]$ `;
    }

    @CommandProcessor.on(Command.HELP)
    help() {
        Commands.term.writeln(" help - Displays ITCHelper message");
        Commands.term.writeln(" ls - Lists all files in the current directory");
        Commands.term.writeln(" cd - Navigate to a directory");
        Commands.term.writeln(" cat - Displays the contents of a file");
        Commands.term.writeln(" clear - Clear the terminal");
        Commands.term.writeln(" pwd - Display present working directory");
        Commands.term.writeln(" request - Requests a challenge");
        Commands.term.writeln(" submit - Submits a challenge");
        Commands.term.writeln(" progress - Displays your progress");
        Commands.term.writeln(" verify - Verifies a challenge");
    }

    @CommandProcessor.on(Command.LS)
    ls(args: string[]) {
        if (args.length > 0) {
            unknownArg({
                prefix: Command.LS,
                term: Commands.term,
                unArg: args[0]
            });
            return;
        }
        Commands.pathTree.Children.forEach((child: TreeNode) =>

            // append a ./ to show it is a folder
            Commands.term.writeln(`${child.isFile ? "" : "./"}${child.Name}`)
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

            Commands.updatePath(validatePath({
                start: Commands.term.fileSystemTree,
                directions: directions,
                term: Commands.term,
                arg: args[0]
            }));
            return;
        }

        // Directory level navigation. Endpoint must be inside of ITCHelper.path
        // Get each direction we need from the arg[0]. EX ./user/file [user,file]
        let directions = args[0].split("/").filter(element => element == "" || element == "." ? false : true);

        Commands.updatePath(validatePath({
            start: Commands.pathTree,
            directions: directions,
            term: Commands.term,
            arg: args[0]
        }));
    }

    // TODO: Cat into any dir?
    @CommandProcessor.on(Command.CAT)
    async cat(args: string[]) {
        // Arg[0] is required
        if (args[0] == undefined) {
            output({
                prefix: Command.CAT,
                term: Commands.term,
                msg: "Missing file argument path"
            });
            return;
        }

        // Find the file
        let file: TreeNode | null = null;
        Commands.pathTree.Children.forEach((child) => file = (child.Name == args[0]) ? child : null);
        if (file == null) {
            output({
                prefix: Command.CAT,
                term: Commands.term,
                msg: `File not found '${args[0]}'`
            })
            return;
        }
        if (!(file as TreeNode).isFile) {
            output({
                prefix: Command.CAT,
                term: Commands.term,
                msg: "Argument is a directory and not a file"
            });
        }
        await Commands.term.api.command.cat(Commands.term.user?.userId!, (file as TreeNode)!.Name);
        let catouput = Commands.term.api.command.cat.result;
        Commands.term.writeln(`${catouput}`);
    }

    @CommandProcessor.on(Command.CLEAR)
    clear(args: string[]) {
        if (args.length > 0) {
            unknownArg({
                prefix: Command.CLEAR,
                term: Commands.term,
                unArg: args[0]
            });
            return;
        }
        Commands.term.clear();
    }

    @CommandProcessor.on(Command.PWD)
    pwdCommand(args: string[]) {
        if (args.length > 0) {
            unknownArg({
                prefix: Command.PWD,
                term: Commands.term,
                unArg: args[0]
            });
            return;
        }

        Commands.term.writeln(Commands.pwd);
    }

    @CommandProcessor.on(Command.REQUEST)
    async request(args: string[]) {
        if (args.length > 0) {
            unknownArg({
                prefix: Command.PWD,
                term: Commands.term,
                unArg: args[0]
            });
            return;
        }

        Commands.term.writeln("intelliterm: mounting a challenge in /home/localuser/challenges");

        // Grab the result from the server
        await Commands.term.api.command.request(Commands.term.user?.userId!);

        // cache new file system
        let updatedFileSystem = Commands.term.api.command.request.result;
        if (updatedFileSystem != null) {

            // Serialize the file system
            Commands.term.fileSystemTree = serializeFilesSystemToTree(JSON.parse(updatedFileSystem));

            // WARNING: Kinda weird.
            // Use PWD to navigate to the folder that we were on when the filesystem is updated.
            let tempPwd = Commands.pwd;
            Commands.updatePath(Commands.term.fileSystemTree);
            Commands.updatePath(
                validatePath({
                    start: Commands.term.fileSystemTree,
                    directions: tempPwd.split("/").filter(element => element == "" ? false : true),
                    term: Commands.term,
                    arg: tempPwd
                })
            );
            return;
        }
    }

    @CommandProcessor.on(Command.SUBMIT)
    submit(args: string[]) {
        if (args.length > 0) {
            unknownArg({
                prefix: Command.SUBMIT,
                term: Commands.term,
                unArg: args[0]
            });
            return;
        }

        // Click the filesharing input element
        (document.getElementById("file") as HTMLInputElement).click();
    }

    @CommandProcessor.on(Command.PROGRESS)
    async progress(args: string[]) {
        if (args.length > 0) {
            unknownArg({
                prefix: Command.PROGRESS,
                term: Commands.term,
                unArg: args[0]
            });
            return;
        }
        await Commands.term.api.command.progress(Commands.term.user?.userId!);
        let progressoutput = Commands.term.api.command.progress.result;
        Commands.term.writeln(`${progressoutput}`);
    }

    @CommandProcessor.on(Command.VERIFY)
    async verify(args: string[]) {
        if (args.length > 0) {
            unknownArg({
                prefix: Command.VERIFY,
                term: Commands.term,
                unArg: args[0]
            });
            return;
        }
        await Commands.term.api.command.verify(Commands.term.user!.userId!);
        if (Commands.term.api.command.verify.result) {
            output({
                prefix: "intelliterm",
                term: Commands.term,
                msg: "Successful output! Run request to get another challenge"
            });
            Commands.term.write(Commands.term.hostname);
            return;
        }
        output({
            prefix: "intelliterm",
            term: Commands.term,
            msg: "Invalid output. Submit another file."
        });
        Commands.term.write(Commands.term.hostname);
    }
}
