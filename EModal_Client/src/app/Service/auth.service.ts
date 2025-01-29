import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, tap } from 'rxjs';
import { Router ,RouterModule} from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = 'http://localhost:5229/api';

  constructor(private http: HttpClient, private router: Router ,private httpClient: HttpClient) {}

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/auth/login`, credentials).pipe(
      tap((response) => {
        if (response.token) {
          console.log(response.token);
         localStorage.setItem('token', response.token);
          this.router.navigate(['/watch-container']);
        }
      }),
      catchError((error) => {
        console.error('Login error', error);
        throw error;
      })
    );
  }
  
  
  isAuthenticated(): boolean {
    return localStorage.getItem('token') !== null;
  }

  
}
