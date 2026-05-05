import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

@Component({
  selector: 'lib-signal-toggle',
  imports: [MatSlideToggleModule],
  templateUrl: './signal-toggle.html',
  styleUrl: './signal-toggle.scss',
})
export class SignalToggleComponent {
  @Input() checked = false;
  @Input() disabled = false;
  @Input() label = '';

  @Output() readonly checkedChange = new EventEmitter<boolean>();
}
