import {
  showUnknowError,
  showUserNotFoundError,
  showValidationErrors,
} from "./alertErrors";
import { isNotFoundError, isUnknownError, isValidationError } from "./guards";

export const handleException = (error: unknown) => {
  if (isValidationError(error)) {
    showValidationErrors(error);
    return;
  }

  if (isNotFoundError(error) && error.status === 404) {
    showUserNotFoundError(error);
    return;
  }

  if (isUnknownError(error)) {
    showUnknowError(error);
    return;
  }
};
