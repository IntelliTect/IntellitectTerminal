<template>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Terminal } from "xterm";

@Component
export default class Home extends Vue {

  async created() {

    // The stored string the user is typing
    var temp: string = "";

    // File path, change this later
    const PATH = 'Hello from \x1B[1;3;31mIntellitect\x1B[0m $ ';

    // Position the cursor is currently at. This is needed for back spaces.
    var cursorPosition = PATH.length;
    var term: Terminal = new Terminal();

    // XTerms input
    const input = document.getElementById('terminal');
    if (input != null) {
      term.open(input);

      term.write(PATH);

      // Main key handler. Anything pressed goes here.
      term.onKey((e) => {
        console.log(cursorPosition);

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
            term.write("\n");
            term.write(PATH);
            temp = "";
            return;

          // // Left arrow
          case "\x1B[D":
            // If the cursor is going out of our text... dont
            if (cursorPosition == PATH.length) {return;}
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
