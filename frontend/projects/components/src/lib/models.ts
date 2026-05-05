export type SignalButtonVariant = 'primary' | 'secondary' | 'ghost' | 'destructive';

export type SignalTone = 'success' | 'warning' | 'error' | 'info' | 'neutral';

export type SignalConnectionState = 'connected' | 'reconnecting' | 'disconnected' | 'paused';

export type SignalTrend = 'up' | 'down' | 'flat';

export type SignalMessageType = 'event' | 'command' | 'query' | 'request' | 'error' | 'telemetry';

export interface SignalMessage {
  timestamp: string;
  channel: string;
  type: SignalMessageType;
  title: string;
  payload: string | Record<string, unknown> | readonly unknown[];
}

export interface HubFilterOption {
  id: string;
  label: string;
  rate?: string;
  tone?: SignalTone;
}

export interface SignalToastData {
  title: string;
  message?: string;
  variant?: SignalTone;
  actionLabel?: string;
}

export interface SignalConfirmDialogData {
  title?: string;
  message?: string;
  details?: string;
  confirmLabel?: string;
  cancelLabel?: string;
  destructive?: boolean;
}
