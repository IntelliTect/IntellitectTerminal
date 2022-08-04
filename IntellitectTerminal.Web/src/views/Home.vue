<template>
</template>

<script lang="ts">
  import { Component, Prop, Vue } from "vue-property-decorator";
  import { Terminal } from "xterm";

  @Component
  export default class Home extends Vue {

    async created() {
      var temp: string = "";
      var term = new Terminal();
      const input = document.getElementById('terminal');
      if (input != null) {
        term.open(document.getElementById('terminal') as HTMLElement);
        term.write('Hello from \x1B[1;3;31mIntelliTect\x1B[0m $ ');
        term.onKey(e => {
          console.log(e.key);
          temp += e.key;
          term.write(e.key);
          if (e.key == '\r') {
            term.write('\n');
            term.write('Hello from \x1B[1;3;31mIntellitect\x1B[0m $ ');
            temp = "";
            console.log('cleared temp');
          }
        })
      }
    }
  }
</script>
