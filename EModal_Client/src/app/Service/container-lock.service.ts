import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContainerLockService {
  private apiUrl = 'http://localhost:5189/api/container-lock';

  constructor(private http: HttpClient) {}

  isContainerLocked(containerId: string): Observable<{ containerId: string, isLocked: boolean }> {
    return this.http.get<{ containerId: string, isLocked: boolean }>(`${this.apiUrl}/isLocked?containerId=${containerId}`);
  }

  lockContainer(username: string, containerId: string ,Status: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/lock`, { username, containerId ,Status });
  }

  unlockContainer(containerId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/unlock?containerId=${containerId}`);
  }
}
