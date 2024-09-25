import "./_Header.scss";

import obi from "../../assets/images/Obi-Wan Kenobi, Jedi, Star Wars.jpg";

function Header() {
  return (
    <header className="header">
      <nav className="header__nav">
        <h1 className="header__nav-title">Home</h1>
        <div className="header__profile">
          <img src={obi} alt="imagem de perfil" />
        </div>
      </nav>
    </header>
  );
}

export default Header;
