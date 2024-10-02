import { isAxiosError } from "axios";
import Cookies from "js-cookie";

import { handleApiErrors } from "../../utils/helpers/handleApiErrors";

import {
  AuthResponse,
  RenewTokenRequest,
  RenewTokenResponse,
  UserLoginRequest,
  UserRegisterRequest,
  UserResponse,
} from "../../types/auth/types";
import api from "../base/api";

export const getMe = async (): Promise<UserResponse> => {
  try {
    const { data } = await api.get<UserResponse>("/account/profile", {
      headers: {
        Authorization: `Bearer ${Cookies.get("accessToken")}`,
      },
    });
    return data;
  } catch (error) {
    if (isAxiosError(error)) {
      await handleApiErrors(error);
    }

    throw error;
  }
};

export const register = async (
  request: UserRegisterRequest,
): Promise<AuthResponse> => {
  try {
    const { data } = await api.post<AuthResponse>("/account/register", request);

    return data;
  } catch (error: unknown) {
    if (isAxiosError(error)) {
      await handleApiErrors(error);
    }

    throw error;
  }
};

export const login = async (
  request: UserLoginRequest,
): Promise<AuthResponse> => {
  try {
    const { data } = await api.post<AuthResponse>("/account/login", request);

    return data;
  } catch (error: unknown) {
    if (isAxiosError(error)) {
      throw await handleApiErrors(error);
    }

    throw error;
  }
};

export const refreshToken = async (
  request: RenewTokenRequest,
): Promise<RenewTokenResponse> => {
  try {
    const { data } = await api.post<RenewTokenResponse>(
      "/account/token-renew",
      request,
    );
    return data;
  } catch (error: unknown) {
    if (isAxiosError(error)) {
      throw await handleApiErrors(error);
    }

    throw error;
  }
};