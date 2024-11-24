import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";

import useAuthRedirect from "../../hooks/animation/useAuthRedirect";
import useAuthTransitionStore from "../../hooks/animation/useAuthTransitionStore";
import useJwtState from "../../hooks/useToken/useJwtStore";
import { useUserStore } from "@hooks/exports";

import HomeIcon from "../../assets/icons/ic_outline-home.svg";
import ProfileIcon from "../../assets/icons/mdi_user-outline--black.svg";

import { HeaderMenuRedirect, MenuItem } from "../../contracts";

const menuItems: MenuItem[] = [
  {
    label: "Home",
    icon: HomeIcon,
    route: "/home",
  },
  {
    label: "Profile",
    icon: ProfileIcon,
    route: "/profile",
  },
];

const useHeaderLogic = () => {
  const { setRedirectingToAuth } = useAuthTransitionStore();

  useAuthRedirect();

  const navigate = useNavigate();

  const { removeToken } = useJwtState();
  const { user } = useUserStore();

  const [isMenuOpen, setMenuOpen] = useState<boolean>(false);

  const menuRef = useRef<HTMLDivElement>(null);

  const toggleMenu = useCallback(() => {
    setMenuOpen((prevState) => !prevState);
  }, []);

  const handleLogout = useCallback(() => {
    setRedirectingToAuth(true);
    removeToken();
  }, [removeToken, setRedirectingToAuth]);

  const handleMenuClose = useCallback(
    (event: MouseEvent) => {
      const clickedElement = event.target as HTMLElement;

      const isClickedOnProfileImage = clickedElement.id === "profile-image";

      if (
        !isClickedOnProfileImage &&
        menuRef.current &&
        !menuRef.current.contains(event.target as HTMLElement) &&
        isMenuOpen
      ) {
        setMenuOpen(false);
      }
    },
    [isMenuOpen],
  );

  useEffect(() => {
    document.addEventListener("mousedown", handleMenuClose);
    return () => {
      document.removeEventListener("mousedown", handleMenuClose);
    };
  }, [handleMenuClose]);

  const handleRedirect = useCallback(
    (redirect: HeaderMenuRedirect) => {
      navigate(redirect);
    },
    [navigate],
  );

  return {
    user,
    isMenuOpen,
    toggleMenu,
    handleLogout,
    menuRef,
    menuItems,
    handleRedirect,
  };
};

export default useHeaderLogic;
