import { motion } from "framer-motion";

import "./_Header.scss";

import useHeaderLogic from "./useHeaderLogic";

import obi from "../../assets/images/Obi-Wan Kenobi, Jedi, Star Wars.jpg";

function Header() {
  const { user, toggleMenu, isMenuOpen } = useHeaderLogic();

  return (
    <header className="header">
      <nav className="header__nav">
        <h1 className="header__nav-title">Home</h1>
        <div className="header__profile">
          <img
            src={
              user
                ? import.meta.env.VITE_SERVER_URL + `/${user.profileImagePath}`
                : obi
            }
            alt="imagem de perfil"
            onClick={() => {
              toggleMenu();
            }}
          />
        </div>
      </nav>
      <motion.div
        className="header__menu"
        initial={{ y: "-200%", opacity: 0 }}
        animate={{
          y: isMenuOpen ? "0%" : "-200%",
          opacity: isMenuOpen ? [0, 0, 0, 0, 1] : [0.7, 0.6, 0.4, 0, 0],
        }}
        transition={{ type: "spring", stiffness: 500, damping: 30 }}
      >
        a
      </motion.div>
    </header>
  );
}

export default Header;
