<template>
  <div 
    class="message-item" 
    :class="{
      'message-item__incoming': message.incoming,
      'message-item__outgoing': message.outgoing
    }"
  >
    <v-sheet
      :min-height="50" 
      :min-width="80"
      width="max-content"
      elevation="2"
      rounded
      :color="message.incoming ? '#82BFFF' : '#fff'"
      class="d-flex align-center pa-3 mb-1"
      :class="{
        'ml-auto': message.outgoing,
        'mr-auto': message.incoming
      }"
      @click="showSentAtDate = !showSentAtDate"
    >
      {{message.text}}
    </v-sheet>
    <v-expand-transition>
      <div  
        v-if="showSentAtDate"
        class="text-caption ml-auto" 
        style="width: auto;"
      >
        {{ message.sentDateFormatted }}
        <v-icon size="15">mdi-check</v-icon>
      </div>

    </v-expand-transition>
  </div>
  
</template>

<script lang="ts">
import Message from "@/models/Message";
import { Vue, Component, Prop } from "vue-property-decorator"

@Component({
  name: "MessageItem"
})
export default class MessageItem extends Vue {
  @Prop({ required: true })
  message!: Message;

  showSentAtDate: boolean = false;
}
</script>

<style lang="scss">
.message-item {
  cursor: pointer;
  &__outgoing {
    margin-left: auto;
  }
  &__incoming {
    margin-right: auto;
  }
}
</style>