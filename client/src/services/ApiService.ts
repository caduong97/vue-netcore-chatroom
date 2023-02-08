import { firebaseAuthPluginInstance } from "@/plugins/firebaseAuthPlugin";
import { msalPluginInstance } from "@/plugins/msalPlugin";
import AuthStore from "@/store/AuthStore";
import axios, { AxiosResponse } from "axios";

export class ApiService {
  static baseURL: string = window.location.origin + "/api";

  public static ConfigureInterceptors() {
    // Add a request interceptor
    axios.interceptors.request.use(async function (config: any) {

      let accessToken: string = "";
      // TODO: try to acquire token
      if (AuthStore.msalAuthenticated) {
        const msalToken = await msalPluginInstance.acquireToken();
        accessToken = msalToken ? msalToken : accessToken;
      }
      if (AuthStore.googleAuthenticated) {
        const firebaseToken = await firebaseAuthPluginInstance.getUserIdToken();
        accessToken = firebaseToken ? firebaseToken : accessToken;
      }

      console.log("axios.interceptors.request access token", accessToken);

      (config as any).headers["Authorization"] =  "Bearer " + accessToken

      return config;
    }, function (error: any) {
      // Do something with request error
      console.log("axios.interceptors.request error", error)
      return Promise.reject(error);
    });

    // Add a response interceptor
    axios.interceptors.response.use(function (response: any) {
      // Any status code that lie within the range of 2xx cause this function to trigger
      // Do something with response data
      return response;
    }, function (error: any) {
      // Any status codes that falls outside the range of 2xx cause this function to trigger
      // Do something with response error
      return Promise.reject(error);
    });
  }

  public static async get(path: string) {
    return await axios.get(this.baseURL + path, { withCredentials: true })
  }

  public static async post<T>(path: string, data: any): Promise<AxiosResponse<T>> {
    return await axios.post(this.baseURL + path, data, { 
        withCredentials: true, 
        headers: { 'Content-Type': 'application/json' }
      }
    )
  }
}