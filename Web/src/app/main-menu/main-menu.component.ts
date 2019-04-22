import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MainMenuItem } from './main-menu-item';

@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html',
  styleUrls: ['./main-menu.component.css']
})
export class MainMenuComponent {

  @Input() isLoggedIn: boolean;
  @Input() userName: string;

  @Input() menuItems: MainMenuItem[];

  @Output() logOutClicked = new EventEmitter<void>();

  logOut(): void {
    this.logOutClicked.emit();
  }
}
