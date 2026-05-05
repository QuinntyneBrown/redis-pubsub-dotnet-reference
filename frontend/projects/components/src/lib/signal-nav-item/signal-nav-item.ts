import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'lib-signal-nav-item',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './signal-nav-item.html',
  styleUrl: './signal-nav-item.scss',
})
export class SignalNavItemComponent {
  @Input() label = '';
  @Input() icon = '';
  @Input() active = false;
  @Input() disabled = false;

  @Output() readonly selected = new EventEmitter<MouseEvent>();

  protected onClick(event: MouseEvent): void {
    if (this.disabled) {
      event.preventDefault();
      return;
    }

    this.selected.emit(event);
  }
}
