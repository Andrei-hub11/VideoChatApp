import * as yup from "yup";

import emailIcon from "../../assets/icons/basil_gmail-outline.svg";
import passwordOffIcon from "../../assets/icons/mdi_eye-off-outline.svg";
import passwordIcon from "../../assets/icons/mdi_eye-outline.svg";
import personIcon from "../../assets/icons/mdi_user-outline.svg";

import { Field } from "../../contracts";

export const registerForm: Field[] = [
  {
    name: "userName",
    label: "Nome",
    validation: yup.string().required("O nome é obrigatório"),
    iconSrc: personIcon,
  },
  {
    name: "email",
    label: "Adicione um email",
    validation: yup.string().required("O email é obrigatório"),
    iconSrc: emailIcon,
  },
  {
    name: "password",
    label: "Diga-me uma senha",
    validation: yup
      .string()
      .min(8, "A senha deve ter no mínimo 8 caracteres")
      .matches(/^(?=.*[a-z])/, "Deve conter pelo menos 1 letra minúscula")
      .matches(/^(?=.*[A-Z])/, "Deve conter pelo menos 1 letra maiúscula")
      .matches(
        /^(?=(?:[^!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]*[!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]){2})([A-Za-z\d!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]+)$/,
        "Deve conter pelo menos 2 caracteres especiais",
      )
      .required("A senha é obrigatória"),
    type: "password",
    iconSrc: passwordIcon,
    iconOptional: passwordOffIcon,
  },
  {
    name: "passwordConfirmation",
    label: "Confirme a senha",
    validation: yup
      .string()
      .oneOf([yup.ref("password"), undefined], "As senhas devem coincidir")
      .required("A confirmação de senha é obrigatória"),
    type: "password",
    iconSrc: passwordIcon,
    iconOptional: passwordOffIcon,
  },
];

export const loginForm: Field[] = [
  {
    name: "email",
    label: "Adicione um email",
    validation: yup.string().required("O email é obrigatório"),
    iconSrc: emailIcon,
  },
  {
    name: "password",
    label: "Senha",
    validation: yup
      .string()
      .min(8, "A senha deve ter no mínimo 8 caracteres")
      .matches(/^(?=.*[a-z])/, "Deve conter pelo menos 1 letra minúscula")
      .matches(/^(?=.*[A-Z])/, "Deve conter pelo menos 1 letra maiúscula")
      .matches(
        /^(?=(?:[^!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]*[!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]){2})([A-Za-z\d!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]+)$/,
        "Deve conter pelo menos 2 caracteres especiais",
      )
      .required("A senha é obrigatória"),
    type: "password",
    iconSrc: passwordIcon,
    iconOptional: passwordOffIcon,
  },
];

export const updateProfileForm: Field[] = [
  {
    name: "userName",
    label: "Nome",
    validation: yup.string().required("O nome é obrigatório"),
    iconSrc: personIcon,
  },
  {
    name: "email",
    label: "Adicione um email",
    validation: yup.string().required("O email é obrigatório"),
    iconSrc: emailIcon,
  },
  {
    name: "password",
    label: "Diga-me uma senha",
    validation: yup.string().test({
      name: "password",
      test: (value, context) => {
        if (!value) return true; // Não valida se o campo está vazio

        if (value.length < 8) {
          return context.createError({
            message: "A senha deve ter no mínimo 8 caracteres",
          });
        }

        if (!/[a-z]/.test(value)) {
          return context.createError({
            message: "A senha deve conter pelo menos uma letra minúscula",
          });
        }

        if (!/[A-Z]/.test(value)) {
          return context.createError({
            message: "A senha deve conter pelo menos uma letra maiúscula",
          });
        }

        const specialCharCount = (
          value.match(/[!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]/g) || []
        ).length;

        if (specialCharCount < 2) {
          return context.createError({
            message: "A senha deve conter pelo menos dois caracteres especiais",
          });
        }

        return true;
      },
      message: "Senha inválida",
    }),
    type: "password",
    iconSrc: passwordIcon,
    iconOptional: passwordOffIcon,
  },
  {
    name: "passwordConfirmation",
    label: "Confirme a senha",
    validation: yup.string().test({
      test: function (value) {
        const password = this.parent.password;
        if (!password) return true;
        if (!value) return false;
        return value === password;
      },
      message: "As senhas devem coincidir",
    }),
    type: "password",
    iconSrc: passwordIcon,
    iconOptional: passwordOffIcon,
  },
];

export const passwordResetRequestForm: Field[] = [
  {
    name: "email",
    label: "Adicione um email",
    validation: yup.string().required("O email é obrigatório"),
    iconSrc: emailIcon,
  },
];

export const passwordResetForm: Field[] = [
  {
    name: "newPassword",
    label: "Nova senha",
    validation: yup
      .string()
      .min(8, "A senha deve ter no mínimo 8 caracteres")
      .matches(/^(?=.*[a-z])/, "Deve conter pelo menos 1 letra minúscula")
      .matches(/^(?=.*[A-Z])/, "Deve conter pelo menos 1 letra maiúscula")
      .matches(
        /^(?=(?:[^!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]*[!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]){2})([A-Za-z\d!@#$%^&*()\-_=+~`{}[\]|\\:;"'<>,.?/]+)$/,
        "Deve conter pelo menos 2 caracteres especiais",
      )
      .required("A senha é obrigatória"),
    type: "password",
    iconSrc: passwordIcon,
    iconOptional: passwordOffIcon,
  },
  {
    name: "passwordConfirmation",
    label: "Confirme a senha",
    validation: yup
      .string()
      .oneOf([yup.ref("newPassword"), undefined], "As senhas devem coincidir")
      .required("A confirmação de senha é obrigatória"),
    type: "password",
    iconSrc: passwordIcon,
    iconOptional: passwordOffIcon,
  },
];
