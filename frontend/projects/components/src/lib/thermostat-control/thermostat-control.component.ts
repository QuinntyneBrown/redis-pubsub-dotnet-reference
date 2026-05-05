import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';

export interface RoomOption {
  roomId: string;
  name: string;
}

@Component({
  selector: 'lib-thermostat-control',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatFormFieldModule, MatSelectModule, MatSliderModule, MatButtonModule,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <mat-card class="thermostat-card">
      <mat-card-title>Thermostat</mat-card-title>
      <mat-card-content>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Room</mat-label>
          <mat-select [(ngModel)]="selectedRoomId">
            @for (r of rooms; track r.roomId) {
              <mat-option [value]="r.roomId">{{ r.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <label>Target: {{ targetC }} °C</label>
        <mat-slider min="15" max="28" step="0.5">
          <input matSliderThumb [(ngModel)]="targetC" />
        </mat-slider>
      </mat-card-content>
      <mat-card-actions>
        <button mat-flat-button color="primary"
                [disabled]="!selectedRoomId"
                (click)="onSubmit()">
          Apply
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [
    `
      :host { display: block; max-width: 360px; }
      .full-width { width: 100%; }
      mat-slider { width: 100%; }
    `,
  ],
})
export class ThermostatControlComponent {
  @Input() rooms: RoomOption[] = [];
  @Output() submit = new EventEmitter<{ roomId: string; targetC: number }>();

  selectedRoomId = '';
  targetC = 21;

  onSubmit(): void {
    if (!this.selectedRoomId) return;
    this.submit.emit({ roomId: this.selectedRoomId, targetC: this.targetC });
  }
}
