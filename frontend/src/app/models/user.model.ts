export interface User {
  username: string;
  displayName: string;
  id?: number;
  email?: string;
  role?: string;
}

export interface ApiUser {
  id: number;
  username: string;
  fullName: string;
  email: string;
  isActive: boolean;
  role: string;
}
