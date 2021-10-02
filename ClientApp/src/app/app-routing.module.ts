import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { CardsComponent } from './cards/cards.component';
import { GamesComponent } from './games/games.component';
import { AuthorizeGuard } from '/Projects/MyTCGCollection/ClientApp/src/api-authorization/authorize.guard';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'cards', component: CardsComponent, canActivate: [AuthorizeGuard] },

  { path: 'games', component: GamesComponent, canActivate: [AuthorizeGuard] }
];
  

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule {}
