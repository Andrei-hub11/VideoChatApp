import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import Button from "../../components/Button/Button";

describe("Button component", () => {
  it("renders a primary button with correct text and behavior", () => {
    const handleClick = vi.fn(); // Mock function for click event
    render(
      <Button
        btn={{ variant_key: "primary", onClick: handleClick }}
        children="Click Me"
      />,
    );

    const buttonElement = screen.getByRole("button", { name: /click me/i });
    expect(buttonElement).toBeInTheDocument();
    expect(buttonElement).toHaveClass("btn primary");

    fireEvent.click(buttonElement);
    expect(handleClick).toHaveBeenCalled(); // Check if onClick is called
  });

  it("renders a formAction button and prevents click when disabled", () => {
    const handleClick = vi.fn();
    render(
      <Button
        btn={{
          variant_key: "formAction",
          onClick: handleClick,
          disabled: true,
        }}
        children="Submit"
      />,
    );

    const buttonElement = screen.getByRole("button", { name: /submit/i });
    expect(buttonElement).toBeInTheDocument();
    expect(buttonElement).toHaveClass("btn primary btn--action");

    fireEvent.click(buttonElement);
    expect(handleClick).not.toHaveBeenCalled(); // Check that onClick is not called due to disabled state
  });

  it("renders a formAction button and calls onClick when not disabled", () => {
    const handleClick = vi.fn();
    render(
      <Button
        btn={{
          variant_key: "formAction",
          onClick: handleClick,
          disabled: false,
        }}
        children="Submit"
      />,
    );

    const buttonElement = screen.getByRole("button", { name: /submit/i });
    expect(buttonElement).toBeInTheDocument();

    fireEvent.click(buttonElement);
    expect(handleClick).toHaveBeenCalled(); // Check if onClick is called
  });
});
