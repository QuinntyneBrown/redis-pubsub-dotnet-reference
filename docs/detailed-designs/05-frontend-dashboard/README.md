# Frontend Application — Angular Dashboard — Detailed Design

**Status:** Draft

## 1. Overview

A single-page Angular application that connects to the [BFF (Feature 04)](../04-bff-service/README.md) and presents the [Smart Building reference app's](../02-reference-application/README.md) data through a dark-themed Material UI dashboard. The frontend is a teaching tool: it exists to demonstrate that the five message patterns are usable end-to-end from a real browser client.

**Scope:**
- One dashboard page. No router-driven multi-page app.
- Live telemetry charts (Chart.js).
- Live room status table (Angular Material table).
- Thermostat control (Material form → command via BFF).
- Connection status indicator.

**Out of scope:** Authentication, multi-user routing, internationalization, mobile-specific layouts.

**Tech:**
- **Angular** (latest LTS, standalone components, signals where natural).
- **Angular Material** with a prebuilt **dark** theme.
- **Chart.js** (used directly — no `ng2-charts` wrapper, to keep the dependency graph small).
- **`@microsoft/signalr`** for the WebSocket connection to the BFF hub.
- **`HttpClient`** for HTTP commands / queries / requests.

## 2. Architecture

### 2.1 C4 Context

![C4 Context](diagrams/c4_context.png)

### 2.2 C4 Container

The Angular SPA is a single browser-side container; the BFF and Redis sit on the other side.

![C4 Container](diagrams/c4_container.png)

### 2.3 C4 Component

Internal Angular components and services.

![C4 Component](diagrams/c4_component.png)

## 3. Component Details

### 3.1 `AppShellComponent`
- Material toolbar (app title, connection status indicator).
- Hosts the dashboard directly; no router today.

### 3.2 `DashboardComponent`
- Composes the three live widgets: `<app-temperature-chart>`, `<app-rooms-table>`, `<app-thermostat-control>`.
- Wires data flow from `RoomStateStore` and `TelemetryStore` into the widgets via `async` pipe / signals.
- On init, opens the SignalR connection (`SignalRClient.start`), subscribes to the four channels the dashboard cares about, and seeds room state from `ListRooms`.

### 3.3 `TemperatureChartComponent`
- Renders a Chart.js line chart, **one line per room**, time on the x-axis, °C on the y-axis. Rolling 5-minute window (matching the aggregator's window in Feature 02).
- Subscribes to `TelemetryStore.temperatureSeries$` and pushes points as they arrive. Throttles re-renders to at most one per 250 ms to keep the chart fluid even at high inbound rates.

### 3.4 `RoomsTableComponent`
- Material `mat-table` with columns: **Room**, **Name**, **Temperature (°C)**, **Occupied**, **Last Updated**.
- Data source is `RoomStateStore.rooms$`.
- Seeded on startup by `ListRooms` query (HTTP); subsequent updates merge in via SignalR (`RoomOccupancyChanged`, `ThermostatChanged`, telemetry).

### 3.5 `ThermostatControlComponent`
- Material form: room selector (from `RoomStateStore.rooms$`) + slider (15–28 °C).
- On submit, calls `BackendClient.command<SetThermostatAck>("Contracts.Requests.SetThermostat", { roomId, targetC })` and toasts the resulting `effectiveC`.

### 3.6 `SignalRClient` (service)
Owns the singleton `HubConnection` to `/hub`. Auto-reconnects via `withAutomaticReconnect()`. Exposes:
- `subscribe(channel: string): Promise<void>`
- `unsubscribe(channel: string): Promise<void>`
- `messages$: Observable<{ channel: string; envelope: any }>` — multicast stream of every received message.
- `send<TResponse>(typeName: string, payload: object): Promise<TResponse>` — alternative to `BackendClient` for command / query / request over SignalR.
- `connectionState$: Observable<'connected' | 'reconnecting' | 'disconnected'>` — drives the toolbar status chip.

### 3.7 `BackendClient` (service)
HTTP wrapper over `HttpClient` for command / query / request:
- `command<T>(type: string, payload: object): Promise<T>` → `POST /api/commands`
- `query<T>(type: string, payload: object): Promise<T>` → `POST /api/queries`
- `request<T>(type: string, payload: object): Promise<T>` → `POST /api/requests`

Surfaces 4xx/5xx responses as typed errors so the UI can show a Material snackbar.

### 3.8 `RoomStateStore`
- Holds the canonical room snapshot. Subscribes to `SignalRClient.messages$` filtered to `evt.*` and merges updates into a `BehaviorSubject<RoomSnapshot[]>`.
- Initial seed comes from `BackendClient.query("Contracts.Queries.ListRooms", {})`.

### 3.9 `TelemetryStore`
- Holds rolling temperature points per room, capped at the last 5 minutes.
- Subscribes to `SignalRClient.messages$` filtered to telemetry channels and appends each `TemperatureReading` to that room's series.

### 3.10 Subscription policy on startup

`DashboardComponent.ngOnInit` subscribes to:

```
tel.Contracts.TemperatureReading
tel.Contracts.OccupancyReading
evt.Contracts.RoomOccupancyChanged
evt.Contracts.ThermostatChanged
```

`ngOnDestroy` calls `SignalRClient.unsubscribeAll()`. Subscriptions are tied to component life, not page life — if the dashboard ever lives behind a router, navigating away cleanly drops the topics.

## 4. Data Model

### 4.1 TypeScript records

Mirror the .NET Contracts records as TypeScript interfaces. Hand-maintained — small enough that codegen would be overkill.

```ts
interface TemperatureReading      { sensorId: string; roomId: string; celsius: number; at: string; }
interface OccupancyReading        { sensorId: string; roomId: string; occupied: boolean; at: string; }
interface RoomOccupancyChanged    { roomId: string; occupied: boolean; at: string; }
interface ThermostatChanged       { deviceId: string; roomId: string; effectiveC: number; at: string; }
interface RoomStatus              { roomId: string; name: string; temperatureC: number | null; occupied: boolean; lastUpdated: string | null; devices: string[]; }
interface SetThermostat           { roomId: string; targetC: number; }
interface SetThermostatAck        { roomId: string; deviceId: string; effectiveC: number; }
```

### 4.2 Class Diagram

The Angular services and stores plus their connection to the BFF.

![Class Diagram](diagrams/class_diagram.png)

## 5. Key Workflows

### 5.1 Page load → connect → subscribe → render

![Sequence Bootstrap](diagrams/sequence_bootstrap.png)

### 5.2 User changes thermostat

A request flow that uses HTTP for the command and SignalR for the resulting `ThermostatChanged` event broadcast.

![Sequence Set Thermostat](diagrams/sequence_set_thermostat.png)

## 6. UI / UX

### 6.1 Layout

A single-column dashboard. Material toolbar at the top, three stacked panels:

1. **Temperature chart** — full-width Chart.js line chart, one colored line per room, smooth bezier curves, ~5-minute rolling window, animated point-add.
2. **Rooms table** — Material table, dense rows, occupancy shown as a colored Material chip (green = occupied, gray = empty).
3. **Thermostat control** — compact card with room dropdown + slider + submit button.

### 6.2 Theme

Angular Material's prebuilt **dark** theme (e.g. `cyan-orange` from `@angular/material/prebuilt-themes/`, or a custom theme via `mat.define-dark-theme()`). Background near-black; primary cyan/teal; accents amber/orange to make active chart lines pop. Material's `density-2` for the table to keep rows compact.

### 6.3 Connection status

Toolbar right-corner Material chip:
- **Live** (green) when SignalR is connected.
- **Reconnecting…** (amber) during transient disconnects.
- **Offline** (red) on terminal failure.

Driven by `SignalRClient.connectionState$`.

## 7. Open Questions

- **Codegen for TypeScript contracts.** Hand-maintained is fine for the demo (~10 records). If the message catalog grows, a `dotnet typegen` step or `NSwag`-from-OpenAPI is warranted.
- **Single dashboard vs. per-room drill-down.** Out of scope; would be added later behind the Angular router.
- **Chart libraries.** Chart.js is sufficient for line charts. If candlesticks / heatmaps are needed, swap to ECharts.
- **State management library.** Plain RxJS subjects are sufficient at this size. NgRx or `signalStore` would be considered if the store graph grows past a handful of slices.
