import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, filter } from 'rxjs';
import { IBackendClient } from './backend-client.contract';
import { RoomOccupancyChanged, RoomStatus, ThermostatChanged } from './models';
import { BACKEND_CLIENT } from './backend-client.contract';
import { IRoomStateStore } from './room-state-store.contract';
import { ISignalRClient, SIGNALR_CLIENT } from './signalr-client.contract';

@Injectable({ providedIn: 'root' })
export class RoomStateStore implements IRoomStateStore {
  private readonly backend: IBackendClient = inject(BACKEND_CLIENT);
  private readonly signalR: ISignalRClient = inject(SIGNALR_CLIENT);

  private readonly _rooms = new BehaviorSubject<RoomStatus[]>([]);
  readonly rooms$: Observable<RoomStatus[]> = this._rooms.asObservable();

  constructor() {
    this.signalR.messages$
      .pipe(filter((m) => m.channel.startsWith('evt.')))
      .subscribe((m) => this.applyEvent(m.channel, m.envelope.payload));
  }

  async seed(): Promise<void> {
    interface RoomSummary {
      roomId: string;
      name: string;
      occupied: boolean;
      lastTemperatureC: number | null;
      lastUpdated: string | null;
    }
    const result = await this.backend.query<{ rooms: RoomSummary[] }>(
      'Contracts.Queries.ListRooms',
      {},
    );
    const rooms: RoomStatus[] = (result.rooms ?? []).map((s) => ({
      roomId: s.roomId,
      name: s.name,
      temperatureC: s.lastTemperatureC,
      occupied: s.occupied,
      lastUpdated: s.lastUpdated,
      devices: [],
    }));
    this._rooms.next(rooms);
  }

  private applyEvent(channel: string, payload: unknown): void {
    if (channel.endsWith('.RoomOccupancyChanged')) {
      const e = payload as RoomOccupancyChanged;
      this.update(e.roomId, (r) => ({ ...r, occupied: e.occupied, lastUpdated: e.at }));
    } else if (channel.endsWith('.ThermostatChanged')) {
      const e = payload as ThermostatChanged;
      this.update(e.roomId, (r) => ({ ...r, temperatureC: e.effectiveC, lastUpdated: e.at }));
    }
  }

  private update(roomId: string, mutate: (r: RoomStatus) => RoomStatus): void {
    const next = this._rooms.value.map((r) => (r.roomId === roomId ? mutate(r) : r));
    this._rooms.next(next);
  }
}
