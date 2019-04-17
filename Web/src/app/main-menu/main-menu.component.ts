import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html',
  styleUrls: ['./main-menu.component.css']
})
export class MainMenuComponent {

  @Input() isLoggedIn: boolean;
  @Input() userName: string;

  @Input() isOrdersVisible: boolean;
  @Input() isAdminVisible: boolean;

  @Output() logOutClicked = new EventEmitter<void>();

  logOut(): void {
    this.logOutClicked.emit();
  }
}
