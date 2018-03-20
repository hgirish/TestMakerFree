import { Component } from '@angular/core';
import {Router } from '@angular/router';
import { AuthService} from '../services/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  constructor(public auth: AuthService,
  private router:Router) { }

  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout(): boolean {
    if (this.auth.logout()) {
      this.router.navigate([""]);
    }
    return false;
  }
}
