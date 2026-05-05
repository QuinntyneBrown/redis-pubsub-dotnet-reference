import { Observable } from 'rxjs';
import { ConnectionState, Point, RoomStatus } from './models';

export interface IBackendClient {
  command<T>(type: string, payload: object): Promise<T>;
  query<T>(type: string, payload: object): Promise<T>;
  request<T>(type: string, payload: object): Promise<T>;
}

export interface BusMessage {
  channel: string;
  envelope: { payload: unknown };
}

export interface ISignalRClient {
  start(): Promise<void>;
  subscribe(channel: string): Promise<void>;
  unsubscribe(channel: string): Promise<void>;
  unsubscribeAll(): Promise<void>;
  send<T>(typeName: string, payload: object): Promise<T>;
  readonly messages$: Observable<BusMessage>;
  readonly connectionState$: Observable<ConnectionState>;
}

export interface IRoomStateStore {
  readonly rooms$: Observable<RoomStatus[]>;
  seed(): Promise<void>;
}

export interface ITelemetryStore {
  readonly temperatureSeries$: Observable<Map<string, Point[]>>;
}
