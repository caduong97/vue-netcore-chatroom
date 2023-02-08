import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import routes from "@/router/Routes";

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  routes
})

export default router
