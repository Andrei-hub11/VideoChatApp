import { useEffect, useRef, useState } from "react";
import { useMutation } from "react-query";

import { UserResponse } from "@contracts/account/types";
import { UpdateProfileRequest } from "@contracts/account/types";
import { ErrorTypes } from "@contracts/http/types";

import { useUserStore } from "@hooks/exports";

import { updateProfile } from "@services/account/account";

import { handleException } from "@utils/exports";
import { isUpdateProfileForm } from "@utils/helpers/guards";

const useProfileLogic = () => {
  const { user, setUser } = useUserStore();

  const { mutateAsync: updateProfileMutation } = useMutation<
    UserResponse,
    ErrorTypes,
    { userId: string; request: UpdateProfileRequest }
  >(({ userId, request }) => updateProfile(userId, request));

  const profileImageInputRef = useRef<HTMLInputElement | null>(null);
  const [base64Image, setBase64Image] = useState<string | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  useEffect(() => {
    if (user?.profileImagePath && user.profileImagePath !== "") {
      setImagePreview(
        import.meta.env.VITE_SERVER_URL + `/${user.profileImagePath}`,
      );
    }
  }, [user]);

  const handleUpdateProfile = async (
    values: unknown,
  ): Promise<UserResponse | null> => {
    try {
      if (isUpdateProfileForm(values)) {
        const updatedUser = await updateProfileMutation({
          userId: user?.id ?? "",
          request: {
            newUserName: values.userName,
            newEmail: values.email,
            newPassword: values.password,
            newProfileImage: base64Image ?? "",
          },
        });

        console.log(updatedUser);

        setUser(updatedUser);
        return updatedUser;
      }
    } catch (error: unknown) {
      handleException(error);

      return null;
    }

    return null;
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

  return {
    handleUpdateProfile,
    handleImageInputClick,
    handleTrashClick,
    handleImageChange,
    imagePreview,
    profileImageInputRef,
  };
};

export default useProfileLogic;
function showImageInputError() {
  throw new Error("Function not implemented.");
}
