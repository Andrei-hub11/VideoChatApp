export type RegisterForm = {
  userName: string;
  email: string;
  password: string;
  passwordConfirmation: string;
};

export type UserRegisterRequest = {
  userName: string;
  email: string;
  password: string;
  profileImage: string | null;
};

export interface UserResponse {
  id: string;
  userName: string;
  email: string;
  profileImagePath: string;
}

export interface UserRegisterResponse {
  user: UserResponse;
  accessToken: string;
  refreshToken: string;
  roles: string[];
}
