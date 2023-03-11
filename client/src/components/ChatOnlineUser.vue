<template>
  <v-tooltip right >
    <template v-slot:activator="{ on, attrs }">
      <v-badge
        bordered
        top
        color="green darken-1"
        dot
        offset-x="10"
        offset-y="10"
        avatar
      >
        <v-avatar
          v-if="onlineUser"
          color="#81D4FA"
          size="38"
          v-bind="attrs"
          v-on="on"
          class="ml-2"
        >
          <span>{{ onlineUser.initials }}</span>
        </v-avatar>
        <v-avatar
          v-if="onlineUserCount"
          color="#81D4FA"
          size="38"
          v-bind="attrs"
          v-on="on"
          class="ml-2"
        >
          <span>+{{ onlineUserCount }}</span>
        </v-avatar>
      </v-badge>
    </template>
    <span v-if="onlineUser" class='text-caption'>{{ onlineUser.fullName }} is online</span>
    <span v-else v-html="moreThanOneOnlineUserText"></span>
  </v-tooltip>
</template>

<script lang="ts">
import User from "@/models/User";
import { Vue, Component, Prop } from "vue-property-decorator";

@Component({
  name: "ChatOnlineUser"
})
export default class ChatOnlineUser extends Vue {
  @Prop({ required: false, default: null })
  onlineUser!: User;

  @Prop({ required: false, default: 0})
  onlineUserCount!: number;

  @Prop({ required: false, default: null })
  allOnlineUsers!: User[];

  get moreThanOneOnlineUserText(): string {
    if (this.allOnlineUsers.length > 0) {
      return `<span class='text-caption'>Online: ${this.allOnlineUsers.map(u => u.fullName).join(', ')}</span>`
    }
    return ""
  }

}
</script>

<style>

</style>