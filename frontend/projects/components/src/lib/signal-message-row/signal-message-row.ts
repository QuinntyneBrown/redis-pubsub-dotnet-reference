import { Component, Input } from '@angular/core';

import { SignalMessage, SignalMessageType } from '../models';

@Component({
  selector: 'lib-signal-message-row',
  imports: [],
  templateUrl: './signal-message-row.html',
  styleUrl: './signal-message-row.scss',
})
export class SignalMessageRowComponent {
  @Input() message: SignalMessage | null = null;
  @Input() timestamp = '';
  @Input() channel = '';
  @Input() type: SignalMessageType = 'event';
  @Input() title = '';
  @Input() payload: SignalMessage['payload'] = '';

  protected get resolvedTimestamp(): string {
    return this.message?.timestamp ?? this.timestamp;
  }

  protected get resolvedChannel(): string {
    return this.message?.channel ?? this.channel;
  }

  protected get resolvedType(): SignalMessageType {
    return this.message?.type ?? this.type;
  }

  protected get resolvedTitle(): string {
    return this.message?.title ?? this.title;
  }

  protected get formattedPayload(): string {
    const payload = this.message?.payload ?? this.payload;

    if (typeof payload === 'string') {
      return payload;
    }

    try {
      return JSON.stringify(payload, null, 2);
    } catch {
      return String(payload);
    }
  }
}
