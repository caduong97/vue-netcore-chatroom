<template>
  <div>
    <Navigation></Navigation>    
    <v-main>
      <v-container fluid class="d-flex align-center">
        <template v-if="!chatIdFromRouteParam">
          <v-row>
            <v-btn 
              color="primary"
              elevation="4"
              class="mx-auto"
              @click="onCreateClick"
            >Create a chat</v-btn>
          </v-row>
          <ChatCreationDialog></ChatCreationDialog>
        </template>

        <router-view></router-view>
      </v-container>
    </v-main>
  </div>
</template>

<script lang="ts">
import User from '@/models/User';
import UserStore from '@/store/UserStore';
import { Component, Vue } from 'vue-property-decorator';
import Navigation from "@/components/Navigation.vue";
import ChatStore from '@/store/ChatStore';
import Chat from '@/models/Chat';
import ChatCreationDialog from "@/components/ChatCreationDialog.vue"

@Component({
  components: {
    Navigation,
    ChatCreationDialog
  },
})
export default class Home extends Vue {
  loading: boolean = false;

  get me(): User | null {
    return UserStore.me;
  }

  get chats(): Chat[] {
    return ChatStore.chats;
  }

  get chatIdFromRouteParam() {
    return this.$route.params.chatId ?? null
  }

  onCreateClick() {
    this.$root.$emit("openCreateChatDialog")
  }
  
  async fetchData() {
    if (this.chats.length === 0) {
      await ChatStore.getChats();
    }
  }

  async created() {
    await this.fetchData();
  }

}
</script>

<style lang="scss">
.v-main {
  min-height: 100vh;
}
.v-main .container {
  height: 100%;
}
</style>
