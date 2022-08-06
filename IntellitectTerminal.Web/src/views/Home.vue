<template>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Terminal } from "xterm";
import { CommandServiceViewModel, UserServiceViewModel } from '@/viewmodels.g';
import { User } from "../models.g";
import * as Cookies from 'tiny-cookie';
import { isCookieEnabled, getCookie, setCookie, removeCookie } from 'tiny-cookie'

class TreeNode {
  Name: string;
  isFile: boolean;
  Parent: TreeNode | null;
  Children: TreeNode[];
  constructor(name: string, isFile: boolean, parent: TreeNode | null, children: TreeNode[]) {
    this.Name = name;
    this.isFile = isFile;
    this.Parent = parent;
    this.Children = children;
  }
}

interface FileNode {
  Parent: string;
  Name: string;
  isFile: boolean;
  Children: FileNode[];
}

function serializeFilesSystemToTree(node: FileNode | TreeNode) {
  let parent = new TreeNode(node.Name, node.isFile, null, []);
  node.Children.forEach((child) => {
    let childNode: TreeNode = serializeFilesSystemToTree(child);
    childNode.Parent = parent;
    parent.Children.push(childNode);
  });
  return parent;
}

enum Keys {
  BACKSPACE = "\x7F",
  ENTER = "\r",
  ARROW_LEFT = "\x1B[D",
  ARROW_RIGHT = "\x1B[C",
  ARROW_UP = "\x1B[A",
  ARROW_BOTTOM = "\x1B[B",
  CTRL_L = "\f"
}

enum Commands {
  HELP = "help",
  REQUEST = "request",
  LS = "ls",
  CD = "cd",
  CAT = "cat",
  CLEAR = "clear",
  PROGRESS = "progress",
  PWD = "pwd",
  SUBMIT = "submit",
  VERIFY = "verify"
}

@Component
// WARNING: Despite being a vue project we are just using typescript :)
export default class Home extends Vue {

  // Connects to the API
  commandservice = new CommandServiceViewModel();
  userservice = new UserServiceViewModel();
  user: User | null = null;
  doc: HTMLInputElement | null = null;

  // The stored string the user is typing
  userInput: string = "";

  // Entire file system
  fileSystemTree: TreeNode = new TreeNode("/", false, null, []);

  // Path we are on
  pathTree: TreeNode = new TreeNode("/", false, null, []);

  // Hostname relative to path we are on
  hostname = `[\x1b[34mintellitect\x1B[0m@localuser \x1b[34m${this.pathTree.Name}\x1b[0m]$ `;

  // Present working directory
  pwd: string = "/";

  // Update the path we are on and display hostname correctly
  updatePath(location: TreeNode) {

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

  // Position the cursor is currently at. This is needed for back spaces.
  cursorPosition = this.hostname.length;
  term = new Terminal({
    cursorBlink: true,
    fontSize: 30,
    cols: 200,
    fontFamily: "monospace",
  });

  history = [];
  welcomeMessage = `\n\nWelcome to the \x1b[34mIntelliTect Terminal\x1b[0m! View commands by typing help.\n\n`;

  async created() {

    // XTerms input
    const input = document.getElementById('terminal');

    // Get the file input element that is hidden in index.html
    this.doc = document.getElementById("file") as HTMLInputElement;

    // On doc input
    this.doc?.addEventListener("change", (files) => {

      // Create the http request
      // TODO: Bug with submitting the same file after request
      let formData = new FormData();
      formData.append("file", this.doc!.files![0]);
      formData.append("userId", this.user!.userId!);
      fetch("/api/CommandService/SubmitFile", {
        method: "POST",
        body: formData
      }).then((response) => response.json()).then((result) => {
        console.log('Success:', result);
        output("submit", this.term, "File submitted. Use \x1b[31mverify\x1b[0m to confirm the submission.")
        this.term.write(this.hostname);
      }).catch((error) => {
        console.error('Error:', error);
        output("submit", this.term, "An error occured uploading the file.");
        this.term.write(this.hostname);
      });
    })
    if (input != null) {
       // if user is in cookies, get user id from cookies
      let userId = getCookie('userId');
      let user;
      if (userId == null) {
        // if user is not in cookies, get user id from api
        await this.userservice.initializeFileSystem(null);
        user = this.userservice.initializeFileSystem.result;
        userId = user!.userId;
        if(userId != null) {
          setCookie('userId', userId, { expires: 100 });
        } 
      } else {
        // if user is in cookies, get user from api
        // Request for the file system
        await this.userservice.initializeFileSystem(userId);
        user = this.userservice.initializeFileSystem.result;
      }
      // Request for the file system
      //await this.userservice.initializeFileSystem(null);

      // Get user
     // let user = this.userservice.initializeFileSystem.result;
      console.log(user);
      this.user = user;
      // Serialize and cache filesystem to a Tree
      console.log(JSON.parse(this.user?.fileSystem!));
      this.fileSystemTree = serializeFilesSystemToTree(JSON.parse(this.user?.fileSystem!));
      this.pathTree = this.fileSystemTree;

      if (user != null) {
        this.initTerminal(input)
      }
    }
  }

  initTerminal(input: HTMLElement) {
    this.term.open(input);

    // Display welcome message
    this.term.write(this.welcomeMessage);
    this.term.write("\r\n");

    // Log path you are on
    this.term.write(this.hostname);

    // Main key handler. Anything pressed goes here.
    this.term.onKey((e) => this.keyPressedHandler(e));
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

}

function output(prefix: string, term: Terminal, msg: string) {
  term.writeln(`\r\n\x1b[34m${prefix}\x1b[0m: ${msg}`);
}

function unknownArg(prefix: string, term: Terminal, unArg: string) {
  output(prefix, term, `Unknown argument '${unArg}'`);
}

function validatePath(start: TreeNode, directions: string[], term: Terminal, arg: string): TreeNode {
  // Start on the dir we are on
  let traverse: TreeNode = start;
  directions.every((direction: string) => {

    // Navigate up one on ..
    if (direction == "..") {
      traverse = traverse.Parent!;
      return true;
    }

    // Find the node that matches the path
    let i = traverse.Children.find((child) => child.Name == direction);


    // If no node is found, the path is errornous. Return.
    if (i == undefined) {
      output(Commands.CD, term, `Directory not found '${arg}'`);
      return;
    }
    if (i.isFile) {
      output(Commands.CD, term, "Argument is a file and not a directory");
      return;
    }
    traverse = i;
    return true;
  })
  return traverse;
}

</script>
