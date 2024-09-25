import { motion } from "framer-motion";

import "./_Formikit.scss";

import useFormkitLogic from "./useFormkitLogic";

import Loader from "../../animations/Loader/Loader";
import { FormProps } from "../../types";
import Button from "../Button/Button";

function Formkit({ fields: fieldsForm, handleFormAction }: FormProps) {
  const {
    fields,
    values,
    errors,
    touched,
    handleBlur,
    handleChange,
    handleSubmit,
    handleSubmitClick,
    isSubmitting,
    isLoading,
    inputRefs,
  } = useFormkitLogic({ fields: fieldsForm, handleFormAction });

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
                type={field.type ? field.type : "text"}
                placeholder={field.label}
                value={values[field.name] || ""}
                onChange={handleChange}
                onBlur={handleBlur}
                ref={(e) => {
                  if (e) {
                    inputRefs.current[index] = e; // Atribuindo a ref com função de callback
                  }
                }}
                id={field.name}
              />
              <img
                className="form__input-icon"
                src={field.iconSrc}
                alt="form icon"
                height={24}
                width={24}
              />
            </div>
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
