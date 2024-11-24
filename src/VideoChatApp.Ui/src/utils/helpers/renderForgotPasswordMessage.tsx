import { FormikErrors, FormikTouched, FormikValues } from "formik";

import { Field } from "../../contracts";

type RenderForgotPasswordMessageParams = {
  field: Field;
  errors: FormikErrors<FormikValues>;
  touched: FormikTouched<FormikValues>;
  forgotPasswordAction?: () => void;
};

export const renderForgotPasswordMessage = ({
  field,
  errors,
  touched,
  forgotPasswordAction,
}: RenderForgotPasswordMessageParams) => {
  // Retorna null se o campo não for do tipo "password"
  if (field.type !== "password") {
    return null;
  }

  const isError = errors[field.name] && touched[field.name];

  return (
    <div className="form__info">
      <small className={`form__msg ${isError ? "form__msg--variant" : ""}`}>
        {isError ? <>{errors[field.name]}</> : <>'error'</>}
      </small>
      {forgotPasswordAction && (
        <p
          className="form__msg form__msg--forgot"
          onClick={forgotPasswordAction}
        >
          Forgot password
        </p>
      )}
    </div>
  );
};

export default renderForgotPasswordMessage;
