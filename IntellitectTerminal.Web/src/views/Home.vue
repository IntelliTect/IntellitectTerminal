<template>
  <div class="home">
    <!-- Component is auto-imported by `unplugin-vue-components`
    (either kebab-case or PascalCase works.) -->
    <HelloWorld msg="Welcome to your new Vue.js Coalesce application!" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component({})
export default class Home extends Vue {}
</script>
