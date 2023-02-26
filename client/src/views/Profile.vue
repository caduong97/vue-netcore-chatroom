<template>
  <v-row class="pa-5">
    <v-col cols="12">
      <h1 class="text-center">Welcome!</h1>
      <v-card elevation="3" max-width="500" class="profile-card">
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
    </v-col>
  </v-row>
</template>

<script lang="ts">
import User from '@/models/User';
import AuthStore from '@/store/AuthStore';
import UserStore from '@/store/UserStore';
import { Component, Vue, Watch } from 'vue-property-decorator';
import _ from "lodash";

@Component({
  name: "Profile",
  components: {
  },
})
export default class Profile extends Vue {
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

.profile-card {
    margin: 2rem auto;

  }
</style>