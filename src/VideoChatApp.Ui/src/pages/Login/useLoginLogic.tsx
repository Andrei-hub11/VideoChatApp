import { useEffect } from "react";
import { useMutation } from "react-query";
import { useNavigate } from "react-router-dom";

import { UserLoginRequest } from "@contracts/account/types";
import { ErrorTypes } from "@contracts/http/types";
import { AuthResponse } from "@contracts/httpResponse/types";

import { useJwtState, useUserStore } from "@hooks/exports";

import { login } from "@services/exports";

import { isLoginForm, handleException } from "@utils/exports";

const useLoginLogic = () => {
  const navigate = useNavigate();

  const { mutateAsync: loginMutation, isSuccess: isLoginSuccess } = useMutation<
    AuthResponse,
    ErrorTypes,
    UserLoginRequest
  >(login);

  const { saveToken, saveRefreshToken } = useJwtState();
  const { setUser, user } = useUserStore();

  useEffect(() => {
    if (isLoginSuccess && user) {
      navigate("/home");
    }
  }, [isLoginSuccess, navigate, user]);

  const handleLogin = async (values: unknown): Promise<boolean> => {
    try {
      if (isLoginForm(values)) {
        const result = await loginMutation({ ...values });

        setUser(result.user);
        saveToken(result.accessToken);
        saveRefreshToken(result.refreshToken);

        return true;
      }
    } catch (error: unknown) {
      handleException(error);

      return false;
    }

    return false;
  };

  const handleRedirect = () => {
    navigate("/register");
  };

  const handleForgotPassword = () => {
    navigate("/forgot-password");
  };

  return {
    handleLogin,
    handleRedirect,
    handleForgotPassword,
  };
};

export default useLoginLogic;
