<template>
</template>

<script lang="ts">
  import { Component, Prop, Vue } from "vue-property-decorator";
 import { Terminal } from "xterm";


  @Component
  export default class CoalesceExample extends Vue {

    async created() {
      var userInput: string = "";
      var term = new Terminal({cursorBlink: true,});
      const input = document.getElementById('terminal');
      var welcomeMessage = "Welcome to the Intellitect CLI! View commands by typing help";

      if (input != null) {
        term.open(document.getElementById('terminal') as HTMLElement);
        term.write(welcomeMessage);
        term.write("\r\n");
        term.write('\x1B[1;3;31mIntelliTect\x1B[0m $ ');
        term.onKey(e => {
          //console.log(e.key);
          userInput += e.key;
          term.write(e.key);
          if (e.key == '\r') {
            if (userInput == "help\r") {
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
            else {
              term.write("\r\n");
              term.write('Command not found. Type "help" for a list of commands.');
              term.write("\r\n");
              term.write('\x1B[1;3;31mIntelliTect\x1B[0m $ ');
            }
            console.log(userInput); 
            userInput = "";
          }
        })  
        
      }
    }
  }
</script>
