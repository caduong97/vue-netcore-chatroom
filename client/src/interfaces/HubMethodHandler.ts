import { HubResponse } from "./HubResponse";

export default interface HubMethodHandler {
  name: string;
  handler: (data: HubResponse<any>) => void;
}