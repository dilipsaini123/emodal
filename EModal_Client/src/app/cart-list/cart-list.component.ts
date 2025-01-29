
import { Component, OnInit } from '@angular/core';
import { CartService } from '../Service/cart.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PaymentService } from '../Service/payment.service';
import { Router, RouterModule } from '@angular/router';
import { ContainerService } from '../Service/container.service';
import { ContainerLockService } from '../Service/container-lock.service';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Component({
  selector: 'app-cart-list',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './cart-list.component.html',
  styleUrls: ['./cart-list.component.css'],
  standalone: true,
})
export class CartListComponent implements OnInit {
  username: string = ''; 
  cart: any[] = []; 
  totalFees = 0; 
  showPaymentModal = false; 
  paymentDetails = {
    cardNumber: '',
    cvv: '',
    expiry: '',
  };
  paymentErrors = {
    cardNumber: '',
    cvv: '',
    expiry: '',
  };
  Status: string="Active";

  constructor(private cartService: CartService, private paymentService: PaymentService, private router: Router, private containerService: ContainerService, private containerLockService: ContainerLockService) {}

  ngOnInit() {
    this.username = localStorage.getItem('username') || '';
    if (!this.username) {
      alert('No user logged in! Redirecting to login.');
      this.router.navigate(['/login']);
      return;
    }
    this.fetchCart();
  }

  fetchCart() {
    this.cartService.getCart(this.username).subscribe({
      next: (cartItems) => {
        console.log('Fetched Cart Items (Raw API Response):', cartItems);
  
        if (!cartItems || cartItems.length === 0) {
          console.warn('Cart is empty.');
          this.cart = [];
          return;
        }
  
        let remainingItems = cartItems.length;
        this.cart = [];
  
        cartItems.forEach((cartItem, index) => {
          console.log(`Fetching details for container ${cartItem.containerNumber} at index ${index}...`);
  
          this.containerService.getContainerById(cartItem.containerNumber).subscribe({
            next: (containerData) => {
              console.log(`Fetched container details for ${cartItem.containerNumber}:`, containerData);
  
              cartItem.details = containerData;
              this.cart.push(cartItem);
  
              remainingItems--;
              if (remainingItems === 0) {
                this.sortCart();
              }
            },
            error: () => {
              console.error(`Error fetching container ${cartItem.containerNumber}`);
              remainingItems--;
              if (remainingItems === 0) {
                this.sortCart();
              }
            }
          });
        });
      },
      error: (error) => {
        console.error('Error fetching cart:', error);
        alert('Error fetching cart data.');
      }
    });
  }
  
  sortCart(): void {
    this.cart.sort((a, b) => a.id - b.id);
    console.log('Sorted Cart List:', this.cart);
  }
  
  get selectedContainers() {
    return this.cart.filter(container => container.selected);
  }

  calculateTotalFees() {
    this.totalFees = this.selectedContainers.reduce(
      (sum, container) => sum + (container.details?.demurrageFees + container.details?.additionalFees),
      0
    );
  }

  removeFromCart(containerNumber: string) {
    if (!containerNumber) return;
  
    this.cartService.removeFromCart(this.username, containerNumber).subscribe(
      () => {
        this.cart = this.cart.filter(container => container.details?.containerId !== containerNumber);
        this.calculateTotalFees();
        console.log(`Container ${containerNumber} removed from cart.`);
      },
      (error) => {
        console.error(` Error removing ${containerNumber}:`, error);
        alert('Error removing container from cart.');
      }
    );
  }
  

  toggleSelectAll(event: any) {
    const selectAll = event.target.checked;
    this.cart.forEach(container => container.selected = selectAll);
    this.calculateTotalFees();
  }

  isAllSelected(): boolean {
    return this.cart.every(container => container.selected);
  }

