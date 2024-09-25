import { useMutation, useQuery } from "react-query";

import { getMe, register } from "../../services/auth/login";
import {
  UserRegisterRequest,
  UserRegisterResponse,
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

  const { refetch: refetchUser } = useQuery<UserResponse, Error>("me", getMe, {
    enabled: false, // Não faz a chamada automática ao montar
  });

  // Register mutation
  const {
    mutateAsync: registerMutation,
    isSuccess: isRegisterSuccess,
    isLoading: isRegisterLoading,
  } = useMutation<UserRegisterResponse, Error, UserRegisterRequest>(register);

  const fetchUserProfile = async (): Promise<UserResponse> => {
    const { data } = await refetchUser();

    if (!data) {
      throw new Error("User profile data is undefined.");
    }

    return data;
  };

  const registerUser = async (
    newAccount: UserRegisterRequest,
  ): Promise<UserRegisterResponse> => {
    return await registerMutation(newAccount);
  };

  const isLoading = isRegisterLoading;
  const isSuccess = isRegisterSuccess;

  return {
    isLoading,
    isSuccess,
    fetchUserProfile,
    registerUser,
  };
};

export default useAuth;
