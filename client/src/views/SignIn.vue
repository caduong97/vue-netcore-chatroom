<template>
  <v-container 
    fluid
    class="signin-page ma-0 d-flex justify-center primary fill-height fill-width" 
  >
      <v-card
        class="mx-auto"
        elevation="2"
        max-width=500
        min-width=300
        width=500
      >
        <v-card-title>
          <h4>Sign in</h4>
        </v-card-title>

        <v-card-actions>
          <v-row no-gutters class="pa-3">
            <v-col cols="12" class="mb-5">
              <v-btn @click="onSignInO365Click" width="100%">
                <v-icon>mdi-microsoft</v-icon>
                <span class="px-3">Sign in with Office 365 account</span>  
              </v-btn>
            </v-col>
            <v-col cols="12">
              <v-btn @click="onSignInGoogleClick" width="100%">
                <v-icon>mdi-google</v-icon>
                <span class="px-3">Sign in with Google account</span>  
              </v-btn>
            </v-col>
          </v-row>
          
        </v-card-actions>

        <v-row no-gutters class="px-6 py-3 d-flex align-center">
          <v-divider></v-divider>
          <span style="white-space: pre-wrap;">  OR  </span>
          <v-divider></v-divider>
        </v-row>

        <v-card-text>
          <v-form ref="signInEmailPasswordForm">
            <v-row class="px-3 pt-3">
              <v-col cols="12" class="pa-0">
                <v-text-field
                  v-model="emailPasswordAuthenticationRequest.email"
                  label="Email"
                  dense
                  outlined
                  :rules="[rules.required]"
                ></v-text-field>
              </v-col>
              <v-col cols="12" class="pa-0" >
                <v-text-field
                  v-model="emailPasswordAuthenticationRequest.password"
                  label="Password"
                  dense
                  outlined
                  type="password"
                  :rules="[rules.required]"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>


        </v-card-text>

        <v-card-actions>
          <v-row no-gutters class="pa-3 pb-3 pt-0">
            <v-col cols="12">
              <v-btn @click="onSignInEmailClick" width="100%">
                <v-icon>mdi-email-outline</v-icon>
                <span class="px-3">Sign in with email</span>  
              </v-btn>
            </v-col>    
            <v-col cols="12" class="pt-8 pb-3 px-3">
              New user? Do you want to <router-link to="/signUp">sign up</router-link> instead?</v-col>
          </v-row>
          
        </v-card-actions>

      </v-card>
    
  </v-container>
</template>

<script lang="ts">
import AuthenticationRequest from "@/models/AuthenticationRequest";
import AuthStore from "@/store/AuthStore";
import { Vue, Component } from "vue-property-decorator";

@Component({
  name: "SignIn"
})
export default class SignIn extends Vue {
  emailPasswordAuthenticationRequest = new AuthenticationRequest();
  rules = {
    required: (value: string) => !!value || "Required"
  }


  async onSignInO365Click() {
    try {
      await this.$msal.signIn();
    } catch (error) {
      console.error(error);
      alert("Error signing in with O365: " + error);
    }
  }

  async onSignInGoogleClick() {
    try {
      await this.$firebaseAuth.signInWithGoogle();
    } catch (error) {
      console.error(error);
      alert("Error signing in with Google: " + error);
    }
  }

  async onSignInEmailClick() {
    if (!(this.$refs.signInEmailPasswordForm as any).validate()) {
      return
    }

    try {
      await AuthStore.signIn(this.emailPasswordAuthenticationRequest);
    } catch (error) {
      console.error("Error when trying to sign in with email and password:",error)
    }
  }
}
</script>

<style lang="scss">
</style>