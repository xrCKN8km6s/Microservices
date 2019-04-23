export interface Role {
  readonly id: number;
  readonly name: string;
  readonly isGlobal: boolean;
  readonly permissions: ReadonlyArray<number>;
}
