import { Injectable, inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { BusMessage, ISignalRClient } from './contracts';
import { ConnectionState } from './models';
import { BFF_BASE_URL } from './tokens';

@Injectable({ providedIn: 'root' })
export class SignalRClient implements ISignalRClient {
  private readonly base = inject(BFF_BASE_URL);

  private readonly _messages = new Subject<BusMessage>();
  private readonly _state = new BehaviorSubject<ConnectionState>('disconnected');

  readonly messages$: Observable<BusMessage> = this._messages.asObservable();
  readonly connectionState$: Observable<ConnectionState> = this._state.asObservable();

  private connection: HubConnection | null = null;

  async start(): Promise<void> {
    this.connection = new HubConnectionBuilder()
      .withUrl(`${this.base}/hub`)
      .withAutomaticReconnect()
      .build();

    this.connection.on('OnMessage', (channel: string, envelopeJson: string) => {
      const envelope = JSON.parse(envelopeJson);
      this._messages.next({ channel, envelope });
    });

    this.connection.onreconnecting(() => this._state.next('reconnecting'));
    this.connection.onreconnected(() => this._state.next('connected'));
    this.connection.onclose(() => this._state.next('disconnected'));

    await this.connection.start();
    this._state.next('connected');
  }

  subscribe(channel: string): Promise<void> {
    return this.invokeVoid('Subscribe', channel);
  }

  unsubscribe(channel: string): Promise<void> {
    return this.invokeVoid('Unsubscribe', channel);
  }

  unsubscribeAll(): Promise<void> {
    return this.invokeVoid('UnsubscribeAll');
  }

  async send<T>(typeName: string, payload: object): Promise<T> {
    return await this.requireConnection().invoke<T>('Send', typeName, payload);
  }

  private async invokeVoid(method: string, ...args: unknown[]): Promise<void> {
    await this.requireConnection().invoke(method, ...args);
  }

  private requireConnection(): HubConnection {
    if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
      throw new Error('SignalR connection is not started.');
    }
    return this.connection;
  }
}
