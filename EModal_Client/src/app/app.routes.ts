import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { AuthGuard } from '../app/AuthGaurd/auth.guard';
import { WatchListComponent } from './watch-list/watch-list/watch-list.component';
import { NgModule } from '@angular/core';
import { CartListComponent } from './cart-list/cart-list.component';


export const routes: Routes = [
    {
    path:'' ,redirectTo:'login',pathMatch:'full', 
   },
   {
    path:'login', component:LoginComponent,
    
   }
   ,{
    path:'watch-container', component:WatchListComponent,canActivate:[AuthGuard]
    },
    {
      path:"cart-list",component:CartListComponent,canActivate:[AuthGuard]
    }

];
@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
  })
  export class AppRoutingModule {}
