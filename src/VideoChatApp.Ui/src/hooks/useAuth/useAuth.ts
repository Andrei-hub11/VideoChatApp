import { setLogger, useMutation, useQuery } from "react-query";

import {
  getMe,
  login,
  refreshToken,
  register,
  resetPassword,
  updatePassword,
} from "../../services/account/account";
import {
  AuthResponse,
  ForgotPasswordRequest,
  RenewTokenRequest,
  RenewTokenResponse,
  UpdatePasswordRequest,
  UserLoginRequest,
  UserRegisterRequest,
  UserResponse,
} from "../../types/account/types";
import { ErrorTypes } from "../../types/http/types";

const useAuth = () => {
  // Fetch the current user
  //   const {
  //     data: user,
  //     isLoading,
  //     error,
  //   } = useQuery<User, Error>("currentUser", fetchCurrentUser);

  setLogger({
    // eslint-disable-next-line no-console
    log: console.log,
    // eslint-disable-next-line no-console
    warn: console.warn,
    error: () => {}, // do nothing
  });

  const { refetch: refetchUser } = useQuery<UserResponse, ErrorTypes>(
    "me",
    getMe,
    {
      enabled: false, // Não faz a chamada automática ao montar
      retry: false,
    },
  );

  // Register mutation
  const {
    mutateAsync: registerMutation,
    isSuccess: isRegisterSuccess,
    isLoading: isRegisterLoading,
  } = useMutation<AuthResponse, ErrorTypes, UserRegisterRequest>(register);

  const {
    mutateAsync: loginMutation,
    isSuccess: isLoginSuccess,
    isLoading: isLoginLoading,
  } = useMutation<AuthResponse, ErrorTypes, UserLoginRequest>(login);

  const {
    mutateAsync: renewTokenMutation,
    isSuccess: isRenewTokenSuccess,
    isLoading: isRenewTokenLoading,
  } = useMutation<RenewTokenResponse, ErrorTypes, RenewTokenRequest>(
    refreshToken,
  );

  const {
    mutateAsync: resetPasswordMutation,
    isSuccess: isResetPasswordSuccess,
    isLoading: isResetPasswordLoading,
  } = useMutation<boolean, ErrorTypes, ForgotPasswordRequest>(resetPassword);

  const {
    mutateAsync: updatePasswordMutation,
    isSuccess: isUpdatePasswordSuccess,
    isLoading: isUpdatePasswordLoading,
  } = useMutation<boolean, ErrorTypes, UpdatePasswordRequest>(updatePassword);

  const fetchUserProfile = async (): Promise<UserResponse> => {
    const { data, error } = await refetchUser();

    if (error) {
      throw error;
    }

    if (!data) {
      throw new Error("User profile data is undefined.");
    }

    return data;
  };

  const registerUser = async (
    request: UserRegisterRequest,
  ): Promise<AuthResponse> => {
    return await registerMutation(request);
  };

  const userLogin = async (
    request: UserLoginRequest,
  ): Promise<AuthResponse> => {
    return await loginMutation(request);
  };

  const resetPasswordRequest = async (request: ForgotPasswordRequest) => {
    const response = await resetPasswordMutation(request);
    return response;
  };

  const renewAccessToken = async (request: RenewTokenRequest) => {
    const response = await renewTokenMutation(request);

    return response;
  };

  const updateUserPassword = async (
    request: UpdatePasswordRequest,
  ): Promise<boolean> => {
    const response = await updatePasswordMutation(request);
    return response;
  };

  const isLoading =
    isRegisterLoading ||
    isLoginLoading ||
    isRenewTokenLoading ||
    isResetPasswordLoading ||
    isUpdatePasswordLoading;

  const isSuccess =
    isRegisterSuccess ||
    isLoginSuccess ||
    isRenewTokenSuccess ||
    isResetPasswordSuccess ||
    isUpdatePasswordSuccess;

  return {
    isLoading,
    isSuccess,
    fetchUserProfile,
    registerUser,
    userLogin,
    resetPasswordRequest,
    renewAccessToken,
    updateUserPassword,
  };
};

export default useAuth;
