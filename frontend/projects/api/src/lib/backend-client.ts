import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { IBackendClient } from './backend-client.contract';
import { BFF_BASE_URL } from './bff-base-url.contract';

@Injectable({ providedIn: 'root' })
export class BackendClient implements IBackendClient {
  private readonly http = inject(HttpClient);
  private readonly base = inject(BFF_BASE_URL);

  command<T>(type: string, payload: object): Promise<T> {
    return this.post<T>('commands', type, payload);
  }

  query<T>(type: string, payload: object): Promise<T> {
    return this.post<T>('queries', type, payload);
  }

  request<T>(type: string, payload: object): Promise<T> {
    return this.post<T>('requests', type, payload);
  }

  private post<T>(kind: string, type: string, payload: object): Promise<T> {
    return firstValueFrom(
      this.http.post<T>(`${this.base}/api/${kind}`, { type, payload }),
    );
  }
}
