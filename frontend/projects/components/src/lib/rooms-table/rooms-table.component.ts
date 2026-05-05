import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';

export interface RoomRow {
  roomId: string;
  name: string;
  temperatureC: number | null;
  occupied: boolean;
  lastUpdated: string | null;
}

@Component({
  selector: 'lib-rooms-table',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatChipsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <table mat-table [dataSource]="rooms" class="mat-elevation-z2">
      <ng-container matColumnDef="roomId">
        <th mat-header-cell *matHeaderCellDef>Room</th>
        <td mat-cell *matCellDef="let r">{{ r.roomId }}</td>
      </ng-container>
      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef>Name</th>
        <td mat-cell *matCellDef="let r">{{ r.name }}</td>
      </ng-container>
      <ng-container matColumnDef="temperatureC">
        <th mat-header-cell *matHeaderCellDef>Temperature (°C)</th>
        <td mat-cell *matCellDef="let r">{{ r.temperatureC ?? '—' }}</td>
      </ng-container>
      <ng-container matColumnDef="occupied">
        <th mat-header-cell *matHeaderCellDef>Occupied</th>
        <td mat-cell *matCellDef="let r">
          <mat-chip [class]="r.occupied ? 'occupied' : 'empty'">
            {{ r.occupied ? 'Yes' : 'No' }}
          </mat-chip>
        </td>
      </ng-container>
      <ng-container matColumnDef="lastUpdated">
        <th mat-header-cell *matHeaderCellDef>Last Updated</th>
        <td mat-cell *matCellDef="let r">{{ r.lastUpdated | date: 'short' }}</td>
      </ng-container>
      <tr mat-header-row *matHeaderRowDef="columns"></tr>
      <tr mat-row *matRowDef="let r; columns: columns"></tr>
    </table>
  `,
  styles: [
    `
      :host {
        display: block;
      }
      table {
        width: 100%;
      }
      .occupied {
        background: #22c55e;
        color: #000;
      }
      .empty {
        background: #52525b;
        color: #fff;
      }
    `,
  ],
})
export class RoomsTableComponent {
  @Input() rooms: RoomRow[] = [];
  readonly columns = ['roomId', 'name', 'temperatureC', 'occupied', 'lastUpdated'];
}
