import { Component, OnInit } from '@angular/core';
import { ContainerService } from '../../Service/container.service';
import { CartService } from '../../Service/cart.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WatchlistService } from '../../Service/watchlist.service';
import { ContainerLockService } from '../../Service/container-lock.service';

@Component({
  selector: 'app-watch-list',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './watch-list.component.html',
  styleUrls: ['./watch-list.component.css']
})
export class WatchListComponent implements OnInit {
  isPopupVisible = false;
  username: string = '';
  container = { containerId: '' };
  cartContainerNumbers: Set<string> = new Set(); 
  containers: any[] = [];
  cart: any[] = [];
  isLoading = false;
  errorMessage: string = '';

  constructor(
    private containerService: ContainerService, 
    private router: Router, 
    private cartService: CartService, 
    private watchlistService: WatchlistService,
    private containerLockService: ContainerLockService
  ) { }

  ngOnInit() {
    this.username = localStorage.getItem('username') || ''; 
    if (this.username) {
      this.fetchWatchlist();
      this.fetchCart();
    } else {
      alert('No user logged in! Redirecting to login.');
      this.router.navigate(['/login']);
    }
  }

  openPopup() {
    this.isPopupVisible = true;
  }

  closePopup() {
    this.isPopupVisible = false;
    this.container = { containerId: '' };
  }

  goToCart() {
    this.router.navigate(['/cart-list']);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('userInfo');
    localStorage.removeItem('username');
    this.router.navigate(['/login']);
  }

  
  fetchWatchlist() {
    this.isLoading = true;
    this.containers = [];
  
    if (!this.username) {
      console.error('Username is undefined. Cannot fetch watchlist.');
      this.isLoading = false;
      return;
    }
  
    this.watchlistService.getWatchlist(this.username).subscribe({
      next: (watchlist) => {
        console.log('Fetched Watchlist:', watchlist);
  
        if (!watchlist || watchlist.length === 0) {
          console.warn('Watchlist is empty.');
          this.isLoading = false;
          return;
        }
  
        let remainingItems = watchlist.length;
  
        watchlist.forEach((container, index) => {
          console.log(`Checking lock status for container ${container.containerNumber} at index ${index}...`);
  
          this.containerLockService.isContainerLocked(container.containerNumber).subscribe({
            next: (lockStatus) => {
              console.log(`Container ${container.containerNumber} lock status:`, lockStatus.isLocked);
              container.isLocked = lockStatus.isLocked;
  
              this.containerService.getContainerById(container.containerNumber).subscribe({
                next: (data) => {
                  container.details = data;
                  this.containers.push(container);
  
                  remainingItems--;
                  if (remainingItems === 0) {
                    this.sortContainers();
                  }
                },
                error: () => {
                  console.error(`Error fetching container ${container.containerNumber}`);
                  remainingItems--;
                  if (remainingItems === 0) {
                    this.sortContainers();
                  }
                }
              });
            },
            error: () => {
              console.error(`Error checking lock status for container ${container.containerNumber}`);
              remainingItems--;
            }
          });
        });
      },
      error: (error) => {
        console.error('Error fetching watchlist:', error);
        this.errorMessage = 'Error fetching watchlist. Please try again.';
        this.isLoading = false;
      }
    });
  }
  

  addToWatchlist() {
    if (!this.container.containerId) {
      alert('Please enter a container number.');
      return;
    }

    const watchlistItem = {
      username: this.username,
      containerNumber: this.container.containerId
    };

    this.watchlistService.addToWatchlist(watchlistItem).subscribe({
      next: () => {
        alert('Container added to watchlist!');
        this.fetchWatchlist(); // Reload watchlist
        this.closePopup();
      },
      error: (error) => {
        console.error('Error adding to watchlist:', error);
        alert('Error adding container to watchlist.');
      }
    });
  }

  removeFromWatchlist(container: any) {
    if (!container || !container.details?.containerId) {
        console.error('Error: Container object is undefined or missing containerId.');
        alert('Error: Container data is missing.');
        return;
    }

    const containerId = container.details.containerId; // Extract containerId

    this.watchlistService.removeFromWatchlist(this.username, containerId).subscribe({
      next: () => {
        this.containers = this.containers.filter((c) => c.details?.containerId !== containerId);
        
        this.removeFromCart(container);

        alert(`Container ${containerId} removed from watchlist.`);
      },
      error: (error) => {
        console.error('Error removing from watchlist:', error);
        alert('Error removing container from watchlist.');
      }
    });
}


  
  sortContainers(): void {
    this.containers.sort((a, b) => a.id - b.id);
    console.log('Sorted container list:', this.containers);
    this.isLoading = false;
  }

  fetchCart() {
    this.cartService.getCart(this.username).subscribe({
      next: (cartItems) => {
        this.cartContainerNumbers = new Set(cartItems.map(item => item.containerNumber));
        console.log('Cart Containers:', this.cartContainerNumbers);
      },
      error: () => {
        console.error('Error fetching cart.');
      }
    });
  }

  addToCart(container: any) {
    const cartItem = {
      username: this.username,
      containerNumber: container.containerNumber
    };
  
    this.cartService.addToCart(cartItem).subscribe({
      next: () => {
        alert('Container added to cart successfully!');
        this.cartContainerNumbers.add(container.containerNumber); 
        this.fetchCart(); 
      },
      error: () => {
        alert('Error adding container to cart.');
      }
    });
  }

 
  removeFromCart(container: any) {
    this.cartService.removeFromCart(this.username, container.containerNumber).subscribe({
      next: () => {
        this.cartContainerNumbers.delete(container.containerNumber); 
        this.fetchCart(); 
      },
      error: () => {
        console.log('Error removing container from cart.');
      }
    });
  }
}
