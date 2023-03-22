<template>
  <v-dialog
      v-model="dialog"
      width="500"
    >
      <v-card>
        <v-card-title class="text-h5">
          <template v-if="!updatingExistingChat">New chat</template>
          <template v-else-if="updatingExistingChat && editingOnlyChatName">Rename</template>
          <template v-else-if="updatingExistingChat && editingOnlyChatUsers">Manage people</template>
          <template v-else>Manage chat</template>
        </v-card-title>

        <v-card-text>
          <v-form ref="chatCreationForm">
            <v-row>
              <v-col cols="12" class="pb-0">
                <v-text-field
                  v-model="chat.name"
                  label="Name"
                  :rules="[formRules.nameRequired]"
                  :disabled="loading"
                  :readonly="editingOnlyChatUsers"
                ></v-text-field>
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" class="pt-0">
                <v-select
                  @focus="onPeopleSelectionFocus"
                  v-model="addedChatUserIds"
                  :items="users"
                  label="People"
                  item-text="fullName"
                  item-value="id"
                  no-data-text="No people found"
                  :disabled="loading"
                  :readonly="editingOnlyChatName"
                  :clearable="!editingOnlyChatName"
                  multiple
                ></v-select>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
          <v-btn
            color="grey"
            text
            @click="closeDialog"
          >
            Cancel
          </v-btn>
          <v-spacer></v-spacer>
          <v-btn
            v-if="!updatingExistingChat"
            color="primary"
            text
            @click="createNewChat"
          >
            Create
          </v-btn>
          <v-btn
            v-if="updatingExistingChat"
            color="primary"
            text
            @click="saveChat"
          >
            Save
          </v-btn>
        </v-card-actions>
      </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import User from "@/models/User";
import AuthStore from "@/store/AuthStore";
import ChatStore from "@/store/ChatStore";
import UserStore from "@/store/UserStore";
import { Guid } from "guid-typescript";
import { Vue, Component } from "vue-property-decorator"

export enum ChatEditOnlyEnum {
  None = 0,
  ChatUsers,
  ChatName
}

@Component({
  name: "ChatCreationDialog"
})
export default class ChatCreationDialog extends Vue {
  dialog: boolean = false;
  chat: Chat = new Chat();
  formRules: any = {
    nameRequired: (v: any) => !!v || 'Required',
  }
  addedChatUserIds: string[] = []
  editOnly: ChatEditOnlyEnum = ChatEditOnlyEnum.None
  loading: boolean = false;


  get me(): User | null {
    return UserStore.me;
  }

  get users(): User[] {
    return UserStore.users;
  }

  get chats(): Chat[] {
    return ChatStore.chats;
  }

  get updatingExistingChat(): boolean {
    return this.chat.id != Guid.createEmpty().toString();
  }

  get editingOnlyChatName(): boolean {
    return this.editOnly !== null && this.editOnly === ChatEditOnlyEnum.ChatName;
  }

  get editingOnlyChatUsers(): boolean {
    return this.editOnly !== null && this.editOnly === ChatEditOnlyEnum.ChatUsers;
  }

  async createNewChat() {
    if (!(this.$refs.chatCreationForm as any).validate()) {
      return
    }

    this.loading = true;
    if (this.me) 
    {
      this.chat.chatUserIds.push(this.me.id);
    }
    try {
      await ChatStore.createOrUpdateChat(this.chat)
    } catch (error) {
      console.error("Error creating a new chat:", error)
    }

    this.loading = false;

    this.chat = new Chat();
    this.closeDialog();
  }

  async onPeopleSelectionFocus() {
    if (this.users.length === 0) {
      console.log("load people")
      await UserStore.getAllUsers();
    }
  }

  async saveChat() {
    if (!(this.$refs.chatCreationForm as any).validate()) {
      return
    }

    this.loading = true;
    try {
      this.chat.chatUserIds = [...this.addedChatUserIds]
      await ChatStore.createOrUpdateChat(this.chat)
    } catch (error) {
      console.error("Error saving chat:", error)
    }
    this.loading = false;

    this.chat = new Chat();
    this.closeDialog();
  }

  openDialog(payload?: {chatId: string, editOnly: ChatEditOnlyEnum} ) {
    if (payload != null) {
      const chat = this.chats.find(c => c.id === payload.chatId);
      this.chat = chat ? Chat.fromApi(chat) : new Chat();
      this.addedChatUserIds = [...this.chat.chatUserIds]

      this.editOnly = payload.editOnly;
    } 

    this.dialog = true;
  }

  closeDialog() {
    (this.$refs.chatCreationForm as any).reset();
    this.chat = new Chat();
    this.dialog = false;
  }

  created() {
    this.$root.$on("openCreateChatDialog", this.openDialog)
  }

  beforeDestroy() {
    this.$root.$off("openCreateChatDialog", this.closeDialog)
  }
}
</script>

<style lang="scss">

.v-input {
  &--is-readonly {
    opacity: 0.3;
  }
}
</style>