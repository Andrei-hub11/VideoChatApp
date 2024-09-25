import Cookies from "cookies-js";

const useJwtState = () => {
  const token = Cookies.get("accessToken") || null;
  const refreshToken = Cookies.get("refreshToken") || null;

  // Função para salvar o access token
  const saveToken = (newToken: string) => {
    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + 1);

    Cookies.set("accessToken", newToken, {
      expires: expirationDate,
      path: "/",
    });
  };

  const removeToken = () => {
    Cookies.expire("accessToken", { path: "/" });
  };

  const saveRefreshToken = (newRefreshToken: string) => {
    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + 14);

    Cookies.set("refreshToken", newRefreshToken, {
      expires: expirationDate,
      path: "/",
    });
  };

  // Função para remover o refresh token
  const removeRefreshToken = () => {
    Cookies.expire("refreshToken", { path: "/" });
  };

  return {
    token,
    refreshToken,
    saveToken,
    removeToken,
    saveRefreshToken,
    removeRefreshToken,
  };
};

export default useJwtState;
