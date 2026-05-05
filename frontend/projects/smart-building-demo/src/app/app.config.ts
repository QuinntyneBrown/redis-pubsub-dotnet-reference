import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {
  BACKEND_CLIENT, BackendClient,
  SIGNALR_CLIENT, SignalRClient,
  ROOM_STATE_STORE, RoomStateStore,
  TELEMETRY_STORE, TelemetryStore,
  BFF_BASE_URL,
} from 'api';

import { routes } from './app.routes';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    provideAnimationsAsync(),
    { provide: BFF_BASE_URL, useValue: environment.bffBaseUrl },
    { provide: BACKEND_CLIENT, useClass: BackendClient },
    { provide: SIGNALR_CLIENT, useClass: SignalRClient },
    { provide: ROOM_STATE_STORE, useClass: RoomStateStore },
    { provide: TELEMETRY_STORE, useClass: TelemetryStore },
  ],
};
