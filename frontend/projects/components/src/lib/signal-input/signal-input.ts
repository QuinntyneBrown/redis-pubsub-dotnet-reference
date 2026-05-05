import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

let nextInputId = 0;

@Component({
  selector: 'lib-signal-input',
  imports: [MatFormFieldModule, MatIconModule, MatInputModule],
  templateUrl: './signal-input.html',
  styleUrl: './signal-input.scss',
})
export class SignalInputComponent {
  @Input() inputId = `signal-input-${nextInputId++}`;
  @Input() label = 'Command';
  @Input() value = '';
  @Input() placeholder = '';
  @Input() hint = '';
  @Input() error = '';
  @Input() icon = '';
  @Input() type: 'email' | 'number' | 'password' | 'search' | 'text' | 'url' = 'text';
  @Input() disabled = false;
  @Input() readonly = false;
  @Input() autocomplete = 'off';
  @Input() mono = true;

  @Output() readonly valueChange = new EventEmitter<string>();
  @Output() readonly focused = new EventEmitter<FocusEvent>();
  @Output() readonly blurred = new EventEmitter<FocusEvent>();

  protected onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.value = value;
    this.valueChange.emit(value);
  }
}
