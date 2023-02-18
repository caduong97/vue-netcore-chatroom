<template>
  <v-list dense nav >
    <v-hover
      v-slot="{ hover }"
      open-delay="200"
      v-for="chat in chats" :key="chat.id" 
    >
    <v-list-item 
      @click.stop="onChatClick(chat.id)"
    >
      <v-list-item-icon>
        <v-icon>mdi-chat</v-icon>
      </v-list-item-icon>
      <v-list-item-content class="text-left">
        <v-list-item-title>{{ chat.name }}</v-list-item-title>

      </v-list-item-content>
      <MoreAction
        :items="moreActionItems"
        :target="chat.id"
        :nudges="[6,17,0,0]"
        :hovered="hover ? true : false"
        @moreActionToggled="onMoreActionToggled"
        @moreActionItemClicked="onMoreActionItemClicked"
      ></MoreAction>
    </v-list-item>
    </v-hover>
  </v-list>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import ChatStore from "@/store/ChatStore";
import { Vue, Component } from "vue-property-decorator";
import MoreAction, { IMoreActionItem } from "@/components/MoreAction.vue"

@Component({
  name: "ChatNavigation",
  components: {
    MoreAction
  }
})
export default class ChatNavigation extends Vue {
  moreActionItems: IMoreActionItem[] = [
    {id: "manageChatUsers", text: "Add people", disabled: false, dangerous: false},
    {id: "manageChatName", text: "Rename", disabled: false, dangerous: false}
  ]
  hover!: boolean;


  get chats(): Chat[] {
    return ChatStore.chats;
  }

  getMoreActionItemById(id: string): IMoreActionItem | null {
    return this.moreActionItems.find(i => i.id == id) ?? null;
  }

  onChatClick(chatId: string) {
    this.$router.push({name: 'Chat', params: { chatId: chatId} })
  }

  onMoreActionToggled(opened: boolean) {
  }

  onMoreActionItemClicked(payload: {moreActionItemId: string, moreActionItemTarget: any}) {

    const clickedItem = this.getMoreActionItemById(payload.moreActionItemId);
    if (clickedItem == null) return

    switch (clickedItem.id) {
      case "manageChatUsers":
        this.onManageChatUsersClick(payload.moreActionItemTarget)
        break;
      case "manageChatName":
        this.onManageChatName(payload.moreActionItemTarget)
        break;
      default:
        break;
    }

  }

  onManageChatUsersClick(chatId: string) {
    console.log("onManageChatUsersClick", chatId)
  }

  onManageChatName(chatId: string) {
    console.log("onManageChatName", chatId)
  }
}
</script>

<style>

</style>