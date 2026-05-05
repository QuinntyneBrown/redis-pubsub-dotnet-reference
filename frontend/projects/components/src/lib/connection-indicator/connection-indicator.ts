import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

import { SignalConnectionState } from '../models';

@Component({
  selector: 'lib-connection-indicator',
  imports: [MatIconModule],
  templateUrl: './connection-indicator.html',
  styleUrl: './connection-indicator.scss',
})
export class ConnectionIndicatorComponent {
  @Input() state: SignalConnectionState = 'connected';
  @Input() label = '';
  @Input() latency = '';

  protected get resolvedLabel(): string {
    if (this.label) {
      return this.label;
    }

    if (this.state === 'connected') {
      return 'SIGNALR://CONNECTED';
    }

    if (this.state === 'reconnecting') {
      return 'RECONNECTING';
    }

    if (this.state === 'paused') {
      return 'STREAM PAUSED';
    }

    return 'OFFLINE';
  }

  protected get icon(): string {
    if (this.state === 'reconnecting') {
      return 'progress_activity';
    }

    if (this.state === 'paused') {
      return 'pause';
    }

    return '';
  }
}
