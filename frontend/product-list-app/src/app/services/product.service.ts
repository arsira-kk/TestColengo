// product.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'http://localhost:5233/api/products'; // URL ของ API

  constructor(private http: HttpClient) {}

  getProducts(page: number, pageSize: number, sort: string, searchTerm: string): Observable<any> {
    // ส่งคำขอ GET ไปยัง API พร้อมกับพารามิเตอร์
    return this.http.get<any>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}&sort=${sort}&searchTerm=${searchTerm}`);
  }
}
