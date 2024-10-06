import { useState } from "react";

import useJwtState from "../../hooks/useJwtState";
import useUserStore from "../../hooks/useUserStore";

const useHeaderLogic = () => {
  const { removeToken } = useJwtState();
  const { user, removeUser } = useUserStore();

  const [isMenuOpen, setMenuOpen] = useState<boolean>(false);

  const toggleMenu = () => {
    setMenuOpen((prevState) => !prevState);
  };

  return {
    removeToken,
    removeUser,
    user,
    isMenuOpen,
    toggleMenu,
  };
};

export default useHeaderLogic;
