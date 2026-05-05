import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { Chart, registerables } from 'chart.js';
import 'chartjs-adapter-date-fns';

Chart.register(...registerables);

export interface ChartPoint {
  t: number;
  c: number;
}

const PALETTE = ['#00d9ff', '#a855f7', '#22c55e', '#f59e0b', '#ef4444', '#3b82f6'];

@Component({
  selector: 'lib-temperature-chart',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<canvas #canvas></canvas>`,
  styles: [`:host { display: block; height: 320px; } canvas { width: 100%; height: 100%; }`],
})
export class TemperatureChartComponent implements AfterViewInit, OnChanges, OnDestroy {
  @Input() series: Map<string, ChartPoint[]> = new Map();

  @ViewChild('canvas') private canvasRef!: ElementRef<HTMLCanvasElement>;
  private chart: Chart | null = null;

  ngAfterViewInit(): void {
    this.chart = new Chart(this.canvasRef.nativeElement, {
      type: 'line',
      data: { datasets: [] },
      options: {
        animation: false,
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: { type: 'time', time: { unit: 'minute' } },
          y: { title: { display: true, text: '°C' } },
        },
        plugins: { legend: { position: 'bottom' } },
      },
    });
    this.refresh();
  }

  ngOnChanges(): void {
    this.refresh();
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  private refresh(): void {
    if (!this.chart) return;
    const datasets = Array.from(this.series.entries()).map(([roomId, points], i) => ({
      label: roomId,
      data: points.map((p) => ({ x: p.t, y: p.c })),
      borderColor: PALETTE[i % PALETTE.length],
      backgroundColor: PALETTE[i % PALETTE.length],
      tension: 0.3,
      pointRadius: 0,
    }));
    this.chart.data.datasets = datasets;
    this.chart.update('none');
  }
}
