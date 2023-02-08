<template>
  <v-container 
    fluid
    class="signup-page ma-0 d-flex justify-center primary fill-height fill-width" 
  >
    <v-form ref="signUpForm">
      <v-card
        class="mx-auto"
        elevation="2"
        max-width=500
        min-width=300
        width=500
      >
        <v-card-title>
          <h4>Sign up with email and password</h4>
        </v-card-title>
        <v-card-text class="mt-5">
            <v-row class="px-3 pt-3">
              <v-col cols="12" class="pa-0">
                <v-text-field
                  v-model="registerPasswordUserRequest.email"
                  label="Email"
                  dense
                  outlined
                  :rules="[rules.required]"
                ></v-text-field>
              </v-col>
              <v-col cols="12" class="pa-0">
                <v-text-field
                  v-model="registerPasswordUserRequest.firstName"
                  label="First name"
                  dense
                  outlined
                ></v-text-field>
              </v-col>
              <v-col cols="12" class="pa-0">
                <v-text-field
                  v-model="registerPasswordUserRequest.lastName"
                  label="Last name"
                  dense
                  outlined
                ></v-text-field>
              </v-col>
              <v-col cols="12" class="pa-0" >
                <v-text-field
                  v-model="registerPasswordUserRequest.password"
                  label="Password"
                  dense
                  outlined
                  type="password"
                  hint="At least 8 characters"
                  counter
                  :rules="[rules.required]"
                ></v-text-field>
              </v-col>
              <v-col cols="12" class="pa-0" >
                <v-text-field
                  v-model="registerPasswordUserRequest.confirmPassword"
                  label="Confirm password"
                  dense
                  outlined
                  type="password"
                  :rules="[rules.required, rules.passwordMatch]"
                ></v-text-field>
              </v-col>
            </v-row>
        </v-card-text>
        <v-card-actions>
          <v-row no-gutters class="pa-3 pb-3 pt-0">
            <v-col cols="12">
              <v-btn 
                width="100%"
                @click="onSignUpClick"
              >
                <v-icon>mdi-email-outline</v-icon>
                <span class="px-3">Sign up</span>  
              </v-btn>
            </v-col>    
            <v-col cols="12" class="pt-8 pb-3 px-3">
              Back to <router-link to="/signIn">sign in</router-link></v-col>
          </v-row>
          
        </v-card-actions>
      </v-card>
    </v-form>
  </v-container>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import RegisterPasswordUserRequest from "@/models/RegisterPasswordUserRequest"
import AuthStore from "@/store/AuthStore";

@Component({
  name: "SignUp"
})
export default class SignUp extends Vue {
  registerPasswordUserRequest = new RegisterPasswordUserRequest();
  rules = {
    required: (value: string) => !!value || "Required",
    // passwordMin: (value: string) => value.length >= 8 || 'Must be at least 8 characters',
    passwordMatch: (value: string) => value === this.registerPasswordUserRequest.password || "The confirm password doesn't match"
  }

  

  async onSignUpClick() {
    if (!(this.$refs.signUpForm as any).validate()) {
      return
    }

    try {
      await AuthStore.register(this.registerPasswordUserRequest);
    } catch (error) {
      console.error("Error when trying to register:",error)
    }

  }
}
</script>

<style lang="scss">
</style>