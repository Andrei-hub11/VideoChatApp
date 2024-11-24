import { motion } from "framer-motion";

import "./_Formikit.scss";

import useFormkitLogic from "./useFormkitLogic";

import renderForgotPasswordMessage from "../../utils/helpers/renderForgotPasswordMessage";

import Loader from "../../animations/Loader/Loader";
import { FormProps } from "../../contracts";
import Button from "../Button/Button";

function Formkit({
  fields: fieldsForm,
  handleFormAction,
  forgotPasswordAction,
}: FormProps) {
  const {
    fields,
    values,
    errors,
    touched,
    handleBlur,
    handleChange,
    handleSubmit,
    handleSubmitClick,
    togglePasswordVisibility,
    isSubmitting,
    isLoading,
    isPasswordVisible,
    inputRefs,
  } = useFormkitLogic({
    fields: fieldsForm,
    handleFormAction,
    forgotPasswordAction,
  });

  return (
    <motion.form action="" className="form" onSubmit={handleSubmit}>
      {fields.map((field, index) => {
        return (
          <div className="form__control" key={field.name}>
            <div className="form__control-container">
              <input
                className={`form__input ${
                  errors[field.name] && touched[field.name]
                    ? "form__input--variant"
                    : ""
                }`}
                type={
                  field.type === "password" && isPasswordVisible[field.name]
                    ? "text"
                    : field.type
                }
                placeholder={field.label}
                value={values[field.name] || ""}
                onChange={handleChange}
                onBlur={handleBlur}
                ref={(e) => {
                  if (e) {
                    inputRefs.current[index] = e;
                  }
                }}
                id={field.name}
              />

              {field.type !== "password" ? (
                <img
                  className="form__input-icon"
                  src={field.iconSrc}
                  alt="form icon"
                  height={24}
                  width={24}
                />
              ) : null}

              {field.type === "password" && field.iconOptional ? (
                <img
                  className="form__input-icon"
                  src={
                    isPasswordVisible[field.name]
                      ? field.iconSrc
                      : field.iconOptional
                  }
                  alt="form icon"
                  height={24}
                  width={24}
                  onClick={() => togglePasswordVisibility(field.name)}
                />
              ) : null}
            </div>
            {renderForgotPasswordMessage({
              field,
              errors,
              touched,
              forgotPasswordAction: forgotPasswordAction,
            })}
            {field.type !== "password" ? (
              <div>
                <small
                  className={`form__msg 
                ${errors[field.name] && touched[field.name] ? "form__msg--variant" : ""}`}
                >
                  {errors[field.name] && touched[field.name] ? (
                    <>{errors[field.name]}</>
                  ) : (
                    <>'error'</>
                  )}
                </small>
              </div>
            ) : null}
          </div>
        );
      })}
      <div className="form__action">
        <Button
          btn={{
            variant_key: "formAction",
            onClick: handleSubmitClick,
            disabled: isSubmitting,
          }}
        >
          {isLoading ? <Loader /> : "Enviar"}
        </Button>
      </div>
    </motion.form>
  );
}

export default Formkit;
