export interface UserResponse {
  id: string;
  userName: string;
  email: string;
  profileImagePath: string;
}

export interface AuthResponse {
  user: UserResponse;
  accessToken: string;
  refreshToken: string;
  roles: string[];
}
