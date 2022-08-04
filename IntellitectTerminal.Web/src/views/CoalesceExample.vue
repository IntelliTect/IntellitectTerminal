<template>
  <v-container class="white elevation-1" style="max-width: 900px">
    <v-row no-gutters>
      <h1>{{ title }}</h1>
      <v-spacer />
      <v-btn large to="/admin/ApplicationUser" color="primary">
        Application User Admin Table
      </v-btn>
    </v-row>

    <v-divider class="mt-4" />

    Below is a very simple example of using components from coalesce-vue-vuetify
    to display and edit properties of a model. Autosave is enabled.

    <c-loader-status
      #default
      :loaders="{
        'no-error-content no-intial-content': [user.$load],
        '': [user.$save],
      }"
    >
      <div class="title py-2">
        Editing User ID: <c-display :model="user" for="applicationUserId" />
      </div>
      <c-input :model="user" for="name" />
    </c-loader-status>
  </v-container>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { ApplicationUserViewModel } from "@/viewmodels.g";

@Component
export default class CoalesceExample extends Vue {
  @Prop({ required: true, type: String })
  public title!: string;

  private user = new ApplicationUserViewModel();

  async created() {
    await this.user.$load(1);
    this.user.$startAutoSave(this, { wait: 500, debounce: { maxWait: 3000 } });
  }
}
</script>
