import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { Point } from './models';

export interface ITelemetryStore {
  readonly temperatureSeries$: Observable<Map<string, Point[]>>;
}

export const TELEMETRY_STORE = new InjectionToken<ITelemetryStore>('TELEMETRY_STORE');
