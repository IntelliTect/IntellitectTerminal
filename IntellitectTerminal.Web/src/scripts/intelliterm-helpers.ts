import { Terminal } from "xterm";
import { Key } from "./utils";
import { IntelliTerm, KeyListener } from "./intelliterm";

class IntelliTermKeyHelper {

    intelliTerm: IntelliTerm;

    constructor(intelliTerm: IntelliTerm) {
        this.intelliTerm = intelliTerm;
    }

    @KeyListener.on(Key.BACKSPACE)
    backspacePressed() {
        return ""
    }
}

keyPressedHandler(event: { key: any; domEvent?: KeyboardEvent; }) {

    // Command keys
    switch (event.key) {

        case Keys.BACKSPACE:
            // If the cursor is going to delete from our path... dont
            if (this.cursorPosition == this.hostname.length) { return; }
            this.term.write("\b \b");
            this.userInput = this.userInput.substring(0, this.userInput.length - 1);
            this.cursorPosition--;
            return;

        case Keys.ENTER:
            this.term.write(event.key);
            this.userInput += event.key;
            this.term.write("\n");

            // Do not allow a command that is blank to be ran.
            if (this.userInput.trim() != "") {
                var cmd: string[] = this.userInput.trim().split(" ");
                this.commandHandler(cmd[0], cmd.slice(1, cmd.length));
            }

            this.term.write(this.hostname);
            this.userInput = "";
            this.cursorPosition = this.hostname.length;
            return;

        // Breaks functionality that xterm already gives arrows
        case Keys.ARROW_LEFT:
            break;

        // Breaks functionality that xterm already gives arrows
        case Keys.ARROW_RIGHT:
            break;

        // Breaks functionality that xterm already gives arrows
        case Keys.ARROW_UP:
            break;

        // Breaks functionality that xterm already gives arrows
        case Keys.ARROW_BOTTOM:
            break;

        case Keys.CTRL_L:
            this.term.clear();
            break;

        default:
            console.log(event);
            this.term.write(event.key);
            this.userInput += event.key;
            this.cursorPosition++;
    }
}

  async commandHandler(cmd: string, arg: string[]) {

    switch (cmd.toLocaleLowerCase()) {
        case Commands.HELP:
            if (arg.length > 0) {
                unknownArg(Commands.HELP, this.term, arg[0]);
                break;
            }

            this.term.writeln(" help - Displays this message");
            this.term.writeln(" ls - Lists all files in the current directory");
            this.term.writeln(" cat - Displays the contents of a file");
            this.term.writeln(" cd - Navigate to a directory");
            this.term.writeln(" clear - Clear the terminal");
            this.term.writeln(" pwd - Display present working directory");
            this.term.writeln(" challenge - Requests a challenge");
            this.term.writeln(" submit - Submits a challenge");
            this.term.writeln(" edit - Edits a file");
            this.term.writeln(" progress - Displays your progress");
            this.term.writeln(" verify - Verifies a challenge");
            break;

        case Commands.REQUEST:
            if (arg.length > 0) {
                unknownArg(Commands.REQUEST, this.term, arg[0]);
                break;
            }
            this.term.writeln("intelliterm: mounting a challenge in /home/localuser/challenges");

            // Grab the result from the server
            await this.commandservice.request(this.user?.userId!);

            // cache new file system
            let updatedFileSystem = this.commandservice.request.result;
            if (updatedFileSystem != null) {

                // Serialize the file system
                this.fileSystemTree = serializeFilesSystemToTree(JSON.parse(updatedFileSystem));

                // WARNING: Kinda weird.
                // Use PWD to navigate to the folder that we were on when the filesystem is updated.
                let tempPwd = this.pwd;
                this.updatePath(this.fileSystemTree);
                this.updatePath(
                    validatePath(
                        this.fileSystemTree,
                        tempPwd.split("/")
                            .filter(element => element == "" ? false : true),
                        this.term, tempPwd)
                );
            }
            else {
                console.log('user id is unexpectedly null');
            }
            break;

        // TODO: ls into a direct path ex: cat /home/user/readme.txt
        case Commands.LS:
            if (arg.length > 0) {
                unknownArg(Commands.LS, this.term, arg[0]);
                break;
            }
            this.pathTree.Children.forEach((child: TreeNode) =>

                // append a ./ to show it is a folder
                this.term.writeln(`${child.isFile ? "" : "./"}${child.Name}`)
            );
            break;

        case Commands.CD:

            // Arg[0] is required
            if (arg[0] == undefined || arg[0] == ".") {
                break;
            }

            // Root level navigation
            if (arg[0].startsWith("/")) {

                // Get each direction we need from the arg[0]. EX /home/user/file [home,user,file]
                let directions = arg[0].split("/").filter(element => element == "" ? false : true);

                this.updatePath(validatePath(this.fileSystemTree, directions, this.term, arg[0]));
                break;
            }

            // Directory level navigation. Endpoint must be inside of this.path
            // Get each direction we need from the arg[0]. EX ./user/file [user,file]
            let directions = arg[0].split("/").filter(element => element == "" || element == "." ? false : true);

            this.updatePath(validatePath(this.pathTree, directions, this.term, arg[0]));
            break;

        // TODO: cat into a direct path ex: cat /home/user/readme.txt
        case Commands.CAT:
            // Arg[0] is required
            if (arg[0] == undefined) {
                output(Commands.CAT, this.term, "Missing file argument path");
                break;
            }

            // Find the file
            let file: TreeNode | null = null;
            this.pathTree.Children.forEach((child) => file = (child.Name == arg[0]) ? child : null);
            if (file == null) {
                output(Commands.CAT, this.term, `File not found '${arg[0]}'`)
                break;
            }
            if (!(file as TreeNode).isFile) {
                output(Commands.CAT, this.term, "Argument is a directory and not a file");
            }
            await this.commandservice.cat(this.user?.userId!, (file as TreeNode).Name);
            let catouput = this.commandservice.cat.result;
            this.term.writeln("");
            this.term.writeln(`${catouput}`);
            break;

        case Commands.CLEAR:
            if (arg.length > 0) {
                unknownArg(Commands.HELP, this.term, arg[0]);
                break;
            }
            this.term.clear();
            break;

        case Commands.PROGRESS:
            if (arg.length > 0) {
                unknownArg(Commands.HELP, this.term, arg[0]);
                break;
            }
            await this.commandservice.progress(this.user?.userId!);
            let progressoutput = this.commandservice.progress.result;
            this.term.writeln(`${progressoutput}`);
            break;

        case Commands.PWD:
            if (arg.length > 0) {
                unknownArg(Commands.HELP, this.term, arg[0]);
                break;
            }
            this.term.writeln(this.pwd);
            break;

        case Commands.SUBMIT:

            if (arg.length > 0) {
                unknownArg(Commands.HELP, this.term, arg[0]);
                break;
            }

            // Click the input element
            this.doc?.click();
            break;

        case Commands.VERIFY:
            if (arg.length > 0) {
                unknownArg(Commands.HELP, this.term, arg[0]);
                break;
            }
            await this.commandservice.verify(this.user!.userId!);
            if (this.commandservice.verify.result) {
                output("intelliterm", this.term, "Successful output! Run request to get another challenge");
                this.term.write(this.hostname);
                break;
            }
            output("intelliterm", this.term, "Incorrect output. Submit another file ");
            this.term.write(this.hostname);
            break;

        default:
            output("intelliterm", this.term, `Command not found '${cmd}'`)
    }
}
