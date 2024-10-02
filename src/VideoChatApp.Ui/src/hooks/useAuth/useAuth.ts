import { setLogger, useMutation, useQuery } from "react-query";

import {
  getMe,
  login,
  refreshToken,
  register,
} from "../../services/account/account";
import {
  AuthResponse,
  RenewTokenRequest,
  RenewTokenResponse,
  UserLoginRequest,
  UserRegisterRequest,
  UserResponse,
} from "../../types/auth/types";
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

  const renewAccessToken = async (request: RenewTokenRequest) => {
    const response = await renewTokenMutation(request);

    return response;
  };

  const isLoading = isRegisterLoading;
  const isSuccess = isRegisterSuccess || isLoginSuccess || isRenewTokenSuccess;

  return {
    isLoading,
    isSuccess,
    fetchUserProfile,
    registerUser,
    userLogin,
    renewAccessToken,
  };
};

export default useAuth;
