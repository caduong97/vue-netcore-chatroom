<template>
  <v-menu 
    v-model="menu" 
    offset-x 
    :nudge-top="nudges[0]"
    :nudge-right="nudges[1]" 
    :nudge-bottom="nudges[2]" 
    :nudge-left="nudges[3]"  
  >
    <template v-slot:activator="{ on, attrs }">
      <slot :attrs="attrs" :on="on" name="moreActionCustomActivator">
        <v-btn icon small v-bind="attrs" v-on="on" v-show="hovered || menu">
          <v-icon>mdi-dots-horizontal</v-icon>
        </v-btn>
      </slot>
    </template>

    <v-list dense class="pa-0">
      <v-list-item
        v-for="item in shownItems"
        :key="item.id"
        @click="onMoreActionItemClick(item.id)"
      >
        <v-list-item-title>{{ item.text }}</v-list-item-title>
      </v-list-item>
    </v-list>
  </v-menu>
</template>

<script lang="ts">
import { Vue, Component, Watch, Prop } from "vue-property-decorator";

export interface IMoreActionItem {
  id: string;
  text: string;
  disabled: boolean;
  hidden?: boolean;
  dangerous: boolean;
  cssClasses?: string;
}

@Component({
  name: "MoreAction"
})
export default class MoreAction extends Vue {
  menu: boolean = false; 
  
  @Prop({required: true})
  items!: IMoreActionItem[]

  @Prop({ required: false })
  activatorHint!: string;

  @Prop({ required: false, default: [0,0,0,0]})
  nudges!: number[]

  @Prop({ required: true, default: false})
  hovered!: boolean

  @Prop({required: false})
  target!: any

  get shownItems(): IMoreActionItem[] {
    return this.items.filter((i) => !i.hidden)
  }

  @Watch("menu")
  onMenuChange(newVal: boolean, oldVal: boolean) {
    if (newVal !== oldVal) {
      this.$emit("moreActionToggled", newVal)
    }
  }

  onMoreActionItemClick(moreActionItemId: string) {
    this.$emit("moreActionItemClicked", { moreActionItemId: moreActionItemId, moreActionItemTarget: this.target } )
  }

}
</script>

<style>

</style>