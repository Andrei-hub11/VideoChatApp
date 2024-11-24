import { FormikHelpers, FormikValues, useFormik } from "formik";
import { useCallback, useRef, useState } from "react";
import * as yup from "yup";

import { UserResponse } from "@contracts/httpResponse/types";

import { useUserStore } from "@hooks/exports";

import { UpdateProfileFormProps, InputIconState } from "@contracts/index";

interface FormValues {
  [key: string]: string;
}

const useFormProfileLogic = (form: UpdateProfileFormProps) => {
  const { fields, handleFormAction } = form;

  const { user } = useUserStore();

  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isPasswordVisible, setIsPasswordVisible] = useState<InputIconState>(
    {},
  );

  const inputRefs = useRef<(HTMLInputElement | null)[]>(
    Array(fields.length).fill(null),
  );

  const getUserFieldValue = (user: UserResponse, fieldName: string): string => {
    const fieldMapping: { [key: string]: keyof UserResponse } = {
      id: "id",
      userName: "userName",
      email: "email",
      profileImagePath: "profileImagePath",
    };

    return user[fieldMapping[fieldName]] || "";
  };

  const initialValues: FormValues = Object.fromEntries(
    fields.map((field) => [
      field.name,
      user ? getUserFieldValue(user, field.name) : field.name,
      "",
    ]),
  );

  const validations = yup
    .object()
    .shape(
      Object.fromEntries(
        form.fields.map((field) => [field.name, field.validation]),
      ),
    );

  const forceBackspace = useCallback((inputRef: HTMLInputElement | null) => {
    if (inputRef) {
      const currentValue = inputRef.value;

      inputRef.value = currentValue.slice(0, -1);

      const event = new Event("input", { bubbles: true });
      inputRef.dispatchEvent(event);
    }
  }, []);

  const onSubmit = useCallback(
    async (values: FormValues, actions: FormikHelpers<FormValues>) => {
      try {
        setIsLoading((prevState) => !prevState);
        const updatedUser: UserResponse | null = await handleFormAction(values);

        if (updatedUser) {
          const newInitialValues: FormValues = Object.fromEntries(
            fields.map((field) => [
              field.name,
              getUserFieldValue(updatedUser, field.name),
              "",
            ]),
          );

          actions.resetForm({ values: newInitialValues });

          inputRefs.current.forEach((ref) => {
            if (ref) {
              forceBackspace(ref);
            }
          });
        }
      } finally {
        setIsLoading((prevState) => !prevState);
      }
    },
    [forceBackspace, handleFormAction, fields],
  );

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

  const handleSubmitClick = useCallback(
    async (event: React.MouseEvent<HTMLAnchorElement, MouseEvent>) => {
      event.preventDefault();
      handleSubmit();
    },
    [handleSubmit],
  );

  const togglePasswordVisibility = (name: string) => {
    setIsPasswordVisible((prevState) => {
      return {
        ...prevState,
        [name]: !prevState[name],
      };
    });
  };

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

export default useFormProfileLogic;
