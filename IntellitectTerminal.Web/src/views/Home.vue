<template>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Terminal } from "xterm";
import { CommandServiceViewModel } from '@/viewmodels.g';

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
}

@Component
export default class Home extends Vue {

  commandservice = new CommandServiceViewModel();
  // The stored string the user is typing
  userInput: string = "";

  // File path, change this later
  path = '\x1B[1;3;31mIntellitect\x1B[0m $ ';

  // Position the cursor is currently at. This is needed for back spaces.
  cursorPosition = this.path.length;
  term = new Terminal({ cursorBlink: true });

  history = [];
  welcomeMessage = "Welcome to the Intellitect CLI! View commands by typing help";

  async created() {
    // XTerms input
    const input = document.getElementById('terminal');
    if (input != null) {
      this.initTerminal(input)
    let challengeresult = await this.commandservice.requestCommand("3A20F4E1-628F-4FD2-810B-6ABC9EB7D34F");
    console.table(challengeresult);
    console.log(challengeresult);
    }
  }

  initTerminal(input: HTMLElement) {
    this.term.open(input);
    // Message of the Day
    this.term.write(this.welcomeMessage);
    this.term.write("\r\n");

    // Log path you are on
    this.term.write(this.path);

    // Main key handler. Anything pressed goes here.
    this.term.onKey((e) => this.keyPressedHandler(e));
  }

  keyPressedHandler(event: { key: any; domEvent?: KeyboardEvent; }) {

    // Command keys
    switch (event.key) {

      case Keys.BACKSPACE:
        // If the cursor is going to delete from our path... dont
        if (this.cursorPosition == this.path.length) { return; }
        this.term.write("\b \b");
        this.userInput = this.userInput.substring(0, this.userInput.length - 1);
        this.cursorPosition--;
        return;

      case Keys.ENTER:
        this.term.write(event.key);
        this.userInput += event.key;
        this.term.write("\n");

        this.commandHandler(this.userInput.trim());

        this.term.write(this.path);
        this.userInput = "";
        this.cursorPosition = this.path.length;
        return;

      case Keys.ARROW_LEFT:
        // If the cursor is going out of our text... dont
        if (this.cursorPosition == this.path.length) { return; }
        this.term.write("\x1B[D");
        this.cursorPosition--;
        return;

      case Keys.ARROW_RIGHT:
        // If the cursor is going out of our text... dont
        if (this.cursorPosition >= (this.path.length + this.userInput.length)) { return; }
        this.term.write("\x1B[C");
        this.cursorPosition++;
        return;

      // By breaking we dont allow xterm to handle the arrow key itself
      case Keys.ARROW_UP:
        break;
      // By breaking we dont allow xterm to handle the arrow key itself
      case Keys.ARROW_BOTTOM:
        break;

      default:
        console.log(event);
        this.term.write(event.key);
        this.userInput += event.key;
        this.cursorPosition++;
    }
  }

  commandHandler(cmd: string) {
    switch (cmd) {
      case Commands.HELP:
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
        this.term.write(this.path);
        break;
      default:
        this.term.write("Command not found.");
        this.term.write("\r\n");
    }
  }
}
</script>
