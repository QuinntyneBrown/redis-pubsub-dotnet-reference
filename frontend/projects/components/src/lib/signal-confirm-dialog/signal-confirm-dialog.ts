import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

import { SignalConfirmDialogData } from '../models';
import { SignalButtonComponent } from '../signal-button/signal-button';

@Component({
  selector: 'lib-signal-confirm-dialog',
  imports: [MatButtonModule, MatDialogModule, MatIconModule, SignalButtonComponent],
  templateUrl: './signal-confirm-dialog.html',
  styleUrl: './signal-confirm-dialog.scss',
})
export class SignalConfirmDialogComponent {
  private readonly dialogRef = inject<MatDialogRef<SignalConfirmDialogComponent, boolean> | null>(
    MatDialogRef,
    { optional: true },
  );

  private readonly data = inject<SignalConfirmDialogData | null>(MAT_DIALOG_DATA, { optional: true });

  @Input() title = this.data?.title ?? 'Abort pending request?';
  @Input() message =
    this.data?.message ?? 'This will cancel the command and leave the latest building state intact.';
  @Input() details = this.data?.details ?? '';
  @Input() confirmLabel = this.data?.confirmLabel ?? 'Abort request';
  @Input() cancelLabel = this.data?.cancelLabel ?? 'Cancel';
  @Input() destructive = this.data?.destructive ?? true;

  @Output() readonly confirmed = new EventEmitter<void>();
  @Output() readonly cancelled = new EventEmitter<void>();

  protected onConfirm(): void {
    this.confirmed.emit();
    this.dialogRef?.close(true);
  }

  protected onCancel(): void {
    this.cancelled.emit();
    this.dialogRef?.close(false);
  }
}
