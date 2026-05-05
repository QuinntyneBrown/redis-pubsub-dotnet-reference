import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'lib-signal-icon-button',
  imports: [MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './signal-icon-button.html',
  styleUrl: './signal-icon-button.scss',
})
export class SignalIconButtonComponent {
  @Input({ required: true }) icon = '';
  @Input() label = 'Action';
  @Input() active = false;
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
