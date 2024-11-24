import { useEffect, useRef, useState } from "react";
import { useMutation } from "react-query";
import { useNavigate } from "react-router-dom";

import { UserRegisterRequest } from "@contracts/account/types";
import { ErrorTypes, UnknownError } from "@contracts/http/types";
import { AuthResponse } from "@contracts/httpResponse/types";

import { useJwtState } from "@hooks/exports";

import { register } from "@services/exports";

import {
  showUnknowError,
  isRegisterForm,
  handleException,
  omit,
} from "@utils/exports";

const useRegisterLogic = () => {
  const { mutateAsync: registerMutation, isSuccess: isRegisterSuccess } =
    useMutation<AuthResponse, ErrorTypes, UserRegisterRequest>(register);

  const navigate = useNavigate();
  const { saveToken, saveRefreshToken } = useJwtState();

  const profileImageInputRef = useRef<HTMLInputElement | null>(null);
  const [base64Image, setBase64Image] = useState<string | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  useEffect(() => {
    if (isRegisterSuccess) {
      navigate("/home");
    }
  }, [isRegisterSuccess, navigate]);

  const handleRegister = async (values: unknown): Promise<boolean> => {
    try {
      if (isRegisterForm(values)) {
        const newValues = omit(values, "passwordConfirmation");

        const result = await registerMutation({
          ...newValues,
          profileImage: base64Image ?? "",
        });

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

  const handleTrashClick = () => {
    setImagePreview(null);
    setBase64Image("");
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
    handleRegister,
    handleRedirect,
    handleImageInputClick,
    handleTrashClick,
    handleImageChange,
    imagePreview,
    profileImageInputRef,
  };
};

export default useRegisterLogic;
