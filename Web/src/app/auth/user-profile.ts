export interface UserProfile {
  readonly sub: string;
  readonly id: number;
  readonly hasGlobalRole: boolean;
  readonly permissions: ReadonlyArray<PermissionDto>;
}

export interface PermissionDto {
  readonly id: number;
  readonly name: string;
  readonly description: string;
}
