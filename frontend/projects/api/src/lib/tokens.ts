import { InjectionToken } from '@angular/core';
import {
  IBackendClient,
  IRoomStateStore,
  ISignalRClient,
  ITelemetryStore,
} from './contracts';

export const BFF_BASE_URL = new InjectionToken<string>('BFF_BASE_URL');

export const BACKEND_CLIENT = new InjectionToken<IBackendClient>('BACKEND_CLIENT');
export const SIGNALR_CLIENT = new InjectionToken<ISignalRClient>('SIGNALR_CLIENT');
export const ROOM_STATE_STORE = new InjectionToken<IRoomStateStore>('ROOM_STATE_STORE');
export const TELEMETRY_STORE = new InjectionToken<ITelemetryStore>('TELEMETRY_STORE');
