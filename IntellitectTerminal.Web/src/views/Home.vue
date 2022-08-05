<template>
</template>

<script lang="ts">
  import { Component, Prop, Vue } from "vue-property-decorator";
  import { Terminal } from "xterm";

  @Component
  export default class CoalesceExample extends Vue {

    PATH = "\x1B[1;3;31mIntelliTect\x1B[0m $ ";
    
    helpView(term : Terminal) {
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
      var cursorPosition = 19;
      var userInput: string = "";      
      var term = new Terminal({ cursorBlink: true });
      var welcomeMessage = "Welcome to the Intellitect CLI! View commands by typing help";
      if (userInput != null) {
        term.open(document.querySelector("#terminal") as HTMLElement);
        term.write(welcomeMessage);
        term.write("\r\n");
        term.write(this.PATH);

        term.onKey(e => {
          switch (e.key) {
            // Backspace key
            case "\x7F":
              // If the cursor is going to delete from our path... dont
              if (cursorPosition <= this.PATH.length) { return; }
              term.write("\b \b");
              userInput = userInput.substring(0, userInput.length - 1);
              cursorPosition--;
              return;
            // Enter key
            case "\r":
              if (userInput == "help\r") {
                this.helpView(term);
              } 
              else {
                term.write("\r\n");
                term.write(userInput + ' Command not found. Type "help" for a list of commands.');
                term.write("\r\n");
                term.write(this.PATH);
              }
              userInput = "";
              return;
            // // Left arrow
            case "\x1B[D":
              // If the cursor is going out of our text... dont
              if (cursorPosition <= 19) {return;}
              term.write("\x1B[D");
              cursorPosition--;
              return;
            // Right arrow
            case "\x1B[C":
              // If the cursor is going out of our text... dont
              if (cursorPosition >= (this.PATH.length + userInput.length)) { return; }
              term.write("\x1B[C");
              cursorPosition++;
              return;
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
