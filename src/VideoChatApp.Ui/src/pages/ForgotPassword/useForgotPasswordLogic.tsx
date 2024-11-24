import { useMutation } from "react-query";
import { useNavigate, useSearchParams } from "react-router-dom";

import {
  ForgotPasswordRequest,
  UpdatePasswordRequest,
} from "@contracts/account/types";
import { ErrorTypes } from "@contracts/http/types";

import { updatePassword } from "@services/account/account";
import { resetPassword } from "@services/exports";

import {
  showPasswordUpdateSuccessAlert,
  showResetPasswordAlert,
} from "../../utils/helpers/alerts";
import {
  isResetPasswordRequestForm,
  isUpdatePasswordRequestForm,
} from "../../utils/helpers/guards";
import { handleException } from "@utils/exports";

const useForgotPasswordLogic = () => {
  const navigate = useNavigate();

  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const userId = searchParams.get("userId");

  const { mutateAsync: resetPasswordMutation } = useMutation<
    boolean,
    ErrorTypes,
    ForgotPasswordRequest
  >(resetPassword);

  const { mutateAsync: updatePasswordMutation } = useMutation<
    boolean,
    ErrorTypes,
    UpdatePasswordRequest
  >(updatePassword);

  const handleResetPasswordRequest = async (request: ForgotPasswordRequest) => {
    const response = await resetPasswordMutation(request);
    return response;
  };

  const handleUpdateUserPassword = async (
    request: UpdatePasswordRequest,
  ): Promise<boolean> => {
    const response = await updatePasswordMutation(request);
    return response;
  };

  const forgotPassword = async (values: unknown): Promise<boolean> => {
    try {
      if ((!token || !userId) && isResetPasswordRequestForm(values)) {
        await handleResetPasswordRequest({ email: values.email });
        showResetPasswordAlert(values.email);
        return true;
      }

      if (token && userId && isUpdatePasswordRequestForm(values)) {
        await handleUpdateUserPassword({
          userId: userId,
          token: token,
          newPassword: values.newPassword,
        });
        showPasswordUpdateSuccessAlert();
        navigate("/login");
        return true;
      }
    } catch (error: unknown) {
      handleException(error);

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
