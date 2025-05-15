import { renderHook, act } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import useCallPopupLogic from "../../components/CallPopup/useCallPopupLogic";

describe("useCallPopupLogic", () => {
  const handleAction = vi.fn();

  it("initializes with empty error state", () => {
    const { result } = renderHook(() => useCallPopupLogic({ handleAction }));

    expect(result.current.error).toBe("");
  });

  it("sets error when submitting empty value", () => {
    const { result } = renderHook(() => useCallPopupLogic({ handleAction }));

    act(() => {
      result.current.handleSubmit();
    });

    expect(result.current.error).toBe("Cannot be empty");
    expect(handleAction).not.toHaveBeenCalled();
  });

  it("calls handleAction when submitting valid value", () => {
    const { result } = renderHook(() => useCallPopupLogic({ handleAction }));

    // Mock the ref value
    if (result.current.refInput.current) {
      result.current.refInput.current.value = "test value";
    }

    act(() => {
      result.current.handleSubmit();
    });

    expect(handleAction).toHaveBeenCalledWith("test value");
  });

  it("sets error on empty input change", () => {
    const { result } = renderHook(() => useCallPopupLogic({ handleAction }));

    // Mock the ref value
    if (result.current.refInput.current) {
      result.current.refInput.current.value = "";
    }

    act(() => {
      result.current.handleChange();
    });

    expect(result.current.error).toBe("Cannot be empty");
  });

  it("clears error on valid input change", () => {
    const { result } = renderHook(() => useCallPopupLogic({ handleAction }));

    // First set an error
    act(() => {
      result.current.handleSubmit();
    });
    expect(result.current.error).toBe("Cannot be empty");

    // Then provide valid input
    if (result.current.refInput.current) {
      result.current.refInput.current.value = "test value";
    }

    act(() => {
      result.current.handleChange();
    });

    expect(result.current.error).toBe("");
  });
});
