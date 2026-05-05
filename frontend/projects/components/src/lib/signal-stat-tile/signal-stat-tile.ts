import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

import { SignalTone, SignalTrend } from '../models';

@Component({
  selector: 'lib-signal-stat-tile',
  imports: [MatCardModule, MatIconModule],
  templateUrl: './signal-stat-tile.html',
  styleUrl: './signal-stat-tile.scss',
})
export class SignalStatTileComponent {
  @Input() label = '';
  @Input() value = '';
  @Input() delta = '';
  @Input() trend: SignalTrend = 'flat';
  @Input() tone: SignalTone = 'success';

  protected get trendIcon(): string {
    if (this.trend === 'up') {
      return 'trending_up';
    }

    if (this.trend === 'down') {
      return 'trending_down';
    }

    return 'remove';
  }
}
