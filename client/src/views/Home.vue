<template>
  <div>
    <v-app-bar elevation="4" color="primary">
      <v-spacer></v-spacer>
      <v-menu offset-y class="ml-auto mr-0">
        <template v-slot:activator="{ on, attrs }">
          <v-avatar
            color="#fff"
            size="40"
            v-bind="attrs"
            v-on="on"
          >
            <span v-if="me">{{ me.initials }}</span>
          </v-avatar>
        </template>
        <v-list>
          <v-list-item @click="onSignOutClick">
            <v-list-item-title>Sign out</v-list-item-title>  
          </v-list-item>
        </v-list>
      </v-menu>
    </v-app-bar>

    <v-container class="home">
      <h1>Welcome!</h1>

      <v-card elevation="3" max-width="500" class="home_profile-card">
        <v-card-title>My profile</v-card-title>
        <v-card-text>
          <v-form ref="userProfileForm">
            <v-row>
              <v-col cols="6">
                <v-text-field
                  v-model="user.firstName"
                  label="First name"
                  :rules="[formRules.nameRequired]"
                  :disabled="loading"
                ></v-text-field>
              </v-col>
              <v-col cols="6">
                <v-text-field
                  v-model="user.lastName"
                  label="Last name"
                  :disabled="loading"
                ></v-text-field>
              </v-col>
              <v-col cols="6">
                <v-text-field
                  v-model="user.email"
                  label="Email"
                  readonly
                  :disabled="loading"
                ></v-text-field>
              </v-col>
              <v-col cols="6">
                <v-text-field
                  v-model="user.location"
                  label="Location"
                  :disabled="loading"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-textarea
                  v-model="user.bio"
                  label="Bio"
                  :disabled="loading"
                ></v-textarea>
              </v-col>
            </v-row>
          </v-form>
          
        </v-card-text>

        <v-card-actions>
          <v-col>
            <v-btn 
              width="100%" 
              @click="saveChangesClick" 
              :disabled="loading || disableSaveChanges">Save changes</v-btn>
          </v-col>
        </v-card-actions>
      </v-card>

    </v-container>
  </div>
</template>

<script lang="ts">
import User from '@/models/User';
import AuthStore from '@/store/AuthStore';
import UserStore from '@/store/UserStore';
import { Component, Vue, Watch } from 'vue-property-decorator';
import _ from "lodash";

@Component({
  components: {
  },
})
export default class Home extends Vue {
  user: User = new User();
  formRules: any = {
    nameRequired: (v: any) => !!v || 'Required',
  }
  loading: boolean = false;
  disableSaveChanges: boolean = false;

  get me(): User | null {
    return UserStore.me;
  }

  @Watch("me")
  onMeChange(newVal: User | null, oldVal: User | null) {
    if (newVal !== null) {
      this.user = User.copyFrom(newVal)
    }
  }
  @Watch("user", { deep: true })
  onUserChange(newVal: User) {
    if (!this.me) {
      return
    }
    if (_.isEqual(this.me, newVal)) {
      this.disableSaveChanges = true;
    } else {
      this.disableSaveChanges = false;
    }
  }

  async onSignOutClick() {
    if (AuthStore.msalAuthenticated) {
      try {
        await this.$msal.signOut();
      } catch (error) {
        console.error(error);
        alert("Error signing out with O365: " + error);
      }
    }
    if (AuthStore.googleAuthenticated) {
      try {
        await this.$firebaseAuth.signOutWithGoogle();
      } catch (error) {
        console.error(error);
        alert("Error signing out with Google: " + error);
      }
    }

    (window as any).location.href = window.location.origin;
  }

  async saveChangesClick() {
    if (!(this.$refs.userProfileForm as any).validate()) {
      alert("Form invalid")
      return 
    }

    this.loading = true;
    try {
      await UserStore.updateUser(this.user);
    } catch (error) {
      console.error(error)
      alert("Error when updating user: " + error)
    }
    this.loading = false;
  }

  

  mounted() {
    if (this.me !== null) {
      this.user = User.copyFrom(this.me)
    }
  }

}
</script>

<style lang="scss">
.home {
  padding: 3rem;
  box-sizing: border-box;

  &_profile-card {
    margin: 2rem auto;

  }
}
</style>
