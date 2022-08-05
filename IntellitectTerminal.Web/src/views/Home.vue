<template>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Terminal } from "xterm";

@Component
export default class Home extends Vue {

  helpView(term: Terminal) {
    term.write("\r\n");
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
    term.write('\x1B[1;3;31mIntelliTect\x1B[0m $ ');
  }

  async created() {

    // The stored string the user is typing
    var temp: string = "";

    // File path, change this later
    const PATH = 'Hello from \x1B[1;3;31mIntellitect\x1B[0m $ ';

    // Position the cursor is currently at. This is needed for back spaces.
    var cursorPosition = PATH.length;
    var term: Terminal = new Terminal();

    var history = [];

    // XTerms input
    const input = document.getElementById('terminal');
    if (input != null) {
      term.open(input);

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
            temp = temp.substring(0, temp.length - 1);
            cursorPosition--;
            return;

          // Enter key
          case "\r":
            term.write(e.key);
            temp += e.key;
            term.write("\n");

            // Print the command name out :)
            if (temp.trim() != "") {
              term.write(temp);
              term.write("\n");
              history.push(temp);
            }
            term.write(PATH);
            temp = "";
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
            if (cursorPosition >= (PATH.length + temp.length)) { return; }
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
            temp += e.key;
            cursorPosition++;
        }
      })

    }
  }
}
</script>
