import {Component, OnInit, signal} from '@angular/core';
import {firstValueFrom, Subscription} from 'rxjs';
import {RealtimeClientService} from '../../realtime-client.service';
import {HttpClient} from '@angular/common/http';
import {Order} from '../../models/order';
import {OrderState} from '../../models/order-state';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-kitchen',
  standalone: true,
  imports: [
    DatePipe
  ],
  templateUrl: './kitchen.component.html',
  styleUrl: './kitchen.component.css'
})
export class KitchenComponent implements OnInit {
  foodStates = ['Ordered', 'Preparing', 'AwaitingDelivery', 'Completed'];

  orderSubscription: Subscription | undefined;
  orders = signal<Order[]>([]);
  constructor(private realtime: RealtimeClientService, private http: HttpClient) {
  }

  async ngOnInit() {
    // Load exisiting orders (static data)
    //this.realtime.connect();
    let existingOrders = await firstValueFrom(this.http.get<Array<Order>>('http://localhost:5019/api/Kitchen/GetExistingOrders'));
    this.orders.set([...existingOrders]);
    /// Subscribe to future order updates
    this.orderSubscription = this.realtime.ordersUpdated$.subscribe(orders => this.orders.set([...orders]));
  }

  async updateState(id: number, $event: Event) {
    let value = ($event.target as HTMLSelectElement)?.value; // Get the text from the control
    await this.realtime.updateFoodItem(id, value as OrderState); // Set the new enum value
  }
}
