import "./_Register.scss";

import { registerForm } from "../../utils/formfields/fields";

import AuthHeader from "../../components/AuthHeader/AuthHeader";
import Formkit from "../../components/Formkit/Formkit";

import useRegisterLogic from "./useRegisterLogic";

function Register() {
  const { register, handleRedirect } = useRegisterLogic();

  return (
    <section className="register">
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
        <Formkit {...{ fields: registerForm, handleFormAction: register }} />
        <p className="register__form-navigation">
          Já possui uma conta? <span onClick={handleRedirect}>Fazer login</span>
        </p>
      </div>
    </section>
  );
}

export default Register;
