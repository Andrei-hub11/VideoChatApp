import { describe, expect, it } from "vitest";

import {
  isLoginForm,
  isNotFoundError,
  isRegisterForm,
  isUnknownError,
  isUserRegisterRequest,
  isValidationError,
} from "../../utils/helpers/guards";

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

// Adjust paths as necessary

describe("Type Guards", () => {
  describe("isNotFoundError", () => {
    it("should return true for a valid NotFoundError object", () => {
      const validError: NotFoundError = {
        type: "https://example.com/not-found",
        title: "Not Found",
        status: 404,
        detail: "Resource not found",
        instance: "some-instance-id",
      };
      expect(isNotFoundError(validError)).toBe(true);
    });

    it("should return false for an invalid NotFoundError object", () => {
      const invalidError = {
        title: "Not Found",
        status: 404,
      };
      expect(isNotFoundError(invalidError)).toBe(false);
    });
  });

  describe("isValidationError", () => {
    it("should return true for a valid ValidationError object", () => {
      const validError: ValidationError = {
        Type: "Validation",
        Title: "Validation Error",
        Status: 422,
        Detail: "Invalid input",
        Instance: "some-instance-id",
        Errors: {},
      };
      expect(isValidationError(validError)).toBe(true);
    });

    it("should return false for an invalid ValidationError object", () => {
      const invalidError = {
        Title: "Validation Error",
        Status: 422,
      };
      expect(isValidationError(invalidError)).toBe(false);
    });
  });

  describe("isUnknownError", () => {
    it("should return true for a valid UnknownError object", () => {
      const validError: UnknownError = {
        type: "https://example.com/unknown-error",
        title: "Unknown Error",
        status: 500,
        detail: "An unknown error occurred.",
      };
      expect(isUnknownError(validError)).toBe(true);
    });

    it("should return false for an invalid UnknownError object", () => {
      const invalidError = {
        title: "Unknown Error",
        status: 500,
      };
      expect(isUnknownError(invalidError)).toBe(false);
    });
  });

  describe("isRegisterForm", () => {
    it("should return true for a valid RegisterForm object", () => {
      const validForm: RegisterForm = {
        userName: "testuser",
        email: "test@example.com",
        password: "password123",
        passwordConfirmation: "password123",
      };
      expect(isRegisterForm(validForm)).toBe(true);
    });

    it("should return false for an invalid RegisterForm object", () => {
      const invalidForm = {
        email: "test@example.com",
        password: "password123",
      };
      expect(isRegisterForm(invalidForm)).toBe(false);
    });
  });

  describe("isLoginForm", () => {
    it("should return true for a valid LoginForm object", () => {
      const validForm: LoginForm = {
        email: "test@example.com",
        password: "password123",
      };
      expect(isLoginForm(validForm)).toBe(true);
    });

    it("should return false for an invalid LoginForm object", () => {
      const invalidForm = {
        password: "password123",
      };
      expect(isLoginForm(invalidForm)).toBe(false);
    });
  });

  describe("isUserRegisterRequest", () => {
    it("should return true for a valid UserRegisterRequest object", () => {
      const validRequest: UserRegisterRequest = {
        userName: "testuser",
        email: "test@example.com",
        password: "password123",
        profileImage: "",
      };
      expect(isUserRegisterRequest(validRequest)).toBe(true);
    });

    it("should return false for an invalid UserRegisterRequest object", () => {
      const invalidRequest = {
        email: "test@example.com",
        password: "password123",
      };
      expect(isUserRegisterRequest(invalidRequest)).toBe(false);
    });
  });
});
