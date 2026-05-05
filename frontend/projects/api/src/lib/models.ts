export interface TemperatureReading {
  sensorId: string;
  roomId: string;
  celsius: number;
  at: string;
}

export interface OccupancyReading {
  sensorId: string;
  roomId: string;
  occupied: boolean;
  at: string;
}

export interface RoomOccupancyChanged {
  roomId: string;
  occupied: boolean;
  at: string;
}

export interface ThermostatChanged {
  deviceId: string;
  roomId: string;
  effectiveC: number;
  at: string;
}

export interface RoomStatus {
  roomId: string;
  name: string;
  temperatureC: number | null;
  occupied: boolean;
  lastUpdated: string | null;
  devices: string[];
}

export interface SetThermostat {
  roomId: string;
  targetC: number;
}

export interface SetThermostatAck {
  roomId: string;
  deviceId: string;
  effectiveC: number;
}

export interface Point {
  t: number;
  c: number;
}

export type ConnectionState = 'connected' | 'reconnecting' | 'disconnected';
