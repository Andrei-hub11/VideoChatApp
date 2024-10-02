import {
  LoginForm,
  RegisterForm,
  UserRegisterRequest,
} from "../../types/auth/types";
import {
  NotFoundError,
  UnknownError,
  ValidationError,
} from "../../types/http/types";

export function isNotFoundError(obj: unknown): obj is NotFoundError {
  return (
    typeof obj === "object" &&
    obj !== null &&
    "type" in obj &&
    typeof obj.type === "string" &&
    "title" in obj &&
    typeof obj.title === "string" &&
    "status" in obj &&
    typeof obj.status === "number" &&
    "detail" in obj &&
    typeof obj.detail === "string" &&
    "instance" in obj &&
    typeof obj.instance === "string"
  );
}

// Função de guarda para ApiError
export function isValidationError(obj: unknown): obj is ValidationError {
  return (
    typeof obj === "object" &&
    obj !== null &&
    "Type" in obj &&
    typeof obj.Type === "string" &&
    "Title" in obj &&
    typeof obj.Title === "string" &&
    "Status" in obj &&
    typeof obj.Status === "number" &&
    "Detail" in obj &&
    typeof obj.Detail === "string" &&
    "Instance" in obj &&
    typeof obj.Instance === "string" &&
    "Errors" in obj &&
    typeof obj.Errors === "object" &&
    obj.Errors !== null
  );
}

// Função de guarda para unknownnError
export function isUnknownError(obj: unknown): obj is UnknownError {
  return (
    typeof obj === "object" &&
    obj !== null &&
    "type" in obj &&
    typeof obj.type === "string" &&
    "title" in obj &&
    typeof obj.title === "string" &&
    "status" in obj &&
    typeof obj.status === "number" &&
    "detail" in obj &&
    typeof obj.detail === "string"
  );
}

export function isRegisterForm(obj: unknown): obj is RegisterForm {
  return (
    typeof obj === "object" &&
    obj !== null &&
    "userName" in obj &&
    typeof obj["userName"] === "string" &&
    "email" in obj &&
    typeof obj["email"] === "string" &&
    "password" in obj &&
    typeof obj["password"] === "string" &&
    "passwordConfirmation" in obj &&
    typeof obj["passwordConfirmation"] === "string"
  );
}

export function isLoginForm(obj: unknown): obj is LoginForm {
  return (
    typeof obj === "object" &&
    obj !== null &&
    "email" in obj &&
    typeof obj.email === "string" &&
    "password" in obj &&
    typeof obj.password === "string"
  );
}

export function isUserRegisterRequest(
  obj: unknown,
): obj is UserRegisterRequest {
  return (
    typeof obj === "object" &&
    obj !== null &&
    "userName" in obj &&
    typeof obj.userName === "string" &&
    "email" in obj &&
    typeof obj.email === "string" &&
    "password" in obj &&
    typeof obj.password === "string"
  );
}
