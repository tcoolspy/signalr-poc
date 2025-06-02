import { Routes } from '@angular/router';
import {CustomersComponent} from './pages/customers/customers.component';
import {KitchenComponent} from './pages/kitchen/kitchen.component';

export const routes: Routes = [
  { path: 'customers', component: CustomersComponent },
  { path: 'kitchen', component: KitchenComponent },
];
