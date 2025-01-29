import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WatchlistService {
  private apiUrl = 'http://localhost:5138/api/watchlist';

  constructor(private http: HttpClient) {}

  getWatchlist(username: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${username}`);
  }

  addToWatchlist(item: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, item);
  }

  removeFromWatchlist(username: string, containerNumber: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${username}/${containerNumber}`);
  }
}
