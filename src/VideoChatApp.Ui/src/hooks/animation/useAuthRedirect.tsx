import { useEffect } from "react";
import { useLocation } from "react-router-dom";

import useAuthTransitionStore from "./useAuthTransitionStore";

// Ajuste o caminho para a store correta

function useAuthRedirect() {
  const location = useLocation();
  const setRedirectingToAuth = useAuthTransitionStore(
    (state) => state.setRedirectingToAuth,
  );

  useEffect(() => {
    // Atualiza o estado de redirecionamento para autenticação com base na rota
    setRedirectingToAuth(
      location.pathname === "/login" || location.pathname === "/register",
    );
  }, [location.pathname, setRedirectingToAuth]);
}

export default useAuthRedirect;
