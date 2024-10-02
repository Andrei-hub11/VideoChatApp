import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

import {
  showUnknowError,
  showUserNotFoundError,
  showValidationErrors,
} from "../../utils/helpers/alertErrors";
import {
  isLoginForm,
  isNotFoundError,
  isUnknownError,
  isValidationError,
} from "../../utils/helpers/guards";

import useAuth from "../../hooks/useAuth/useAuth";
import useJwtState from "../../hooks/useJwtState";
import useUserStore from "../../hooks/useUserStore";

const useLoginLogic = () => {
  const navigate = useNavigate();

  const { userLogin, isSuccess } = useAuth();
  const { saveToken, saveRefreshToken } = useJwtState();
  const { setUser, user } = useUserStore();

  useEffect(() => {
    if (isSuccess && user) {
      navigate("/home");
    }
  }, [isSuccess, navigate, user]);

  const login = async (values: unknown): Promise<boolean> => {
    try {
      if (isLoginForm(values)) {
        const result = await userLogin({ ...values });

        setUser(result.user);
        saveToken(result.accessToken);
        saveRefreshToken(result.refreshToken);

        return true;
      }
    } catch (error: unknown) {
      if (isValidationError(error)) {
        showValidationErrors(error);
        return false;
      }

      if (isNotFoundError(error) && error.status === 404) {
        showUserNotFoundError(error);
        return false;
      }

      if (isUnknownError(error)) {
        showUnknowError(error);
        return false;
      }

      return false;
    }

    return false;
  };

  const handleRedirect = () => {
    navigate("/register");
  };

  return {
    login,
    handleRedirect,
  };
};

export default useLoginLogic;
