import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

import {
  showUnknowError,
  showUserNotFoundError,
  showValidationErrors,
} from "../../utils/helpers/alertErrors";
import {
  isNotFoundError,
  isRegisterForm,
  isUnknownError,
  isValidationError,
} from "../../utils/helpers/guards";
import { omit } from "../../utils/helpers/omit";

import useAuth from "../../hooks/useAuth/useAuth";
import useJwtState from "../../hooks/useJwtState";

const useRegisterLogic = () => {
  const navigate = useNavigate();
  const { registerUser, isSuccess } = useAuth();
  const { saveToken, saveRefreshToken } = useJwtState();

  useEffect(() => {
    if (isSuccess) {
      navigate("/home");
    }
  }, [isSuccess, navigate]);

  const register = async (values: unknown): Promise<boolean> => {
    try {
      if (isRegisterForm(values)) {
        const newValues = omit(values, "passwordConfirmation");

        const result = await registerUser({ ...newValues, profileImage: "" });

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
    navigate("/login");
  };

  return {
    register,
    handleRedirect,
  };
};

export default useRegisterLogic;
