<template>
  <div 
    class="message-item" 
    :class="{
      'message-item__incoming': message.incoming,
      'message-item__outgoing': message.outgoing,
      'message-item__pending': message.pending,
      'message-item__failed': message.failedToSave
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
        v-if="message.sent && showSentAtDate"
        class="text-caption"
        :class="{
          'ml-auto': message.outgoing,
          'mr-auto': message.incoming
        }" 
        style="width: max-content;"
      >
        {{ message.sentDateFormatted }}
        <v-icon size="15">mdi-check</v-icon>
      </div>

    </v-expand-transition>

    <div  
      v-if="message.failedToSave"
      class="text-caption ml-auto" 
      style="width: max-content; color: #D50000;"
    >
      Failed to send. 
      <span class="text-decoration-underline  message-item__retry" @click="onRetryClick">Retry</span>
    </div>
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

  onRetryClick() {
    this.$emit("messageRetry")
  }
}
</script>

<style lang="scss">
.message-item {
  
  &__outgoing {
    margin-left: auto;
  }
  &__incoming {
    margin-right: auto;
  }
  &__pending {
    opacity: 0.6;
  }
  &__outgoing,
  &__incoming,
  &__pending {
    .v-sheet {
      cursor: pointer;
    }
  }

  &__failed {
    .v-sheet {
      pointer-events: none;
      color: #D50000;
      opacity: 0.75;
      border: 1px solid #D50000;
    }
  }

  &__retry {
    cursor: pointer;
  }
}
</style>