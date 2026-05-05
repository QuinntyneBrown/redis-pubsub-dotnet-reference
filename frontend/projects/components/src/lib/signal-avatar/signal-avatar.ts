import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'lib-signal-avatar',
  imports: [MatIconModule],
  templateUrl: './signal-avatar.html',
  styleUrl: './signal-avatar.scss',
})
export class SignalAvatarComponent {
  @Input() initials = '';
  @Input() icon = '';
  @Input() label = 'User';
  @Input() size: 'sm' | 'md' = 'md';
}
