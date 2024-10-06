import { motion } from "framer-motion";

import "./_Login.scss";

import useLoginLogic from "./useLoginLogic";

import { loginForm } from "../../utils/formfields/fields";

import AuthHeader from "../../components/AuthHeader/AuthHeader";
import Formkit from "../../components/Formkit/Formkit";

function Login() {
  const { login, handleRedirect, handleForgotPassword } = useLoginLogic();

  return (
    <motion.section
      className="login"
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
        subtitle="Entre na sua conta"
        description="Bem-vindo de volta! Faça login para criar ou entrar em uma sala de videochamada e começar a conversar!"
      />
      <div className="login__form">
        <Formkit
          {...{
            fields: loginForm,
            handleFormAction: login,
            forgotPasswordAction: handleForgotPassword,
          }}
        />
        <p className="login__form-navigation">
          Ainda não tem uma conta?{" "}
          <span onClick={handleRedirect}>Registrar-se</span>
        </p>
      </div>
    </motion.section>
  );
}

export default Login;
