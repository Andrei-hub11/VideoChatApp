import { motion } from "framer-motion";

import "./_Register.scss";

import useRegisterLogic from "./useRegisterLogic";

import { registerForm } from "@utils/exports";

import { AuthHeader, ImageUploader, Formkit } from "@components/exports";

function Register() {
  const {
    handleRegister,
    handleRedirect,
    handleImageInputClick,
    handleTrashClick,
    handleImageChange,
    profileImageInputRef,
    imagePreview,
  } = useRegisterLogic();

  return (
    <motion.section
      className="register"
      initial={{ opacity: 0 }}
      animate={{ opacity: [0, 0.3, 0.7, 1] }}
      transition={{ duration: 1 }}
    >
      <AuthHeader
        title={
          <>
            Quick<span>Call</span>
          </>
        }
        subtitle="Crie uma conta"
        description="Registre-se para criar ou entrar em salas de videochamada e interagir com outros usuários"
      />
      <div className="register__form">
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
        <Formkit
          {...{ fields: registerForm, handleFormAction: handleRegister }}
        />
        <p className="register__form-navigation">
          Já possui uma conta? <span onClick={handleRedirect}>Fazer login</span>
        </p>
      </div>
    </motion.section>
  );
}

export default Register;
