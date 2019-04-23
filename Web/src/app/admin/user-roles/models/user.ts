export interface User {
  readonly id: number;
  readonly name: string;
  readonly roles: ReadonlyArray<number>;
}
