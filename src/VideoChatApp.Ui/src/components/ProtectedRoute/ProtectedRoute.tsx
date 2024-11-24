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
  const [_, setIsExiting] = useState<boolean>(false);

  const { fetchUserProfile } = useAuth();
  const { setUser, user } = useUserStore();
  const { setRedirectingToAuth } = useAuthTransitionStore();

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
    if (!isLoading && !token) {
      setIsExiting(true);

      const timeout = setTimeout(() => {
        //enable the page exit animation
        setRedirectingToAuth(true);

        navigate("/login", { state: { from: location } });
      }, 500); // Delay navigation to match animation duration

      return () => clearTimeout(timeout);
    }
  }, [isLoading, token, navigate, location, setRedirectingToAuth]);

  if (isLoading) {
    return <PageLoader />;
  }

  return children;
};

export default ProtectedRoute;
