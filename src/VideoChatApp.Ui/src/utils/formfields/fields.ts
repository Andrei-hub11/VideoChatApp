import * as yup from "yup";

import emailIcon from "../../assets/icons/basil_gmail-outline.svg";
import passwordIcon from "../../assets/icons/mdi_eye-outline.svg";
import personIcon from "../../assets/icons/mdi_user-outline.svg";

import { Field } from "../../types";

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
    validation: yup.string().required("O nome é obrigatório"),
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
        /^(?=(?:[^!@#$%^&*]*[!@#$%^&*]){2})([A-Za-z\d!@#$%^&*]+)$/,
        "Deve conter pelo menos 2 caracteres especiais",
      )
      .required("A senha é obrigatória"),
    type: "password",
    iconSrc: passwordIcon,
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
  },
];

export const loginForm: Field[] = [
  {
    name: "email",
    label: "Adicione um email",
    validation: yup.string().required("O nome é obrigatório"),
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
        /^(?=(?:[^!@#$%^&*]*[!@#$%^&*]){2})([A-Za-z\d!@#$%^&*]+)$/,
        "Deve conter pelo menos 2 caracteres especiais",
      )
      .required("A senha é obrigatória"),
    type: "password",
    iconSrc: passwordIcon,
  },
];
