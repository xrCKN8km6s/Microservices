import { Component, OnInit, OnDestroy } from '@angular/core';
import { RolesService } from './roles.service';
import { Role } from './role.type';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit {

  public displayedColumns: string[] = ['name', 'isGlobal', 'actions'];
  public roles: Role[];

  constructor(private svc: RolesService) { }

  ngOnInit(): void {
    this.svc.getRoles().subscribe(roles => {
      this.roles = roles;
    });
  }
}
