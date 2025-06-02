import {Component, OnInit, signal} from '@angular/core';
import {FoodItem} from '../../models/food-item';
import {Order} from '../../models/order';
import {RealtimeClientService} from '../../realtime-client.service';
import {HttpClient} from '@angular/common/http';
import {firstValueFrom, Subscription} from 'rxjs';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [
    FormsModule
  ],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css'
})
export class CustomersComponent implements OnInit {
  availableFood = signal<Array<FoodItem>>([]);
  activeOrders = signal<Array<Order>>([]);
  activeOrdersSubscription?: Subscription;
  tableNumber?: number;
  apiBaseUrl = 'http://localhost:5019';

  showActiveOrders = false;
  needsLogin = true;
  login?: string;
  password?: string;

  constructor(private realtime: RealtimeClientService, private http: HttpClient) {}

  async ngOnInit() {
    let food = await firstValueFrom(this.http.get<Array<FoodItem>>(`${this.apiBaseUrl}/api/FoodItems/GetFoodItems`));
    this.availableFood.set([...food]);

    let orders = await firstValueFrom(this.http.get<Array<Order>>(`${this.apiBaseUrl}/api/Kitchen/GetExistingOrders`));
    this.activeOrders.set([...orders]);

    this.activeOrdersSubscription = this.realtime.ordersUpdated$.subscribe(orders => {
      this.activeOrders.set([...orders]);
    });
  }

  async loadOrders() {
    let food = await firstValueFrom(this.http.get<Array<FoodItem>>(`${this.apiBaseUrl}/api/FoodItems/GetFoodItems`));
    this.availableFood.set([...food]);

    let orders = await firstValueFrom(this.http.get<Array<Order>>(`${this.apiBaseUrl}/api/Kitchen/GetExistingOrders`));
    this.activeOrders.set([...orders]);

    this.activeOrdersSubscription = this.realtime.ordersUpdated$.subscribe(orders => {
      this.activeOrders.set([...orders]);
    });
  }

  async sendOrder(foodId: number, tableNumber: number) {
    await this.realtime.orderFoodItem(foodId, tableNumber);
  }

  showActiveOrdersToggle() {
    this.showActiveOrders = !this.showActiveOrders;
  }
}
