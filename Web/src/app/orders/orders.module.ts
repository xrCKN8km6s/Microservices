import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrdersComponent } from './orders.component';
import { MaterialModule } from '../material.module';
import { FormsModule } from '@angular/forms';
import { OrdersService } from './orders.service';

@NgModule({
  declarations: [
    OrdersComponent
  ],
  imports: [
    CommonModule,
    OrdersRoutingModule,
    FormsModule,
    MaterialModule
  ],
  providers: [
    OrdersService
  ]
})
export class OrdersModule { }
