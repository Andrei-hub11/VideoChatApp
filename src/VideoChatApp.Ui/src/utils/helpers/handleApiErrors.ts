import { AxiosError } from "axios";

import { isNotFoundError, isUnknownError, isValidationError } from "./guards";

import {
  NotFoundError,
  UnknownError,
  ValidationError,
} from "../../contracts/http/types";

export const handleApiErrors = async (error: AxiosError): Promise<void> => {
  if (error.status === 404 && isNotFoundError(error.response?.data)) {
    const errorData: NotFoundError = error.response.data;
    throw errorData;
  }

  if (
    (error.status === 422 || error.status === 400) &&
    isValidationError(error.response?.data)
  ) {
    const errorData: ValidationError = error.response.data;
    throw errorData;
  }

  if (isUnknownError(error.response?.data)) {
    const errorData: UnknownError = error.response.data;
    throw errorData;
  }

  const unknownError: UnknownError = {
    type: "https://example.com/unknown-error",
    title: "Unknown Error",
    status: 500,
    detail: "An unknown error occurred.",
  };

  throw unknownError;
};
