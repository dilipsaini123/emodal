
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
interface ContainerData {
  containerId: string;
  type: string;
}
@Injectable({
  providedIn: 'root'
})
export class ContainerService {
  private containers: any[] = [];
  private apiUrl = 'http://localhost:5229/api/Containers';  

  constructor(private http: HttpClient) { }
  addToCart(container: any): void {
    if (!this.containers.some(c => c.containerId === container.containerId)) {
      this.containers.push(container);
    }
  }
  getContainerById(containerId: string): Observable<ContainerData> {
    const url = `${this.apiUrl}/${containerId}`;
    return this.http.get<ContainerData>(url, {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${localStorage.getItem('token')}`  
      })
    });
  }}
