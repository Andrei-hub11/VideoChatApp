import { useEffect, useRef } from "react";

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
  const {
    saveToken,
    removeToken,
    removeRefreshToken,
    refreshToken,
    getAccessTokenExpirationDate,
    getRefreshTokenExpirationDate,
  } = useJwtState();
  const { renewAccessToken } = useAuth();
  const isCheckingRef = useRef<boolean>(false);

  useEffect(() => {
    const checkTokenExpiration = async () => {
      if (isCheckingRef.current) {
        return;
      }

      try {
        isCheckingRef.current = true;
        const refreshTokenExpiration = getRefreshTokenExpirationDate();
        const accessTokenExpiration = getAccessTokenExpirationDate();

        const now = new Date();

        if (
          !refreshTokenExpiration ||
          !refreshToken ||
          now > refreshTokenExpiration
        ) {
          if (refreshToken) {
            removeToken();
            removeRefreshToken();
          }
          return;
        }

        if (minutesUntilExpiration(accessTokenExpiration) < 2) {
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
      } finally {
        isCheckingRef.current = false;
      }
    };

    checkTokenExpiration();

    const intervalId = setInterval(checkTokenExpiration, 30000);

    return () => clearInterval(intervalId);
  }, [
    getAccessTokenExpirationDate,
    getRefreshTokenExpirationDate,
    removeToken,
    renewAccessToken,
    saveToken,
    refreshToken,
    removeRefreshToken,
  ]);

  const showSessionExpiredError = () => {
    const apiError: UnknownError = {
      status: 401,
      type: "session_expired",
      title: "Session Expired",
      detail: "Your session has expired. Please log in again to continue.",
    };

    showAuthError(apiError);
  };

  const minutesUntilExpiration = (expirationDate: Date | null): number => {
    if (!expirationDate) {
      return 0;
    }

    const now = new Date();
    const minutesUntilExpiration =
      (expirationDate.getTime() - now.getTime()) / (1000 * 60);

    return minutesUntilExpiration;
  };
};

export default useTokenRenewal;
