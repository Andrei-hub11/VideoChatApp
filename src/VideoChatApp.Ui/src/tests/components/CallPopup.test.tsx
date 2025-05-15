import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import CallPopup from "../../components/CallPopup/CallPopup";

describe("CallPopup Component", () => {
  const defaultProps = {
    handleClose: vi.fn(),
    handleAction: vi.fn(),
    isOpen: true,
    title: "Test Title",
    placeholder: "Test Placeholder",
  };

  it("renders correctly when open", () => {
    render(<CallPopup {...defaultProps} />);

    expect(screen.getByText("Test Title")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("Test Placeholder")).toBeInTheDocument();
    expect(screen.getByText("Enviar")).toBeInTheDocument();
    expect(screen.getByAltText("close-icon")).toBeInTheDocument();
  });

  it("does not display when isOpen is false", () => {
    render(<CallPopup {...defaultProps} isOpen={false} />);

    const popup = screen.getByText("Test Title").closest(".call-popup");
    expect(popup).not.toHaveClass("call-popup--active");
  });

  it("calls handleClose when close icon is clicked", () => {
    render(<CallPopup {...defaultProps} />);

    fireEvent.click(screen.getByAltText("close-icon"));
    expect(defaultProps.handleClose).toHaveBeenCalledTimes(1);
  });

  it("shows error message when trying to submit empty input", () => {
    render(<CallPopup {...defaultProps} />);

    fireEvent.click(screen.getByText("Enviar"));
    expect(screen.getByText("Cannot be empty")).toBeInTheDocument();
    expect(defaultProps.handleAction).not.toHaveBeenCalled();
  });

  it("calls handleAction with input value when submitting valid input", () => {
    render(<CallPopup {...defaultProps} />);

    const input = screen.getByPlaceholderText("Test Placeholder");
    fireEvent.change(input, { target: { value: "test value" } });
    fireEvent.click(screen.getByText("Enviar"));

    expect(defaultProps.handleAction).toHaveBeenCalledWith("test value");
  });

  it("clears error message when input is valid", () => {
    render(<CallPopup {...defaultProps} />);

    // First trigger error
    fireEvent.click(screen.getByText("Enviar"));
    expect(screen.getByText("Cannot be empty")).toBeInTheDocument();

    // Then add valid input
    const input = screen.getByPlaceholderText("Test Placeholder");
    fireEvent.change(input, { target: { value: "test value" } });

    // Error should be gone
    expect(screen.queryByText("Cannot be empty")).not.toBeInTheDocument();
  });
});
