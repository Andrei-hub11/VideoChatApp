import { motion } from "framer-motion";
import { useLocation } from "react-router-dom";

import "./_HeaderMenu.scss";

import LogoutIcon from "../../assets/icons/material-symbols_logout.svg";

import { HeaderMenuProps } from "../../contracts";
import Button from "../Button/Button";

function HeaderMenu({
  isMenuOpen,
  handleRedirect,
  handleLogout,
  menuRef,
  menuItems,
}: HeaderMenuProps) {
  const location = useLocation();

  return (
    <motion.div
      className="header__menu"
      ref={menuRef}
      initial={{ y: "-200%", opacity: 0 }}
      animate={{
        y: isMenuOpen ? "0%" : "-200%",
        opacity: isMenuOpen ? [0, 0, 0, 0, 1] : [0.7, 0.6, 0.4, 0, 0],
      }}
      transition={{ type: "spring", stiffness: 500, damping: 30 }}
    >
      <div className="header__menu-content">
        {menuItems.map((item, index: number) => (
          <div
            key={index}
            className={`header__link ${location.pathname === item.route ? "header__link--variant" : ""}`}
            onClick={() => handleRedirect(item.route)}
          >
            <img
              src={item.icon}
              alt={`${item.label} icon`}
              height={24}
              width={24}
            />
            <p className="header__link-p">{item.label}</p>
          </div>
        ))}
      </div>
      <div className="header__menu-logout">
        <Button {...{ btn: { variant_key: "logout", onClick: handleLogout } }}>
          <img src={LogoutIcon} alt="logout icon" height={24} width={24} />
          Logout
        </Button>
      </div>
    </motion.div>
  );
}

export default HeaderMenu;
