import axios, { isAxiosError } from "axios";

import { handleApiErrors } from "../../utils/helpers/handleApiErrors";

import {
  UserRegisterRequest,
  UserRegisterResponse,
  UserResponse,
} from "../../types/auth/types";

const apiUrl: string = import.meta.env.VITE_API_URL;

export const getMe = async (): Promise<UserResponse> => {
  const { data } = await axios.get<UserResponse>(apiUrl + "/account/me", {
    headers: {
      Authorization: `Bearer ${Cookies.get("accessToken")}`,
    },
  });
  return data;
};

export const register = async (
  newAccount: UserRegisterRequest,
): Promise<UserRegisterResponse> => {
  try {
    const { data } = await axios.post<UserRegisterResponse>(
      apiUrl + "/account/register",
      newAccount,
    );

    return data;
  } catch (error: unknown) {
    if (isAxiosError(error)) {
      throw await handleApiErrors(error);
    }

    throw error;
  }
};
