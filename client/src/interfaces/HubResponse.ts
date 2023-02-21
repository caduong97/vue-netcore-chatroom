import { Guid } from "guid-typescript";

export interface HubResponse<T> {
  id: Guid;
  data: T;
}