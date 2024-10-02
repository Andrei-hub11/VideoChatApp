import "@testing-library/jest-dom";
import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, it, Mock, vi } from "vitest";

import ProtectedRoute from "../../components/ProtectedRoute/ProtectedRoute";

import useAuth from "../../hooks/useAuth/useAuth";
import useJwtState from "../../hooks/useJwtState";
import useUserStore from "../../hooks/useUserStore";

// Mocking hooks
vi.mock("../../hooks/useAuth/useAuth");
vi.mock("../../hooks/useJwtState");
vi.mock("../../hooks/useUserStore");

const MockedComponent = () => <div>Protected Content</div>;

describe("ProtectedRoute", () => {
  it("must render the loader while the profile is being loaded", () => {
    // Mocking fetchUserProfile
    const fetchUserProfile = vi
      .fn()
      .mockImplementation(() => new Promise(() => {}));
    (useAuth as Mock).mockReturnValue({ fetchUserProfile });
    (useUserStore as unknown as Mock).mockReturnValue({
      user: null,
      setUser: vi.fn(),
    });
    (useJwtState as Mock).mockReturnValue({ token: "mock-token" });

    render(
      <MemoryRouter>
        <ProtectedRoute>
          <MockedComponent />
        </ProtectedRoute>
      </MemoryRouter>,
    );

    expect(screen.getByText(/loading, please wait.../i)).toBeInTheDocument();
  });

  it("should redirect to /login if user is not authenticated", async () => {
    const fetchUserProfile = vi.fn().mockResolvedValue(null);
    (useAuth as Mock).mockReturnValue({ fetchUserProfile });
    (useUserStore as unknown as Mock).mockReturnValue({
      user: null,
      setUser: vi.fn(),
    });
    (useJwtState as Mock).mockReturnValue({ token: "mock-token" });

    render(
      <MemoryRouter initialEntries={["/protected"]}>
        <Routes>
          <Route path="/login" element={<div>Login Page</div>} />
          <Route
            path="/protected"
            element={
              <ProtectedRoute>
                <MockedComponent />
              </ProtectedRoute>
            }
          />
        </Routes>
      </MemoryRouter>,
    );

    await waitFor(() => {
      expect(screen.getByText("Login Page")).toBeInTheDocument();
    });
  });

  it("should render child component if user is authenticated", async () => {
    const mockUser = { id: "1", name: "Test User" };
    const fetchUserProfile = vi.fn().mockResolvedValue(mockUser);
    (useAuth as Mock).mockReturnValue({ fetchUserProfile });
    (useUserStore as unknown as Mock).mockReturnValue({
      user: mockUser,
      setUser: vi.fn(),
    });
    (useJwtState as Mock).mockReturnValue({ token: "mock-token" });

    render(
      <MemoryRouter>
        <ProtectedRoute>
          <MockedComponent />
        </ProtectedRoute>
      </MemoryRouter>,
    );

    await waitFor(() => {
      expect(screen.getByText("Protected Content")).toBeInTheDocument();
    });
  });
});