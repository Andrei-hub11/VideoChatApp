import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { UnknownError } from "../../contracts/http/types";

import { useAuth, useJwtState } from "@hooks/exports";

import {
  isUnknownError,
  isValidationError,
  showAuthError,
  showUnknowError,
  showValidationErrors,
} from "@utils/exports";

const useTokenRenewal = () => {
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

export default useTokenRenewal;
