import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

import { ConnectionIndicatorComponent } from './connection-indicator/connection-indicator';
import { HubFilterOption, SignalMessage } from './models';
import { SignalAvatarComponent } from './signal-avatar/signal-avatar';
import { SignalBadgeComponent } from './signal-badge/signal-badge';
import { SignalButtonComponent } from './signal-button/signal-button';
import { SignalCardComponent } from './signal-card/signal-card';
import { SignalConfirmDialogComponent } from './signal-confirm-dialog/signal-confirm-dialog';
import { SignalHubFilterComponent } from './signal-hub-filter/signal-hub-filter';
import { SignalIconButtonComponent } from './signal-icon-button/signal-icon-button';
import { SignalInputComponent } from './signal-input/signal-input';
import { SignalMessageRowComponent } from './signal-message-row/signal-message-row';
import { SignalNavItemComponent } from './signal-nav-item/signal-nav-item';
import { SignalStatTileComponent } from './signal-stat-tile/signal-stat-tile';
import { SignalToastComponent } from './signal-toast/signal-toast';
import { SignalToggleComponent } from './signal-toggle/signal-toggle';

@Component({
  imports: [
    ConnectionIndicatorComponent,
    SignalAvatarComponent,
    SignalBadgeComponent,
    SignalButtonComponent,
    SignalCardComponent,
    SignalConfirmDialogComponent,
    SignalHubFilterComponent,
    SignalIconButtonComponent,
    SignalInputComponent,
    SignalMessageRowComponent,
    SignalNavItemComponent,
    SignalStatTileComponent,
    SignalToastComponent,
    SignalToggleComponent,
  ],
  template: `
    <lib-signal-button label="Dispatch"></lib-signal-button>
    <lib-signal-icon-button icon="refresh" label="Refresh"></lib-signal-icon-button>
    <lib-signal-input label="Command" value="thermostat.set"></lib-signal-input>
    <lib-signal-badge label="Online" tone="success"></lib-signal-badge>
    <lib-signal-card title="Telemetry" meta="24 events" icon="sensors">Card body</lib-signal-card>
    <lib-signal-nav-item label="Live Stream" icon="radio" [active]="true"></lib-signal-nav-item>
    <lib-signal-stat-tile
      label="Msgs / sec"
      value="1,284"
      delta="+12.4%"
      trend="up"
    ></lib-signal-stat-tile>
    <lib-signal-message-row [message]="message"></lib-signal-message-row>
    <lib-connection-indicator state="connected" latency="14ms"></lib-connection-indicator>
    <lib-signal-toggle [checked]="true" label="Stream"></lib-signal-toggle>
    <lib-signal-avatar initials="QB"></lib-signal-avatar>
    <lib-signal-toast title="Command dispatched" message="acknowledged in 12ms"></lib-signal-toast>
    <lib-signal-hub-filter [options]="options" selectedId="all"></lib-signal-hub-filter>
    <lib-signal-confirm-dialog></lib-signal-confirm-dialog>
  `,
})
class HostComponent {
  readonly options: readonly HubFilterOption[] = [
    { id: 'all', label: 'All hubs', tone: 'info' },
    { id: 'telemetry', label: 'telemetry', rate: '482 / s', tone: 'success' },
  ];

  readonly message: SignalMessage = {
    timestamp: 'T-00:14:32.481',
    channel: 'telemetry',
    type: 'event',
    title: 'TemperatureChanged',
    payload: { roomId: 'room-101', value: 22.5 },
  };
}

describe('SignalStream reusable components', () => {
  it('renders the design-system components together', async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent],
      providers: [provideNoopAnimations()],
    }).compileComponents();

    const fixture = TestBed.createComponent(HostComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Dispatch');
    expect(fixture.nativeElement.textContent).toContain('TemperatureChanged');
    expect(fixture.nativeElement.textContent).toContain('SIGNALR://CONNECTED');
  });
});
