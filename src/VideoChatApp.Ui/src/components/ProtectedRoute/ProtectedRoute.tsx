import React, { useEffect, useState } from "react";
import { Navigate, useLocation } from "react-router-dom";

import useAuth from "../../hooks/useAuth/useAuth";
import useJwtState from "../../hooks/useJwtState";
import { useUserStore } from "../../hooks/useUserStore";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const [isLoading, setIsLoading] = useState<boolean>(true); // Estado de carregamento

  const { fetchUserProfile } = useAuth();
  const { setUser, user } = useUserStore();

  const { token } = useJwtState();

  useEffect(() => {
    if (!token) {
      setIsLoading(false);
      return;
    }

    // Chama fetchUserProfile ao montar o componente
    const fetchProfile = async () => {
      const result = await fetchUserProfile();

      setUser(result);
      setIsLoading(false);
    };

    fetchProfile();
  }, [fetchUserProfile, setUser, token]);

  const location = useLocation();

  if (isLoading) {
    return;
  }

  //Se não houver um usuário autenticado, redireciona para a página de login, incluindo a localização atual.
  if (!user?.id) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }
  return children;
};

export default ProtectedRoute;
