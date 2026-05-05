import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { ConnectionState } from './models';

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

export const SIGNALR_CLIENT = new InjectionToken<ISignalRClient>('SIGNALR_CLIENT');
