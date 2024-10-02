import { useEffect, useRef, useState } from "react";
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
import { UnknownError } from "../../types/http/types";

const useRegisterLogic = () => {
  const navigate = useNavigate();
  const { registerUser, isSuccess } = useAuth();
  const { saveToken, saveRefreshToken } = useJwtState();

  const profileImageInputRef = useRef<HTMLInputElement | null>(null);
  const [base64Image, setBase64Image] = useState<string | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  useEffect(() => {
    if (isSuccess) {
      navigate("/home");
    }
  }, [isSuccess, navigate]);

  const register = async (values: unknown): Promise<boolean> => {
    try {
      if (isRegisterForm(values)) {
        const newValues = omit(values, "passwordConfirmation");

        console.log(base64Image);

        const result = await registerUser({
          ...newValues,
          profileImage: base64Image ?? "",
        });

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

  const handleImageInputClick = () => {
    profileImageInputRef.current?.click();
  };

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];

    if (file) {
      const image = new Image();
      const imageUrl = URL.createObjectURL(file);

      const reader = new FileReader();

      reader.onloadend = () => {
        const base64Data = reader.result as string;
        const cleanedBase64 = base64Data.split(",")[1];

        setBase64Image(cleanedBase64);
      };

      image.src = imageUrl;
      image.onload = () => {
        if (image.height > image.width || image.width > image.height) {
          showImageInputError();
          setImagePreview(null);
          URL.revokeObjectURL(imageUrl);
        } else {
          setImagePreview(imageUrl);
          reader.readAsDataURL(file);
        }
      };
    }
  };

  const showImageInputError = () => {
    const imageError: UnknownError = {
      status: 400,
      type: "invalid_image",
      title: "Invalid Image",
      detail:
        "The image must be square to fit properly. Please upload a square image.",
    };

    showUnknowError(imageError);
  };

  return {
    register,
    handleRedirect,
    handleImageInputClick,
    handleImageChange,
    imagePreview,
    profileImageInputRef,
  };
};

export default useRegisterLogic;
