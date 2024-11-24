import { motion } from "framer-motion";

import "./_ForgotPassword.scss";

import useForgotPasswordLogic from "./useForgotPasswordLogic";

import { passwordResetForm, passwordResetRequestForm } from "@utils/exports";

import { Formkit, AuthHeader } from "@components/exports";

function ForgotPassword() {
  const { token, userId, forgotPassword, handleRedirect } =
    useForgotPasswordLogic();

  return (
    <motion.section
      className="forgot"
      initial={{ opacity: 0 }}
      animate={{ opacity: [0, 0.3, 0.7, 1] }}
      transition={{ duration: 1 }}
    >
      {token && userId ? (
        <>
          <AuthHeader
            title={
              <>
                Quick<span>Call</span>
              </>
            }
            subtitle="Criar Nova Senha"
            description="Insira sua nova senha abaixo. Certifique-se de escolher uma senha forte e exclusiva. Depois de redefinir, você poderá acessar sua conta com a nova senha."
          />
          <div className="forgot__form">
            <Formkit
              {...{
                fields: passwordResetForm,
                handleFormAction: forgotPassword,
              }}
            />
          </div>
        </>
      ) : (
        <>
          <AuthHeader
            title={
              <>
                Quick<span>Call</span>
              </>
            }
            subtitle="Redefinir Senha"
            description="Insira seu e-mail abaixo para receber um link de redefinição de senha. Verifique sua caixa de entrada e siga as instruções para criar uma nova senha."
          />
          <div className="forgot__form">
            <Formkit
              {...{
                fields: passwordResetRequestForm,
                handleFormAction: forgotPassword,
              }}
            />
            <p className="forgot__form-navigation">
              Lembrou sua senha?{" "}
              <span onClick={handleRedirect}>Faça login</span>
            </p>
          </div>
        </>
      )}
    </motion.section>
  );
}

export default ForgotPassword;
