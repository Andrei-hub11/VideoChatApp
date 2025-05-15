import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import {
  useAuthTransitionStore,
  useAuth,
  useJwtState,
  useTokenRenewal,
  useUserStore,
} from "@hooks/exports";

import { isUnknownError, showAuthError, showUnknowError } from "@utils/exports";

import PageLoader from "../../animations/PageLoader/PageLoader";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const navigate = useNavigate();
  const location = useLocation();

  const [isLoading, setIsLoading] = useState<boolean>(true);

  const { fetchUserProfile } = useAuth();
  const { setUser, user } = useUserStore();
  const { setRedirectingToAuth } = useAuthTransitionStore();

  const { token, refreshToken } = useJwtState();

  useTokenRenewal();

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        if (token && !user) {
          const result = await fetchUserProfile();
          setUser(result);
        }
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
      }
      setIsLoading(false);
    };

    fetchProfile();
  }, [token, user, fetchUserProfile, setUser]);

  useEffect(() => {
    if (!isLoading && !token && !refreshToken) {
      const timeout = setTimeout(() => {
        setRedirectingToAuth(true);
        navigate("/login", { state: { from: location } });
      }, 500);

      return () => clearTimeout(timeout);
    }
  }, [
    isLoading,
    token,
    refreshToken,
    setRedirectingToAuth,
    navigate,
    location,
  ]);

  if (isLoading) {
    return <PageLoader />;
  }

  return children;
};

export default ProtectedRoute;
