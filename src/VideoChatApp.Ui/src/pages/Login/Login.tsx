import "./_Login.scss";

import { loginForm } from "../../utils/formfields/fields";

import AuthHeader from "../../components/AuthHeader/AuthHeader";
import Formkit from "../../components/Formkit/Formkit";

import useLoginLogic from "./useLoginLogic";

function Login() {
  const { register, handleRedirect } = useLoginLogic();

  return (
    <section className="login">
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
        <Formkit {...{ fields: loginForm, handleFormAction: register }} />
        <p className="login__form-navigation">
          Ainda não tem uma conta?{" "}
          <span onClick={handleRedirect}>Registrar-se</span>
        </p>
      </div>
    </section>
  );
}

export default Login;
