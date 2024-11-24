export type RegisterForm = {
  userName: string;
  email: string;
  password: string;
  passwordConfirmation: string;
};

export type LoginForm = {
  email: string;
  password: string;
};

export type UpdateProfileForm = {
  userName: string;
  email: string;
  password: string;
};

export type UserRegisterRequest = {
  userName: string;
  email: string;
  password: string;
  profileImage: string;
};

export type UserLoginRequest = {
  email: string;
  password: string;
};

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

export type UpdateProfileRequest = {
  newUserName: string;
  newEmail: string;
  newPassword: string;
  newProfileImage: string;
};

export type RenewTokenRequest = {
  refreshToken: string;
};

export interface RenewTokenResponse {
  accessToken: string;
}

export type ForgotPasswordRequest = {
  email: string;
};

export interface UpdatePasswordRequest {
  newPassword: string;
  userId: string;
  token: string;
}
