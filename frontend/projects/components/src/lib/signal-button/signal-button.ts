import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { SignalButtonVariant } from '../models';

@Component({
  selector: 'lib-signal-button',
  imports: [MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './signal-button.html',
  styleUrl: './signal-button.scss',
})
export class SignalButtonComponent {
  @Input() variant: SignalButtonVariant = 'primary';
  @Input() label = '';
  @Input() icon = '';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() loading = false;
  @Input() disabled = false;

  @Output() readonly pressed = new EventEmitter<MouseEvent>();

  protected onClick(event: MouseEvent): void {
    if (this.disabled || this.loading) {
      event.preventDefault();
      event.stopPropagation();
      return;
    }

    this.pressed.emit(event);
  }
}
