import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { LoginComponent } from "./auth/login/login.component";
import { HttpClientModule } from '@angular/common/http';  // <-- Import HttpClientModule
import { WatchListComponent } from './watch-list/watch-list/watch-list.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CartListComponent } from './cart-list/cart-list.component';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, FormsModule,CommonModule, LoginComponent, WatchListComponent,CartListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css' 
})
export class AppComponent {
  title = 'EModal_Client';
}
