import { motion } from "framer-motion";

import "./_Register.scss";

import useRegisterLogic from "./useRegisterLogic";

import { registerForm } from "../../utils/formfields/fields";

import AuthHeader from "../../components/AuthHeader/AuthHeader";
import Formkit from "../../components/Formkit/Formkit";

import ImageInputIcon from "../../assets/icons/ic_twotone-add-a-photo.svg";

function Register() {
  const {
    register,
    handleRedirect,
    handleImageInputClick,
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
        <div
          className={`register__form-img ${imagePreview ? "form-img--variant" : ""}`}
          onClick={handleImageInputClick}
        >
          <img src={imagePreview ? imagePreview : ImageInputIcon} alt="" />
          <input
            ref={profileImageInputRef}
            onChange={handleImageChange}
            type="file"
            accept="image/png, image/jpeg, image/webp"
          />
        </div>
        <Formkit {...{ fields: registerForm, handleFormAction: register }} />
        <p className="register__form-navigation">
          Já possui uma conta? <span onClick={handleRedirect}>Fazer login</span>
        </p>
      </div>
    </motion.section>
  );
}

export default Register;
