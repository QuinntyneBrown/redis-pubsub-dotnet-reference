import { InjectionToken } from '@angular/core';

export interface IBackendClient {
  command<T>(type: string, payload: object): Promise<T>;
  query<T>(type: string, payload: object): Promise<T>;
  request<T>(type: string, payload: object): Promise<T>;
}

export const BACKEND_CLIENT = new InjectionToken<IBackendClient>('BACKEND_CLIENT');
