import { FormikHelpers, FormikValues, useFormik } from "formik";
import React, { useRef, useState } from "react";
import * as yup from "yup";

import { FormProps, InputIconState } from "../../types";

interface FormValues {
  [key: string]: string;
}

const useFormkitLogic = (form: FormProps) => {
  const { fields, handleFormAction } = form;

  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isPasswordVisible, setIsPasswordVisible] = useState<InputIconState>(
    {},
  );

  const inputRefs = useRef<(HTMLInputElement | null)[]>(
    Array(fields.length).fill(null),
  );

  const initialValues: FormValues = Object.fromEntries(
    fields.map((field) => [field.name, ""]),
  );

  const validations = yup
    .object()
    .shape(
      Object.fromEntries(
        form.fields.map((field) => [field.name, field.validation]),
      ),
    );

  const forceBackspace = (inputRef: HTMLInputElement | null) => {
    if (inputRef) {
      const currentValue = inputRef.value;

      inputRef.value = currentValue.slice(0, -1);

      const event = new Event("input", { bubbles: true });
      inputRef.dispatchEvent(event);
    }
  };

  const onSubmit = async (
    values: FormValues,
    actions: FormikHelpers<FormValues>,
  ) => {
    setIsLoading((prevState) => !prevState);
    const isSuccess: boolean = await handleFormAction(values);

    if (isSuccess) {
      actions.resetForm(initialValues);

      inputRefs.current.forEach((ref) => {
        if (ref) {
          forceBackspace(ref);
        }
      });
    }

    setIsLoading((prevState) => !prevState);
  };

  const handleSubmitClick = async (
    event: React.MouseEvent<HTMLAnchorElement, MouseEvent>,
  ) => {
    event.preventDefault();
    await handleSubmit();
  };

  const togglePasswordVisibility = (name: string) => {
    setIsPasswordVisible((prevState) => {
      return {
        ...prevState,
        [name]: !prevState[name],
      };
    });
  };

  const {
    values,
    errors,
    touched,
    handleBlur,
    handleChange,
    handleSubmit,
    isSubmitting,
  } = useFormik<FormikValues>({
    initialValues,
    validationSchema: validations,
    onSubmit,
  });

  return {
    fields,
    values,
    errors,
    touched,
    handleBlur,
    handleChange,
    onSubmit,
    handleSubmit,
    handleSubmitClick,
    togglePasswordVisibility,
    isSubmitting,
    isLoading,
    isPasswordVisible,
    inputRefs,
  };
};

export default useFormkitLogic;
