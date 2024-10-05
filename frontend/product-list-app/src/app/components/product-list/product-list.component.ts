import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  standalone: true,
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
  imports: [FormsModule, CommonModule, HttpClientModule]
})
export class ProductListComponent implements OnInit {
  products: any[] = []; 
  displayedProducts: any[] = []; 
  page = 1; 
  pageSize = 10; 
  sort = 'name'; 
  searchTerm = ''; 
  baseImageUrl = 'https://tabledusud.nl'; 

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getProducts(1, 50, this.sort, this.searchTerm).subscribe({
      next: (data: any) => {
        console.log('Data received from API:', data);
        this.products = data.products || data;

        // ตรวจสอบโครงสร้างข้อมูล
        this.products.forEach(product => {
          console.log('Product:', product);
        });

        this.products = this.products.map(product => ({
          ...product,
          price: product.price ? Number(product.price.amount) : 0, 
          currency: product.price ? product.price.currency : 'THB' 
        }));

        this.updateDisplayedProducts(); 
      },
      error: (err) => {
        console.error('Error loading products:', err);
      }
    });
  }

  updateDisplayedProducts(): void {
    const filteredProducts = this.products.filter(product =>
      product.name.toLowerCase().includes(this.searchTerm.toLowerCase()) 
    );

    const startIndex = (this.page - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.displayedProducts = filteredProducts.slice(startIndex, endIndex); 
  }

  onSortChange(sort: string): void {
    this.sort = sort;
    
    this.products.sort((a, b) => {
      if (sort === 'name') {
        return a.name.localeCompare(b.name);
      } else if (sort === 'createdDate') {
        return new Date(a.createdDate).getTime() - new Date(b.createdDate).getTime();
      }
      return 0;
    });

    this.updateDisplayedProducts(); 
  }

  onSearchChange(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.updateDisplayedProducts(); 
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.getTotalPages()) {
      return; 
    }
    this.page = page;
    this.updateDisplayedProducts(); 
  }

  getTotalPages(): number {
    return Math.ceil(this.products.length / this.pageSize); 
  }
}
