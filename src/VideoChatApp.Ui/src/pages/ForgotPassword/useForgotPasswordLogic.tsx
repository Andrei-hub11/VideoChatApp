import { useNavigate, useSearchParams } from "react-router-dom";

import {
  showUnknowError,
  showValidationErrors,
} from "../../utils/helpers/alertErrors";
import {
  showPasswordUpdateSuccessAlert,
  showResetPasswordAlert,
} from "../../utils/helpers/alerts";
import {
  isResetPasswordRequestForm,
  isUnknownError,
  isUpdatePasswordRequestForm,
  isValidationError,
} from "../../utils/helpers/guards";

import useAuth from "../../hooks/useAuth/useAuth";

const useForgotPasswordLogic = () => {
  const navigate = useNavigate();
  const { resetPasswordRequest, updateUserPassword } = useAuth();

  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const userId = searchParams.get("userId");

  const forgotPassword = async (values: unknown): Promise<boolean> => {
    try {
      if ((!token || !userId) && isResetPasswordRequestForm(values)) {
        await resetPasswordRequest({ email: values.email });
        showResetPasswordAlert(values.email);
        return true;
      }

      if (token && userId && isUpdatePasswordRequestForm(values)) {
        await updateUserPassword({
          userId: userId,
          token: token,
          newPassword: values.newPassword,
        });
        showPasswordUpdateSuccessAlert();
        navigate("/login");
        return true;
      }
    } catch (error: unknown) {
      if (isValidationError(error)) {
        showValidationErrors(error);
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
    token,
    userId,
    forgotPassword,
    handleRedirect,
  };
};

export default useForgotPasswordLogic;
