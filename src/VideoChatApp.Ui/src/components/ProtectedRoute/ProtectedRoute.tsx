import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import {
  showAuthError,
  showUnknowError,
} from "../../utils/helpers/alertErrors";
import { isUnknownError } from "../../utils/helpers/guards";

import PageLoader from "../../animations/PageLoader/PageLoader";
import useAuth from "../../hooks/useAuth/useAuth";
import useJwtState from "../../hooks/useJwtState";
import { useTokenRenewal } from "../../hooks/useTokenRenewal";
import useUserStore from "../../hooks/useUserStore";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const navigate = useNavigate();

  const location = useLocation();

  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [_, setIsExiting] = useState<boolean>(false);

  const { fetchUserProfile } = useAuth();
  const { setUser, user } = useUserStore();

  const { token } = useJwtState();

  const [profileFetched, setProfileFetched] = useState(false);

  useTokenRenewal();

  useEffect(() => {
    if (!token || user !== null || profileFetched) {
      setIsLoading(false);
      return;
    }

    const fetchProfile = async () => {
      try {
        const result = await fetchUserProfile();

        setUser(result);
        setIsLoading(false);
      } catch (error) {
        if (isUnknownError(error) && error.status === 401) {
          showAuthError(error);
          setIsLoading(false);
          return;
        }

        if (isUnknownError(error)) {
          showUnknowError(error);
          setIsLoading(false);
          return;
        }

        setIsLoading(false);
      } finally {
        setProfileFetched((prev) => !prev);
      }
    };

    fetchProfile();
  }, [fetchUserProfile, setUser, token, user, profileFetched]);

  useEffect(() => {
    if (!isLoading && !user?.id) {
      setIsExiting(true); // Ativar animação de saída
      const timeout = setTimeout(() => {
        navigate("/login", { state: { from: location } });
      }, 500); // Atrasar a navegação para coincidir com a duração da animação

      return () => clearTimeout(timeout); // Limpar timeout
    }
  }, [isLoading, user, navigate, location]);

  if (isLoading) {
    return <PageLoader />;
  }

  return children;
};

export default ProtectedRoute;
