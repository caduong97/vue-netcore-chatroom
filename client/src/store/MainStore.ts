import { Module, VuexModule, Mutation, Action, getModule } from "vuex-module-decorators";
import store from ".";

export interface IMainStoreState {
  appHidden: boolean
}

// Hack due to issues
// https://github.com/championswimmer/vuex-module-decorators/issues/131
// https://github.com/championswimmer/vuex-module-decorators/issues/189
const name = 'main';
if (store && store.state[name]) {
  try{
    store.unregisterModule(name);
  } catch (e){
    console.warn("Unregister store module workaround error, ignoring ...")
  }
}


@Module({ dynamic: true, store: store, name: 'main' })
export class MainStoreModule extends VuexModule implements IMainStoreState {
  appHidden: boolean = false;

  @Action
  toggleAppHidden(appHidden: boolean) {
    this.TOGGLE_APP_HIDDEN(appHidden)
  }

  @Mutation
  TOGGLE_APP_HIDDEN(payload: boolean) {
    this.appHidden = payload;
  }
}

const MainStore = getModule(MainStoreModule);
export default MainStore;