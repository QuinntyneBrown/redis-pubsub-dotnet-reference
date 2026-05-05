import { Component, Input } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';

import { SignalTone } from '../models';

@Component({
  selector: 'lib-signal-badge',
  imports: [MatChipsModule],
  templateUrl: './signal-badge.html',
  styleUrl: './signal-badge.scss',
})
export class SignalBadgeComponent {
  @Input() label = 'Online';
  @Input() tone: SignalTone = 'success';
  @Input() showDot = true;
}
