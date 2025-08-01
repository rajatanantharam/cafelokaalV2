import { Injectable, ElementRef } from '@angular/core';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';
import { CafeOrderModel } from '../models/order.models';

@Injectable({
  providedIn: 'root'
})
export class ChartService {

  constructor() {
    Chart.register(...registerables);
  }

  createChart(chartElement: ElementRef<HTMLCanvasElement>, cafeOrders: CafeOrderModel[], existingChart?: Chart): Chart | undefined {
    if (!chartElement) return undefined;

    const ctx = chartElement.nativeElement.getContext('2d');
    if (!ctx) return undefined;

    // Destroy existing chart if it exists
    if (existingChart) {
      existingChart.destroy();
    }

    const data = {
      labels: cafeOrders.map(order => `Order ${order.processDate}`),
      datasets: [{
        label: 'Process Time (minutes)',
        data: cafeOrders.map(order => order.processTime),
        backgroundColor: [
          'rgba(54, 162, 235, 0.8)',
          'rgba(255, 99, 132, 0.8)',
          'rgba(255, 205, 86, 0.8)',
          'rgba(75, 192, 192, 0.8)',
          'rgba(153, 102, 255, 0.8)',
          'rgba(255, 159, 64, 0.8)',
          'rgba(199, 199, 199, 0.8)',
          'rgba(83, 102, 255, 0.8)',
        ],
        borderColor: [
          'rgba(54, 162, 235, 1)',
          'rgba(255, 99, 132, 1)',
          'rgba(255, 205, 86, 1)',
          'rgba(75, 192, 192, 1)',
          'rgba(153, 102, 255, 1)',
          'rgba(255, 159, 64, 1)',
          'rgba(199, 199, 199, 1)',
          'rgba(83, 102, 255, 1)',
        ],
        borderWidth: 2,
        borderRadius: 10,
        borderSkipped: false,
      }]
    };

    const config: ChartConfiguration = {
      type: 'bar' as ChartType,
      data: data,
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'top',
          },
          title: {
            display: true,
            text: 'Order Processing Times'
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            title: {
              display: true,
              text: 'Time (minutes)'
            }
          },
          x: {
            title: {
              display: true,
              text: 'Orders'
            }
          }
        }
      },
    };

    return new Chart(ctx, config);
  }
}
