import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { SignalTone } from '../models';

@Component({
  selector: 'lib-signal-toast',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './signal-toast.html',
  styleUrl: './signal-toast.scss',
})
export class SignalToastComponent {
  @Input() title = '';
  @Input() message = '';
  @Input() variant: SignalTone = 'success';
  @Input() icon = '';
  @Input() actionLabel = '';
  @Input() dismissLabel = 'Dismiss';

  @Output() readonly action = new EventEmitter<void>();
  @Output() readonly dismissed = new EventEmitter<void>();

  protected get resolvedIcon(): string {
    if (this.icon) {
      return this.icon;
    }

    if (this.variant === 'warning') {
      return 'warning';
    }

    if (this.variant === 'error') {
      return 'error';
    }

    if (this.variant === 'info') {
      return 'info';
    }

    return 'check_circle';
  }
}
