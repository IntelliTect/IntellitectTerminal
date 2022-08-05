<template>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Terminal } from "xterm";
import { CommandServiceViewModel, UserServiceViewModel } from '@/viewmodels.g';

class TreeNode {
  Value: string;
  isFile: boolean;
  Parent: TreeNode | null;
  Children: TreeNode[];
  constructor(name: string, isFile: boolean, parent: TreeNode | null, children: TreeNode[]) {
    this.Value = name;
    this.isFile = isFile;
    this.Parent = parent;
    this.Children = children;
  }
}

interface FileNode {
  Parent: string;
  Value: string;
  isFile: boolean;
  Children: FileNode[];
}

function serializeFilesSystemToTree(node: FileNode | TreeNode) {
  let parent = new TreeNode(node.Value, node.isFile, null, []);
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
}

enum Commands {
  HELP = "help",
  REQUEST = "request",
  LS = "ls",
  CD = "cd",
  CAT = "cat",
}

@Component
export default class Home extends Vue {

  // Connects to the API
  commandservice = new CommandServiceViewModel();
  userservice = new UserServiceViewModel();

  // The stored string the user is typing
  userInput: string = "";

  // File path
  path: TreeNode = new TreeNode("", false, null, []);
  hostname = `[\x1b[34mintellitect\x1B[0m@usrname ${this.path.Value}]$ `;
  updatePath(location: TreeNode) {
    this.path = location;
    this.hostname = `[\x1b[34mintellitect\x1B[0m@usrname ${location.Value}]$ `;
  }

  // Position the cursor is currently at. This is needed for back spaces.
  cursorPosition = this.hostname.length;
  term = new Terminal({ cursorBlink: true });

  history = [];
  welcomeMessage = "Welcome to the Intellitect CLI! View commands by typing help";

  async created() {

    // XTerms input
    const input = document.getElementById('terminal');
    if (input != null) {

      // Request for the file system
      await this.userservice.initializeFileSystem(null);

      // Get user
      let user = this.userservice.initializeFileSystem.result;

      // Serialize and cache filesystem to a Tree
      console.log(JSON.parse(user?.fileSystem!));
      this.path = serializeFilesSystemToTree(JSON.parse(user?.fileSystem!));

      // Command services
      await this.commandservice.request("3A20F4E1-628F-4FD2-810B-6ABC9EB7D34F");
      let challengeresult = this.commandservice.request.result;


      console.table(user);
      console.table(challengeresult);
      console.log(challengeresult);
      this.initTerminal(input)
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

      case Keys.ARROW_LEFT:
        // If the cursor is going out of our text... dont
        if (this.cursorPosition == this.hostname.length) { return; }
        this.term.write("\x1B[D");
        this.cursorPosition--;
        return;

      case Keys.ARROW_RIGHT:
        // If the cursor is going out of our text... dont
        if (this.cursorPosition >= (this.hostname.length + this.userInput.length)) { return; }
        this.term.write("\x1B[C");
        this.cursorPosition++;
        return;

      // Breaks functionality that xterm already gives arrows
      case Keys.ARROW_UP:
        break;

      // Breaks functionality that xterm already gives arrows
      case Keys.ARROW_BOTTOM:
        break;

      default:
        console.log(event);
        this.term.write(event.key);
        this.userInput += event.key;
        this.cursorPosition++;
    }
  }

  async commandHandler(cmd: string, arg: string[]) {

    function err(prefix: string, term: Terminal, msg: string) {
      term.write(`${prefix}: ${msg} \r\n`);
    }

    function unknownArg(prefix: string, term: Terminal, unArg: string) {
      err(prefix, term, `Unknown argument '${unArg}'`);
    }


    switch (cmd.toLocaleLowerCase()) {
      case Commands.HELP:
        console.log(arg);
        if (arg.length > 0) {
          unknownArg(Commands.HELP, this.term, arg[0]);
          break;
        }

        this.term.write(" help - Displays this message");
        this.term.write("\r\n");
        this.term.write(" ls - Lists all files in the current directory");
        this.term.write("\r\n");
        this.term.write(" cat - Displays the contents of a file");
        this.term.write("\r\n");
        this.term.write(" mkdir - Creates a directory");
        this.term.write("\r\n");
        this.term.write(" challenge - Requests a challenge");
        this.term.write("\r\n");
        this.term.write(" submit - Submits a challenge");
        this.term.write("\r\n");
        this.term.write(" edit - Edits a file");
        this.term.write("\r\n");
        this.term.write(" progress - Displays your progress");
        this.term.write("\r\n");
        this.term.write(" verify - Verifies a challenge");
        this.term.write("\r\n");
        break;

      case Commands.REQUEST:
        if (arg.length > 0) {
          unknownArg(Commands.REQUEST, this.term, arg[0]);
          break;
        }
        console.log(
          await this.commandservice.request("3A20F4E1-628F-4FD2-810B-6ABC9EB7D34F")
        );
        break;

      case Commands.LS:
        if (arg.length > 0) {
          unknownArg(Commands.LS, this.term, arg[0]);
          break;
        }
        this.path.Children.forEach((child: TreeNode) =>
          this.term.write(child.Value + "\r\n")
        );
        break;

      // TODO: cd into a direct path ex: cd /home/user
      case Commands.CD:

        // Arg[0] is required
        if (arg[0] == undefined || arg[0] == ".") {
          break;
        }

        if (arg[0] == "..") {
          if (this.path.Parent != null) {
            this.updatePath(this.path.Parent);
          }
          break;
        }

        // Search for the file
        var location: TreeNode | null = null;
        this.path.Children.forEach(
          (child: TreeNode) => location = child.Value == arg[0] ? child : null
        );
        if (location == null) {
          err(Commands.CD, this.term, `Directory not found '${arg[0]}'`);
          break;
        }
        if ((location as TreeNode).isFile) {
          err(Commands.CD, this.term, "Argument is a file and not a directory");
          break;
        }

        this.updatePath(location);
        break;

      // TODO: cat into a direct path ex: cat /home/user/readme.txt
      case Commands.CAT:
        // Arg[0] is required
        if (arg[0] == undefined) {
          err(Commands.CAT, this.term, "Missing file argument path");
          break;
        }
        let file: TreeNode | null = null;
        this.path.Children.forEach((child) => file = (child.Value == arg[0]) ? child : null);
        if (file == null) {
          err(Commands.CAT, this.term, `File not found '${arg[0]}'`)
          break;
        }
        if (!(file as TreeNode).isFile) {
          err(Commands.CAT, this.term, "Argument is a directory and not a file");
        }
        break;

      default:
        err("intelliterm", this.term, `Command not found '${cmd}'`)
        this.term.write("\r\n");
    }
  }
}
</script>
