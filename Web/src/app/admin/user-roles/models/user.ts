export interface User {
  readonly id: number;
  readonly name: string;
  roles: ReadonlyArray<number>;
}
