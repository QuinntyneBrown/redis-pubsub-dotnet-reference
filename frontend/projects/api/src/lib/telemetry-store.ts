import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, filter } from 'rxjs';
import { ISignalRClient, ITelemetryStore } from './contracts';
import { Point, TemperatureReading } from './models';
import { SIGNALR_CLIENT } from './tokens';

const WINDOW_MS = 5 * 60 * 1000;

@Injectable({ providedIn: 'root' })
export class TelemetryStore implements ITelemetryStore {
  private readonly signalR: ISignalRClient = inject(SIGNALR_CLIENT);

  private readonly _series = new BehaviorSubject<Map<string, Point[]>>(new Map());
  readonly temperatureSeries$: Observable<Map<string, Point[]>> = this._series.asObservable();

  constructor() {
    this.signalR.messages$
      .pipe(filter((m) => m.channel === 'tel.Contracts.TemperatureReading'))
      .subscribe((m) => this.append(m.envelope.payload as TemperatureReading));
  }

  private append(reading: TemperatureReading): void {
    const t = new Date(reading.at).getTime();
    const cutoff = Date.now() - WINDOW_MS;
    const next = new Map(this._series.value);
    const existing = next.get(reading.roomId) ?? [];
    const trimmed = existing.filter((p) => p.t >= cutoff);
    trimmed.push({ t, c: reading.celsius });
    next.set(reading.roomId, trimmed);
    this._series.next(next);
  }
}
