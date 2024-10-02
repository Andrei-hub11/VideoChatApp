import Cookies from "js-cookie";

const useJwtState = () => {
  const token = Cookies.get("accessToken") || null;
  const refreshToken = Cookies.get("refreshToken") || null;

  const saveToken = (newToken: string) => {
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
  };

  const removeToken = () => {
    Cookies.remove("accessToken", { path: "/", sameSite: "lax", secure: true });
  };

  const saveRefreshToken = (newRefreshToken: string) => {
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
  };

  const removeRefreshToken = () => {
    Cookies.remove("refreshToken", {
      path: "/",
      sameSite: "Lax",
      secure: true,
    });
    Cookies.remove("refreshTokenExpirationDate", { path: "/" });
  };

  const getAccessTokenExpirationDate = () => {
    const expirationDateString = Cookies.get("accessTokenExpirationDate");
    if (!expirationDateString) return null;

    return new Date(expirationDateString);
  };

  const getRefreshTokenExpirationDate = () => {
    const expirationDateString = Cookies.get("refreshTokenExpirationDate");
    if (!expirationDateString) return null;

    return new Date(expirationDateString);
  };

  return {
    token,
    refreshToken,
    saveToken,
    removeToken,
    saveRefreshToken,
    removeRefreshToken,
    getAccessTokenExpirationDate,
    getRefreshTokenExpirationDate,
  };
};

export default useJwtState;
