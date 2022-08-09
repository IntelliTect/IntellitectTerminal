import { User } from "@/models.g";
import { Terminal } from "xterm";
import { KeyListener } from "./decorators";
import { IntelliTermCommandHelper, IntelliTermKeyHelper } from "./intelliterm-helpers";
import { API, serializeFilesSystemToTree, TreeNode } from "./utils";

export class IntelliTerm extends Terminal {

    constructor(api: API) {
        super({
            cursorBlink: true,
            fontSize: 30,
            cols: 200,
            fontFamily: "monospace",
        })
        this.api = api;
        this.user = api.user.initializeFileSystem.result;
    }

    // Connects to the API
    api: API;
    user: User | null;

    // Entire file system
    fileSystemTree: TreeNode = new TreeNode("/", false, null, []);

    // Hostname relative to path we are on
    hostname = `[\x1b[34mintellitect\x1B[0m@localuser \x1b[34m${this.fileSystemTree.Name}\x1b[0m]$ `;

    // Initialize the terminal into HTML, process filesystem, inject listeners
    init(input: HTMLElement, fileUploader: HTMLInputElement): void {

        // Initialize the file system. The fileSystemTree is the fresh tree from the server,
        // and the pathTree is the one that we are actively navigating through
        this.fileSystemTree = serializeFilesSystemToTree(JSON.parse(this.user?.fileSystem!));

        this.initDoc(fileUploader)

        // Terminal hooks into this element
        this.open(input);

        this.motd();

        // Key pressed handler
        new IntelliTermCommandHelper(this);
        KeyListener.engage(this, new IntelliTermKeyHelper(this));
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
                // output("submit", this.term, "File submitted. Use \x1b[31mverify\x1b[0m to confirm the submission.")
                this.write(this.hostname);
            }).catch((error) => {
                // output("submit", this.term, "An error occured uploading the file.");
                this.write(this.hostname);
            });
        })
    }

    // Display welcome message
    private motd(): void {
        this.write(`\n\nWelcome to the \x1b[34mIntelliTect Terminal\x1b[0m! View commands by typing help.\n\n`);
        this.write("\r\n");

        // Log path you are on
        this.write(this.hostname);
    }
}