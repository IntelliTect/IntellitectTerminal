<template>
  <v-container>
    <v-row>
      <v-col cols="12" lg="7">
        <v-card>
          <v-card-title>
            {{ msg }}
          </v-card-title>
          <v-card-text class="black--text">
            <h2 class="headline font-weight-light mb-3">
              Here are some next steps to get started:
            </h2>
            <ol>
              <li>
                <strong>Create your data model</strong> by adding classes to the
                Models folder in your data project, and adding DbSet properties
                on AppDbContext.
              </li>
              <li>
                <strong>Create an EF migration</strong> by running
                <code>dotnet ef migrations add MigrationName</code>
                in your .Data project. The pages below will not work until you
                do so.
              </li>
              <li>
                <i class="fas fa-star" />
                <strong> Implement a proper authentication mechanism</strong> in
                Program.cs and remove the existing dummy authentication. Options
                include
                <a
                  href="https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity"
                >
                  Identity
                </a>
                and
                <a
                  href="https://docs.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-6.0#kestrel"
                >
                  Windows Authentication
                </a>

                <br />
                <br />
                This template authenticates all users as a dummy user to
                facilitate rapid initial development. Since all Coalesce
                endpoints require authentication by default, you would otherwise
                have to setup a real authentication scheme before you could
                begin development.
              </li>
            </ol>
            <v-divider class="my-3" />
            <router-link
              :to="{ name: 'coalesce-example' }"
              class="headline font-weight-light"
            >
              View a Simple Custom Page Example
            </router-link>
          </v-card-text>
        </v-card>

        <v-card class="mt-5">
          <v-card-title>
            Admin Page Links
            <v-spacer></v-spacer>
            <a href="/coalesce-security" class="headline font-weight-light">
              Security Overview
            </a>
          </v-card-title>
          <v-card-text class="black--text">
            <v-simple-table dense>
              <thead>
                <tr>
                  <th>Type</th>
                  <th>Admin Table</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="type in adminTypes" :key="type.name">
                  <td>
                    {{ type.displayName }}
                  </td>
                  <td>
                    <router-link
                      :to="{
                        name: 'coalesce-admin-list',
                        params: { type: type.name },
                      }"
                    >
                      Table
                    </router-link>
                  </td>
                </tr>
              </tbody>
            </v-simple-table>
          </v-card-text>
        </v-card>
      </v-col>

      <v-col cols="12" lg="5">
        <v-card>
          <v-card-title> Quick Links </v-card-title>
          <v-card-text class="black--text">
            <h2 class="headline font-weight-light mb-3">Documentation</h2>
            <ul>
              <li>
                <a href="https://vuejs.org/v2/guide/">Vue Documentation</a>
              </li>
              <li>
                <a href="https://vitejs.dev/">Vite Documentation</a>
              </li>
              <li>
                <a href="https://vuetifyjs.com/en/getting-started/quick-start/"
                  >Vuetify Documentation</a
                >
              </li>
              <li>
                <a
                  href="https://intellitect.github.io/Coalesce/stacks/vue/overview"
                  >Coalesce Documentation</a
                >
              </li>
              <li>
                <a
                  href="https://intellitect.github.io/Coalesce/stacks/vue/coalesce-vue-vuetify/overview"
                  >Coalesce Vuetify Documentation</a
                >
              </li>
            </ul>
            <v-divider class="my-3" />
            <h2 class="headline font-weight-light mb-3">Resources</h2>
            <ul>
              <li>
                Check out
                <a href="https://github.com/vuejs/awesome-vue" target="_blank"
                  >🎉 awesome-vue</a
                >
                for Vue components, libraries, examples, and more!
              </li>
              <li>
                Install
                <a
                  href="https://github.com/vuejs/vue-devtools#vue-devtools"
                  target="_blank"
                  >vue-devtools</a
                >
                for enhanced in-browser Vue development experience.
              </li>
            </ul>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

import $metadata from "@/metadata.g";

@Component
export default class HelloWorld extends Vue {
  @Prop() private msg!: string | null;

  adminTypes = Object.values($metadata.types).filter((t) => t.type == "model");
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
