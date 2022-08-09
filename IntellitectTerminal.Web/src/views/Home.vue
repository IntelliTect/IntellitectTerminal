<template>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import { IntelliTerm } from "../scripts/intelliterm";
import { API } from "../scripts/utils";
import * as Cookies from 'tiny-cookie';
import { CommandServiceViewModel, UserServiceViewModel } from "@/viewmodels.g";

@Component
// WARNING: Despite being a vue project we are just using typescript with the XTerm api :) :) :)
export default class Home extends Vue {

  
  async created() {

    // XTerms input
    const xtermElement = document.getElementById('terminal');

    // Get the file input element that is hidden in index.html
    const fileUploader = document.getElementById("file") as HTMLInputElement;

    if (xtermElement != null) {

      const api: API = {
        command: new CommandServiceViewModel(),
        user: new UserServiceViewModel()
      }

       let userId = getCookie('userId');
       let user;
        if (userId == null) {
        // if user is not in cookies, get user id from api
        await api.user.initializeFileSystem(null);
        user = api.user.initializeFileSystem.result;
        userId = user!.userId;
      } else {
        // if user is in cookies, get user from api
        // Request for the file system
      // Request for the file system
      await api.user.initializeFileSystem(null);

      // Get user
      user = api.user.initializeFileSystem.result;
      }



      if (user != null) {

        if(userId != null) {
          setCookie('userId', userId, { expires: 100 });
        } 

        // spawn the intelliterm console into the html element
        const intelliTerm = new IntelliTerm(api);
        intelliTerm.init(xtermElement, fileUploader);
      }
    }
  }
}

</script>