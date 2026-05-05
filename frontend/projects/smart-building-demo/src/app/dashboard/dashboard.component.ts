import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import {
  AppShellComponent,
  TemperatureChartComponent,
  RoomsTableComponent,
  ThermostatControlComponent,
} from 'components';
import {
  BACKEND_CLIENT,
  ConnectionState,
  Point,
  ROOM_STATE_STORE,
  RoomStatus,
  SIGNALR_CLIENT,
  SetThermostatAck,
  TELEMETRY_STORE,
} from 'api';

const SUBSCRIPTIONS = [
  'tel.Contracts.TemperatureReading',
  'tel.Contracts.OccupancyReading',
  'evt.Contracts.RoomOccupancyChanged',
  'evt.Contracts.ThermostatChanged',
];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    AppShellComponent,
    TemperatureChartComponent,
    RoomsTableComponent,
    ThermostatControlComponent,
  ],
  template: `
    <lib-app-shell
      title="Smart Building"
      [connectionState]="(connectionState$ | async) ?? 'disconnected'">
      <section class="panel">
        <h2>Temperature</h2>
        <lib-temperature-chart [series]="(series$ | async) ?? emptySeries"></lib-temperature-chart>
      </section>

      <section class="panel">
        <h2>Rooms</h2>
        <lib-rooms-table [rooms]="(rooms$ | async) ?? []"></lib-rooms-table>
      </section>

      <section class="panel">
        <lib-thermostat-control
          [rooms]="(rooms$ | async) ?? []"
          (submit)="onSetThermostat($event)">
        </lib-thermostat-control>
      </section>
    </lib-app-shell>
  `,
  styles: [
    `
      .panel { margin-bottom: 24px; }
      h2 { margin: 0 0 8px; font-size: 1rem; font-weight: 500; }
    `,
  ],
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly backend = inject(BACKEND_CLIENT);
  private readonly signalR = inject(SIGNALR_CLIENT);
  private readonly roomStore = inject(ROOM_STATE_STORE);
  private readonly telemetry = inject(TELEMETRY_STORE);
  private readonly snackBar = inject(MatSnackBar);

  readonly emptySeries = new Map<string, Point[]>();

  readonly rooms$: Observable<RoomStatus[]> = this.roomStore.rooms$;
  readonly series$: Observable<Map<string, Point[]>> = this.telemetry.temperatureSeries$;
  readonly connectionState$: Observable<ConnectionState> = this.signalR.connectionState$;

  async ngOnInit(): Promise<void> {
    await this.signalR.start();
    for (const channel of SUBSCRIPTIONS) {
      await this.signalR.subscribe(channel);
    }
    await this.roomStore.seed();
  }

  async ngOnDestroy(): Promise<void> {
    await this.signalR.unsubscribeAll();
  }

  async onSetThermostat(e: { roomId: string; targetC: number }): Promise<void> {
    try {
      const ack = await this.backend.request<SetThermostatAck>(
        'Contracts.Requests.SetThermostat',
        e,
      );
      this.snackBar.open(`Set ${ack.roomId} to ${ack.effectiveC} °C`, 'OK', { duration: 3000 });
    } catch {
      this.snackBar.open('Failed to apply thermostat', 'Dismiss', { duration: 4000 });
    }
  }
}
