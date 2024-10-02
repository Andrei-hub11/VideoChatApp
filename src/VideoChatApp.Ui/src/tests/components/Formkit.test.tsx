import "@testing-library/jest-dom";
import { render, fireEvent, screen, waitFor } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import * as yup from "yup";

import Formkit from "../../components/Formkit/Formkit";

import { Field } from "../../types";

export const loginForm: Field[] = [
  {
    name: "email",
    label: "Adicione um email",
    validation: yup.string().required("O nome é obrigatório"),
    iconSrc: "path/to/emailIcon",
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
    iconSrc: "path/to/passwordIcon",
  },
];

describe("Formkit Component", () => {
  it("should render the fields correctly", () => {
    const handleFormAction = vi.fn().mockResolvedValue(true);

    render(<Formkit fields={loginForm} handleFormAction={handleFormAction} />);

    expect(
      screen.getByPlaceholderText("Adicione um email"),
    ).toBeInTheDocument();
    expect(
      screen.getByPlaceholderText("Diga-me uma senha"),
    ).toBeInTheDocument();
  });

  it("should display validation error messages correctly", async () => {
    const handleFormAction = vi.fn().mockResolvedValue(true);

    render(<Formkit fields={loginForm} handleFormAction={handleFormAction} />);

    const submitButton = screen.getByText("Enviar");
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText("O nome é obrigatório")).toBeInTheDocument();
      expect(screen.getByText("A senha é obrigatória")).toBeInTheDocument();
      expect(screen.getByText("O nome é obrigatório")).toHaveClass(
        "form__msg form__msg--variant",
      );
      expect(screen.getByText("A senha é obrigatória")).toHaveClass(
        "form__msg form__msg--variant",
      );
    });
  });

  it("must call the handleFormAction function correctly on submission", async () => {
    const handleFormAction = vi.fn().mockResolvedValue(true);

    render(<Formkit fields={loginForm} handleFormAction={handleFormAction} />);

    const emailInput = screen.getByPlaceholderText("Adicione um email");
    const passwordInput = screen.getByPlaceholderText("Diga-me uma senha");

    fireEvent.change(emailInput, { target: { value: "test@example.com" } });
    fireEvent.change(passwordInput, { target: { value: "Password!#1" } });

    const submitButton = screen.getByText("Enviar");
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(handleFormAction).toHaveBeenCalledWith({
        email: "test@example.com",
        password: "Password!#1",
      });
    });
  });

  it("must reset the form after successful submission", async () => {
    const handleFormAction = vi.fn().mockResolvedValue(true);

    render(<Formkit fields={loginForm} handleFormAction={handleFormAction} />);

    const emailInput = screen.getByPlaceholderText(
      "Adicione um email",
    ) as HTMLInputElement;
    const passwordInput = screen.getByPlaceholderText(
      "Diga-me uma senha",
    ) as HTMLInputElement;

    fireEvent.change(emailInput, { target: { value: "test@example.com" } });
    fireEvent.change(passwordInput, { target: { value: "Password!#1" } });

    const submitButton = screen.getByText("Enviar");
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(handleFormAction).toHaveBeenCalled();
      expect(emailInput.value).toBe("");
      expect(passwordInput.value).toBe("");
    });
  });

  it("should display the loading state while the form is being submitted", async () => {
    const handleFormAction = vi.fn().mockResolvedValue(true);

    render(<Formkit fields={loginForm} handleFormAction={handleFormAction} />);

    const submitButton = screen.getByText("Enviar");

    fireEvent.click(submitButton);

    await waitFor(() => {
      const loader = screen.getByRole("button");
      expect(loader).toBeInTheDocument();
    });
  });
});

export default Formkit;
