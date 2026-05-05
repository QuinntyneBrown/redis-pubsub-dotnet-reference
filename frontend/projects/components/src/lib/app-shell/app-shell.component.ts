import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatChipsModule } from '@angular/material/chips';

export type ConnectionState = 'connected' | 'reconnecting' | 'disconnected';

@Component({
  selector: 'lib-app-shell',
  standalone: true,
  imports: [MatToolbarModule, MatChipsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <mat-toolbar color="primary" class="shell-toolbar">
      <span class="title">{{ title }}</span>
      <span class="spacer"></span>
      <mat-chip [class]="'state-' + connectionState">
        {{ stateLabel() }}
      </mat-chip>
    </mat-toolbar>
    <main class="shell-body">
      <ng-content></ng-content>
    </main>
  `,
  styles: [
    `
      .shell-toolbar { display: flex; align-items: center; }
      .spacer { flex: 1 1 auto; }
      .title { font-weight: 500; }
      .shell-body { padding: 24px; }
      .state-connected { background: #22c55e; color: #000; }
      .state-reconnecting { background: #f59e0b; color: #000; }
      .state-disconnected { background: #ef4444; color: #fff; }
    `,
  ],
})
export class AppShellComponent {
  @Input() title = 'Smart Building';
  @Input() connectionState: ConnectionState = 'disconnected';

  stateLabel(): string {
    switch (this.connectionState) {
      case 'connected': return 'Live';
      case 'reconnecting': return 'Reconnecting…';
      case 'disconnected': return 'Offline';
    }
  }
}
