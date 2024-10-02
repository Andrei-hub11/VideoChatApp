import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

import {
  showAuthError,
  showUnknowError,
  showValidationErrors,
} from "../utils/helpers/alertErrors";
import { isUnknownError, isValidationError } from "../utils/helpers/guards";

import useAuth from "./useAuth/useAuth";
import useJwtState from "./useJwtState";

import { UnknownError } from "../types/http/types";

// Função que renova o token

export const useTokenRenewal = () => {
  const navigate = useNavigate();
  const {
    saveToken,
    removeToken,
    refreshToken,
    getAccessTokenExpirationDate,
    getRefreshTokenExpirationDate,
  } = useJwtState();
  const { renewAccessToken } = useAuth();

  useEffect(() => {
    const checkTokenExpiration = async () => {
      try {
        const refreshTokenExpiration = getRefreshTokenExpirationDate();
        const accessTokenExpiration = getAccessTokenExpirationDate();

        if (
          !refreshTokenExpiration ||
          !refreshToken ||
          !accessTokenExpiration
        ) {
          return;
        }

        const newAccessTokenExpirationDate = subtractMinutes(
          accessTokenExpiration,
          2,
        );

        if (new Date() > refreshTokenExpiration) {
          removeToken();
          showSessionExpiredError();
          navigate("/login");
        }

        if (new Date() > newAccessTokenExpirationDate) {
          const result = await renewAccessToken({ refreshToken });

          saveToken(result.accessToken);
        }
      } catch (error) {
        if (isValidationError(error)) {
          showValidationErrors(error);
          return;
        }

        if (isUnknownError(error)) {
          showUnknowError(error);
          return;
        }
      }
    };

    const intervalId = setInterval(checkTokenExpiration, 60000);

    return () => clearInterval(intervalId);
  }, [
    getAccessTokenExpirationDate,
    getRefreshTokenExpirationDate,
    removeToken,
    refreshToken,
    renewAccessToken,
    saveToken,
    navigate,
  ]);

  const subtractMinutes = (date: Date, minutes: number) => {
    return new Date(date.getTime() - minutes * 60000);
  };

  const showSessionExpiredError = () => {
    const apiError: UnknownError = {
      status: 401,
      type: "session_expired",
      title: "Session Expired",
      detail: "Your session has expired. Please log in again to continue.",
    };

    showAuthError(apiError);
  };
};
