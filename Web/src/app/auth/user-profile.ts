export class UserProfile {
  public sub: string;
  public id: number;
  public hasGlobalRole: boolean;
  public permissions: PermissionDto[];
}

export class PermissionDto {
  public id: number;
  public name: string;
  public description: string;
}
