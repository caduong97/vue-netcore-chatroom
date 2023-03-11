<template>
  <v-list dense nav >
    <ChatNavigationItem v-for="chat in chats" :key="chat.id"  :chat="chat"  />
  </v-list>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import ChatStore from "@/store/ChatStore";
import { Vue, Component } from "vue-property-decorator";
import ChatNavigationItem from "@/components/ChatNatigationItem.vue";

@Component({
  name: "ChatNavigation",
  components: {
    ChatNavigationItem
  }
})
export default class ChatNavigation extends Vue {
  get chats(): Chat[] {
    return ChatStore.chats;
  }
}
</script>

<style>

</style>