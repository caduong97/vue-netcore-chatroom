import { NavigationGuard, NavigationGuardNext, Route, RouteConfig } from "vue-router";
import SignIn from "@/views/SignIn.vue";
import SignUp from "@/views/SignUp.vue";
import Home from '@/views/Home.vue';
import AuthStore from "@/store/AuthStore";
import Profile from "@/views/Profile.vue";
export class Routes {

  public static SignedInNavGuard: NavigationGuard<Vue> = (to: Route, from: Route, next: NavigationGuardNext) => {
      console.log('signed in auth guard')
      if (!AuthStore.isAuthenticated) {
        next({ name: Routes.SignIn.name });
      } else {
        next();
      }
  }

  public static SignIn: RouteConfig = {
    path: "/signIn",
    name: "Sign In",
    component: SignIn
  }

  public static SignUp: RouteConfig = {
    path: "/signUp",
    name: "Sign Up",
    component: SignUp
  }

  public static MyProfile: RouteConfig = {
    path: "my-profile",
    name: "Profile",
    component: Profile
  }
    
  public static Home: RouteConfig = {
    path: "/",
    name: "Home",
    beforeEnter: Routes.SignedInNavGuard,
    component: Home,
    children: [
      Routes.MyProfile
    ]
  }


  
}

const routes: RouteConfig[] = [
  Routes.Home,
  Routes.SignIn,
  Routes.SignUp
];

export default routes;