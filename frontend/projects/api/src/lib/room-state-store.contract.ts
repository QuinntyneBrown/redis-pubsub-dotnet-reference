import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { RoomStatus } from './models';

export interface IRoomStateStore {
  readonly rooms$: Observable<RoomStatus[]>;
  seed(): Promise<void>;
}

export const ROOM_STATE_STORE = new InjectionToken<IRoomStateStore>('ROOM_STATE_STORE');
