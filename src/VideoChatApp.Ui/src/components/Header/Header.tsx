import "./_Header.scss";

import obi from "../../assets/images/Obi-Wan Kenobi, Jedi, Star Wars.jpg";

import useJwtState from "../../hooks/useJwtState";
import useUserStore from "../../hooks/useUserStore";

function Header() {
  const { removeToken } = useJwtState();
  const { user, removeUser } = useUserStore();

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
              removeToken();
              removeUser();
            }}
          />
        </div>
      </nav>
    </header>
  );
}

export default Header;
