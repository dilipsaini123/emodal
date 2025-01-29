import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  private apiUrl = 'http://localhost:5130/api/Payments'; 

  constructor(private http: HttpClient) {}

  submitPayment(paymentDetails: any): Observable<any> {
    const token = localStorage.getItem('token') 

    if (!token) {
      throw new Error('User is not authenticated');
    }
    const paymentData = {
      username: paymentDetails.username,
      containers: paymentDetails.containers,  
      cardNumber: paymentDetails.cardNumber,
      cvv: paymentDetails.cvv,
      expiry: paymentDetails.expiry,
      totalFees: paymentDetails.totalFees
    };

    console.log( paymentDetails.containers)


    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  
    return this.http.post(`${this.apiUrl}`, paymentData,{headers:headers});
  }
}
