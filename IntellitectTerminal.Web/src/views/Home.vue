<template>
</template>

<script lang="ts">
  import { Component, Prop, Vue } from "vue-property-decorator";
  import { Terminal } from "xterm";
  

@Component
export default class Home extends Vue {

  helpView(term: Terminal) {
    term.write("help - Displays this message");
    term.write("\r\n");
    term.write("ls - Lists all files in the current directory");
    term.write("\r\n");
    term.write("cat - Displays the contents of a file");
    term.write("\r\n");
    term.write("mkdir - Creates a directory");
    term.write("\r\n");
    term.write("challenge - Requests a challenge");
    term.write("\r\n");
    term.write("submit - Submits a challenge");
    term.write("\r\n");
    term.write("edit - Edits a file");
    term.write("\r\n");
    term.write("progress - Displays your progress");
    term.write("\r\n");
    term.write("verify - Verifies a challenge");
    term.write("\r\n");
  }

  async created() {

    // The stored string the user is typing
    var userInput: string = "";

    // File path, change this later
    const PATH = '\x1B[1;3;31mIntellitect\x1B[0m $ ';

    // Position the cursor is currently at. This is needed for back spaces.
    var cursorPosition = PATH.length;
    var term = new Terminal({ cursorBlink: true });

    var history = [];
    var welcomeMessage = "Welcome to the Intellitect CLI! View commands by typing help";

    // XTerms input
    const input = document.getElementById('terminal');
    if (input != null) {
      term.open(input);
      term.write(welcomeMessage);
      term.write("\r\n");
      term.write(PATH);

      // Main key handler. Anything pressed goes here.
      term.onKey((e) => {
        console.log(e);

        // Command keys
        switch (e.key) {
          // Backspace key
          case "\x7F":
            // If the cursor is going to delete from our path... dont
            if (cursorPosition == PATH.length) { return; }
            term.write("\b \b");
            userInput = userInput.substring(0, userInput.length - 1);
            cursorPosition--;
            return;

          // Enter key
          case "\r":
            term.write(e.key);
            userInput += e.key;
            term.write("\n");

            if (userInput.trim() == "help") {
              this.helpView(term);
            }
            
            term.write(PATH);
            userInput = "";
            cursorPosition = PATH.length;
            return;

          // // Left arrow
          case "\x1B[D":
            // If the cursor is going out of our text... dont
            if (cursorPosition == PATH.length) { return; }
            term.write("\x1B[D");
            cursorPosition--;
            return;

          // Right arrow
          case "\x1B[C":
            // If the cursor is going out of our text... dont
            if (cursorPosition >= (PATH.length + userInput.length)) { return; }
            term.write("\x1B[C");
            cursorPosition++;
            return;

          // Top arrow
          case "\x1B[A":
            break;

          // Bottom arrow
          case "\x1B[B":
            break;
          
          default:
            term.write(e.key);
            userInput += e.key;
            cursorPosition++;
        }
      })

    }
  }
}
</script>
