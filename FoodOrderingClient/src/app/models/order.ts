import {FoodItem} from './food-item';
import {OrderState} from './order-state';

export interface Order {
  id: number;
  tableNumber: number;
  foodItemId: number;
  foodItem: FoodItem;
  orderDate: Date; // using Date for DateTimeOffset
  orderState: OrderState;
}
