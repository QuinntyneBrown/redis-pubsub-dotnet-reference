import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';

import { HubFilterOption } from '../models';

@Component({
  selector: 'lib-signal-hub-filter',
  imports: [MatButtonModule, MatIconModule, MatMenuModule],
  templateUrl: './signal-hub-filter.html',
  styleUrl: './signal-hub-filter.scss',
})
export class SignalHubFilterComponent {
  @Input() options: readonly HubFilterOption[] = [];
  @Input() selectedId = '';
  @Input() label = 'All hubs';
  @Input() searchPlaceholder = 'Filter hubs';

  @Output() readonly selectedIdChange = new EventEmitter<string>();
  @Output() readonly optionSelected = new EventEmitter<HubFilterOption>();

  protected query = '';

  protected get selectedOption(): HubFilterOption | undefined {
    return this.options.find((option) => option.id === this.selectedId);
  }

  protected get triggerLabel(): string {
    return this.selectedOption?.label ?? this.label;
  }

  protected get filteredOptions(): readonly HubFilterOption[] {
    const query = this.query.trim().toLowerCase();

    if (!query) {
      return this.options;
    }

    return this.options.filter((option) => option.label.toLowerCase().includes(query));
  }

  protected setQuery(event: Event): void {
    this.query = (event.target as HTMLInputElement).value;
  }

  protected select(option: HubFilterOption): void {
    this.selectedId = option.id;
    this.selectedIdChange.emit(option.id);
    this.optionSelected.emit(option);
  }
}
