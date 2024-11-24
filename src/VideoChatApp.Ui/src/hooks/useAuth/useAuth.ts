import { setLogger, useMutation, useQuery } from "react-query";

import {
  RenewTokenRequest,
  RenewTokenResponse,
} from "../../contracts/account/types";
import { ErrorTypes } from "../../contracts/http/types";
import { UserResponse } from "@contracts/httpResponse/types";

import { getMe, refreshToken } from "../../services/account/account";

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

  const renewAccessToken = async (request: RenewTokenRequest) => {
    const response = await renewTokenMutation(request);

    return response;
  };

  const isLoading = isRenewTokenLoading;

  const isSuccess = isRenewTokenSuccess;

  return {
    isLoading,
    isSuccess,
    fetchUserProfile,
    renewAccessToken,
  };
};

export default useAuth;
