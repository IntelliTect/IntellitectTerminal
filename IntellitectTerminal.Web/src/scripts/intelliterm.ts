import { User } from "@/models.g";
import { Terminal } from "xterm";
import { IntelliTermKeyHelper } from "./intelliterm-helpers";
import { API, Key, serializeFilesSystemToTree, TreeNode, Command } from "./utils";

export class IntelliTerm {

    constructor(api: API) {
        this.api = api;
        this.user = api.user.initializeFileSystem.result;
    }

    // Connects to the API
    api: API;
    user: User | null;

    // Entire file system
    fileSystemTree: TreeNode = new TreeNode("/", false, null, []);

    // Path we are on
    pathTree: TreeNode = new TreeNode("/", false, null, []);

    // Present working directory
    pwd: string = "/";

    // Hostname relative to path we are on
    hostname = `[\x1b[34mintellitect\x1B[0m@localuser \x1b[34m${this.pathTree.Name}\x1b[0m]$ `;

    // Style settings
    term = new Terminal({
        cursorBlink: true,
        fontSize: 30,
        cols: 200,
        fontFamily: "monospace",
    });

    // Initialize the terminal into HTML, process filesystem, inject listeners
    init(input: HTMLElement, fileUploader: HTMLInputElement): void {

        // Initialize the file system. The fileSystemTree is the fresh tree from the server,
        // and the pathTree is the one that we are actively navigating through
        this.fileSystemTree = serializeFilesSystemToTree(JSON.parse(this.user?.fileSystem!));
        this.pathTree = this.fileSystemTree;

        this.initDoc(fileUploader)

        // Terminal hooks into this element
        this.term.open(input);

        this.motd();

        // Key pressed handler
        KeyListener.engage(this.term, new IntelliTermKeyHelper(this));
    }

    // TODO: Bug with submitting the same file after request
    private initDoc(fileUploader: HTMLInputElement): void {
        fileUploader.addEventListener("change", (files) => {
            // Create the http request
            let formData = new FormData();
            formData.append("file", fileUploader!.files![0]);
            formData.append("userId", this.user!.userId!);
            fetch("/api/CommandService/SubmitFile", {
                method: "POST",
                body: formData
            }).then((response) => response.json()).then((result) => {
                console.log('Success:', result);
                // output("submit", this.term, "File submitted. Use \x1b[31mverify\x1b[0m to confirm the submission.")
                this.term.write(this.hostname);
            }).catch((error) => {
                console.error('Error:', error);
                // output("submit", this.term, "An error occured uploading the file.");
                this.term.write(this.hostname);
            });
        })
    }

    // Display welcome message
    private motd(): void {
        this.term.write(`\n\nWelcome to the \x1b[34mIntelliTect Terminal\x1b[0m! View commands by typing help.\n\n`);
        this.term.write("\r\n");

        // Log path you are on
        this.term.write(this.hostname);
    }

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
        this.hostname = `[\x1b[34mintellitect\x1B[0m@localuser \x1b[34m${location.Name}\x1b[0m]$ `;
    }
}

// IntelliTerm Key Listener
// Makes decorators for easy 
export abstract class KeyListener {

    // All of the key listener functions
    private static keyListeners: { key: Key, action: Function }[] = [];

    // Bind a key to a function call
    static on = (key: Key) => (target: Object): any =>
        KeyListener.keyListeners.push({
            key: key,
            action: target as Function
        });

    /// Initiate the key listeners onto the terminal instance
    static engage(term: Terminal, keyListener: IntelliTermKeyHelper) {
        term.onKey((e) => {

            // Manual switch case
            let actionCalled: boolean = KeyListener.keyListeners.every((keyListener) => {

                let k = keyListener.key;

                // Ignore arrow keys (for now)
                if (k == Key.ARROW_BOTTOM || k == Key.ARROW_UP || k == Key.ARROW_LEFT || k == Key.ARROW_RIGHT) {
                    return true;
                }

                if (keyListener.key == e.key) {
                    keyListener.action();

                    // Break
                    return true;
                }
                return false;
            })

            if (!actionCalled) {
                keyListener.default(e);
                return;
            }
        })
    }
}

export abstract class CommandProcessor {

    // All of the commands
    private static commands: Map<Command, Function> = new Map();

    // Bind a command to a function call
    static on = (command: Command) => (target: Object): any =>
        CommandProcessor.commands.set(command, target as Function);

    // Map to the command to its action
    static process(command: Command): void {
        let action = CommandProcessor.commands.get(command);
        if (action != undefined) {
            action();
        }
    }
}