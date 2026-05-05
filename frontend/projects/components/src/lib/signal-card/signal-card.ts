import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'lib-signal-card',
  imports: [MatCardModule, MatIconModule],
  templateUrl: './signal-card.html',
  styleUrl: './signal-card.scss',
})
export class SignalCardComponent {
  @Input() title = '';
  @Input() meta = '';
  @Input() icon = '';
}
