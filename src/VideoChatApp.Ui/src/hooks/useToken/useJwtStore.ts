import Cookies from "js-cookie";
import { create } from "zustand";

interface JwtState {
  token: string | null;
  refreshToken: string | null;
  accessTokenExpirationDate: Date | null;
  refreshTokenExpirationDate: Date | null;
  saveToken: (newToken: string) => void;
  removeToken: () => void;
  saveRefreshToken: (newRefreshToken: string) => void;
  removeRefreshToken: () => void;
  getAccessTokenExpirationDate: () => Date | null;
  getRefreshTokenExpirationDate: () => Date | null;
}

const useJwtStore = create<JwtState>((set) => ({
  token: Cookies.get("accessToken") || null,
  refreshToken: Cookies.get("refreshToken") || null,
  accessTokenExpirationDate: Cookies.get("accessTokenExpirationDate")
    ? new Date(Cookies.get("accessTokenExpirationDate")!)
    : null,
  refreshTokenExpirationDate: Cookies.get("refreshTokenExpirationDate")
    ? new Date(Cookies.get("refreshTokenExpirationDate")!)
    : null,

  saveToken: (newToken: string) => {
    const expirationDate = new Date();
    expirationDate.setMinutes(expirationDate.getMinutes() + 15);

    Cookies.set("accessToken", newToken, {
      expires: expirationDate,
      path: "/",
      sameSite: "lax",
      secure: true,
    });

    Cookies.set("accessTokenExpirationDate", expirationDate.toISOString(), {
      path: "/",
      sameSite: "lax",
    });

    set({ token: newToken, accessTokenExpirationDate: expirationDate });
  },

  removeToken: () => {
    Cookies.remove("accessToken", { path: "/", sameSite: "lax", secure: true });
    set({ token: null, accessTokenExpirationDate: null });
  },

  saveRefreshToken: (newRefreshToken: string) => {
    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + 14);

    Cookies.set("refreshToken", newRefreshToken, {
      expires: expirationDate,
      path: "/",
      sameSite: "lax",
      secure: true,
    });

    Cookies.set("refreshTokenExpirationDate", expirationDate.toISOString(), {
      path: "/",
      sameSite: "lax",
    });

    set({
      refreshToken: newRefreshToken,
      refreshTokenExpirationDate: expirationDate,
    });
  },

  removeRefreshToken: () => {
    Cookies.remove("refreshToken", {
      path: "/",
      sameSite: "lax",
      secure: true,
    });
    Cookies.remove("refreshTokenExpirationDate", { path: "/" });
    set({ refreshToken: null, refreshTokenExpirationDate: null });
  },

  getAccessTokenExpirationDate: () => {
    return Cookies.get("accessTokenExpirationDate")
      ? new Date(Cookies.get("accessTokenExpirationDate")!)
      : null;
  },

  getRefreshTokenExpirationDate: () => {
    return Cookies.get("refreshTokenExpirationDate")
      ? new Date(Cookies.get("refreshTokenExpirationDate")!)
      : null;
  },
}));

export default useJwtStore;
