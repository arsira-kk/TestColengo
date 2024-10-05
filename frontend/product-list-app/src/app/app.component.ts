import { Component } from '@angular/core';
import { ProductListComponent } from './components/product-list/product-list.component';

@Component({
  standalone: true,
  selector: 'app-root',
  template: `
    <app-product-list></app-product-list>
  `,
  imports: [ProductListComponent]
})
export class AppComponent {}
