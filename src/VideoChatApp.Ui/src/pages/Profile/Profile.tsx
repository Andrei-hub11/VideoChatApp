import { motion } from "framer-motion";

import "./_Profile.scss";

import useProfileLogic from "./useProfileLogic";

import { updateProfileForm } from "@utils/formfields/fields";

import FormProfile from "@components/FormProfile/FormProfile";
import { Header, ImageUploader } from "@components/exports";

const Profile = () => {
  const {
    handleUpdateProfile,
    handleImageInputClick,
    handleTrashClick,
    handleImageChange,
    imagePreview,
    profileImageInputRef,
  } = useProfileLogic();

  return (
    <motion.main
      className="profile"
      initial={{ opacity: 0 }}
      animate={{ opacity: [0, 1] }}
      exit={{ opacity: 0, transition: { duration: 0.2 } }}
      transition={{ duration: 0.5, delay: 2.2 }}
    >
      <Header {...{ currentPage: "Profile", requestsToJoin: [] }} />
      <div className="profile__form">
        <ImageUploader
          {...{
            imgUploaderProps: {
              imagePreview,
              handleImageInputClick,
              handleTrashClick,
              handleImageChange,
              profileImageInputRef,
            },
          }}
        />
        <FormProfile
          {...{
            fields: updateProfileForm,
            handleFormAction: handleUpdateProfile,
          }}
        />
      </div>
    </motion.main>
  );
};

export default Profile;