  paySelectedContainers() {
    this.calculateTotalFees(); 
  
    console.log("Selected Containers for Payment:", this.selectedContainers);
  
    if (this.selectedContainers.length > 0) {
      Promise.all(
        this.selectedContainers.map(container =>
          this.containerLockService.lockContainer(this.username, container.details?.containerId,this.Status).toPromise()
        )
      ).then(() => {
        this.showPaymentModal = true;
      }).catch(error => {
        console.error('Error locking containers:', error);
        // alert('Error locking containers.');
        const errorMessage = error.error?.error || error.statusText || 'An unknown error occurred';

        alert('Error: ' + errorMessage);
      });
    } else {
      alert('No containers selected for payment.');
    }
  }
  
  
  

  validatePaymentDetails(): boolean {
    let isValid = true;

    if (!this.paymentDetails.cardNumber || this.paymentDetails.cardNumber.length < 13) {
      this.paymentErrors.cardNumber = 'Please enter a valid card number.';
      isValid = false;
    } else {
      this.paymentErrors.cardNumber = '';
    }

    if (!this.paymentDetails.cvv || this.paymentDetails.cvv.length < 3) {
      this.paymentErrors.cvv = 'Please enter a valid CVV.';
      isValid = false;
    } else {
      this.paymentErrors.cvv = '';
    }

    if (!this.paymentDetails.expiry || !this.isValidExpiryDate(this.paymentDetails.expiry)) {
      this.paymentErrors.expiry = 'Please enter a valid expiry date (MM/YY).';
      isValid = false;
    } else {
      this.paymentErrors.expiry = '';
    }

    return isValid;
  }

  
  isValidExpiryDate(expiry: string): boolean {
    const [month, year] = expiry.split('/');
    const currentDate = new Date();
    const currentMonth = currentDate.getMonth() + 1;
    const currentYear = currentDate.getFullYear() % 100;

    return (
      parseInt(month) >= 1 &&
      parseInt(month) <= 12 &&
      (parseInt(year) > currentYear || (parseInt(year) === currentYear && parseInt(month) >= currentMonth))
    );
  }

  
  submitPayment() {
    if (this.validatePaymentDetails()) {
      const { cardNumber, cvv, expiry } = this.paymentDetails;
  
      const paymentData = {
        username: this.username,
        containers: this.selectedContainers.map(container => ({
          ContainerId: container.details?.containerId,
          Fees:container.details?.demurrageFees + container.details?.additionalFees
        })),
        cardNumber,
        cvv,
        expiry,
        totalFees: this.totalFees
      };
  
      console.log(" Sending Payment Data:", JSON.stringify(paymentData, null, 2));
  
      this.paymentService.submitPayment(paymentData).subscribe(
        async (response) => {
          console.log(' Payment successful:', response);
          this.totalFees = 0;
  
          try {
            await Promise.all(
              this.selectedContainers.map(async (container) => {
                await this.cartService.removeFromCart(this.username, container.details?.containerId).toPromise();
              })
            );
            await Promise.all(
              this.selectedContainers.map(async (container) => {
                await this.containerLockService.unlockContainer(container.details?.containerId).toPromise();
              })
            );
  
            this.cart = this.cart.filter(
              container => !this.selectedContainers.some(
                selected => selected.details?.containerId === container.details?.containerId
              )
            );
  
            this.closePaymentModal();
            alert(" Payment Successful");
            this.router.navigate(['/watch-container']);

  
          } catch (error) {
            console.error(' Error while removing containers from cart:', error);
            alert(" Payment Successful, but some containers could not be removed from the cart.");
          }
  
        },
        (error) => {
          console.error(' Payment failed:', error);
          alert(" Payment failed. Please try again.");
        }
      );
    } else {
      console.log(' Please complete all payment details.');
    }
  }
  
  BackToWatchList() {
    this.router.navigate(['/watch-container']);
  }
  

  closePaymentModal() {
    this.showPaymentModal = false;
    this.paymentDetails = { cardNumber: '', cvv: '', expiry: '' };
  }
}
