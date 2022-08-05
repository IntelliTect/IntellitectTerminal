<template>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Terminal } from "xterm";
import { CommandServiceViewModel, UserServiceViewModel } from '@/viewmodels.g';

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
  CLEAR = "clear"
}

@Component
export default class Home extends Vue {

  // Connects to the API
  commandservice = new CommandServiceViewModel();
  userservice = new UserServiceViewModel();

  // The stored string the user is typing
  userInput: string = "";

  // Entire file system
  filesystem: TreeNode = new TreeNode("/", false, null, []);

  // Path we are on
  path: TreeNode = new TreeNode("/", false, null, []);

  // Hostname relative to path we are on
  hostname = `[\x1b[34mintellitect\x1B[0m@usrname ${this.path.Name}]$ `;

  // Update the path we are on and display hostname correctly
  updatePath(location: TreeNode) {
    this.path = location;
    this.hostname = `[\x1b[34mintellitect\x1B[0m@usrname ${location.Name}]$ `;
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
      this.filesystem = serializeFilesSystemToTree(JSON.parse(user?.fileSystem!));
      this.path = this.filesystem;

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

    function err(prefix: string, term: Terminal, msg: string) {
      term.writeln(`${prefix}: ${msg}`);
    }

    function unknownArg(prefix: string, term: Terminal, unArg: string) {
      err(prefix, term, `Unknown argument '${unArg}'`);
    }


    switch (cmd.toLocaleLowerCase()) {
      case Commands.HELP:
        if (arg.length > 0) {
          unknownArg(Commands.HELP, this.term, arg[0]);
          break;
        }

        this.term.writeln(" help - Displays this message");
        this.term.writeln(" ls - Lists all files in the current directory");
        this.term.writeln(" cat - Displays the contents of a file");
        this.term.writeln(" mkdir - Creates a directory");
        this.term.writeln(" clear - Clear the terminal");
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
          this.term.write(child.Name + "\r\n")
        );
        break;

      // TODO: cd into a direct path ex: cd /home/user
      case Commands.CD:

        function validatePath(start: TreeNode, directions: string[], term: Terminal): TreeNode {
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
              err(Commands.CD, term, `Directory not found '${arg[0]}'`);
              return;
            }
            if (i.isFile) {
              err(Commands.CD, term, "Argument is a file and not a directory");
              return;
            }
            traverse = i;
            return true;
          })
          return traverse;
        }

        // Arg[0] is required
        if (arg[0] == undefined || arg[0] == ".") {
          break;
        }

        // Root level navigation
        if (arg[0].startsWith("/")) {

          // Get each direction we need from the arg[0]. EX /home/user/file [home,user,file]
          let directions = arg[0].split("/").filter(element => element == "" ? false : true);

          this.updatePath(validatePath(this.filesystem, directions, this.term));
          break;
        }

        // Directory level navigation. Endpoint must be inside of this.path
        // Get each direction we need from the arg[0]. EX ./user/file [user,file]
        let directions = arg[0].split("/").filter(element => element == "" || element == "." ? false : true);

        this.updatePath(validatePath(this.path, directions, this.term));
        break;

      // TODO: cat into a direct path ex: cat /home/user/readme.txt
      case Commands.CAT:
        // Arg[0] is required
        if (arg[0] == undefined) {
          err(Commands.CAT, this.term, "Missing file argument path");
          break;
        }
        let file: TreeNode | null = null;
        this.path.Children.forEach((child) => file = (child.Name == arg[0]) ? child : null);
        if (file == null) {
          err(Commands.CAT, this.term, `File not found '${arg[0]}'`)
          break;
        }
        if (!(file as TreeNode).isFile) {
          err(Commands.CAT, this.term, "Argument is a directory and not a file");
        }
        break;

      case Commands.CLEAR:
        if (arg.length > 0) {
          unknownArg(Commands.HELP, this.term, arg[0]);
          break;
        } 
        this.term.clear();
        break;

      default:
        err("intelliterm", this.term, `Command not found '${cmd}'`)
    }
  }
}
</script>
