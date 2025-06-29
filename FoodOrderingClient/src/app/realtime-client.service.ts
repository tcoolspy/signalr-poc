import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import {Order} from './models/order';
import { TimerUpdate } from './models/timer-update';
import {Observable, Subject} from 'rxjs';
import {FoodRequest} from './models/food-request';
import {OrderState} from './models/order-state';

@Injectable({
  providedIn: 'root'
})
export class RealtimeClientService {
  private hubConnection: signalR.HubConnection;
  private pendingFoodUpdateSubject = new Subject<Order[]>();
  private broadcastTimerUpdateSubject = new Subject<TimerUpdate[]>();
  ordersUpdated$: Observable<Order[]> = this.pendingFoodUpdateSubject.asObservable();
  timerUpdate$: Observable<TimerUpdate[]> = this.broadcastTimerUpdateSubject.asObservable();
  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5019/foodhub')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection to FoodHub started!'))
      .catch(err => console.log('Error while starting connection to FoodHub: ' + err));

    this.hubConnection.on('PendingFoodUpdated', (orders: Order[]) => {
      this.pendingFoodUpdateSubject.next(orders);
    });

    this.hubConnection.on('UpdateOrderTime', (currentTimeUpdate: TimerUpdate[]) => {
      //console.log(currentTimeUpdate);
      this.broadcastTimerUpdateSubject.next(currentTimeUpdate);
    });
  }

  async orderFoodItem(foodId: number, table: number) {
    console.log("ordering food item " + foodId + " on table " + table);
    await this.hubConnection?.invoke('OrderFoodItem', {
      foodId,
      table
    } as FoodRequest);
  }

  async updateFoodItem(orderId: number, state: OrderState) {
    await this.hubConnection?.invoke('UpdateFoodItem', orderId, state);
  }
}
