import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = 'http://localhost:5138/api/cart';

  constructor(private http: HttpClient) {}

  getCart(username: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${username}`);
  }

  addToCart(item: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, item);
  }

  removeFromCart(username: string, containerNumber: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${username}/${containerNumber}`);
  }
}
