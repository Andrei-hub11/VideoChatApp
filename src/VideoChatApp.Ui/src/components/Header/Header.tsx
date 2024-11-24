import "./_Header.scss";

import useHeaderLogic from "./useHeaderLogic";

import { Image } from "@components/exports";

import CloseIcon from "../../assets/icons/gg_close.svg";
import SuccessIcon from "../../assets/icons/mdi_success.svg";
import randomPic from "../../assets/images/Batmans pic.jpeg";

import { HeaderProps } from "../../contracts";
import HeaderMenu from "../HeaderMenu/HeaderMenu";

function Header({ currentPage, requestsToJoin }: HeaderProps) {
  const {
    user,
    toggleMenu,
    isMenuOpen,
    handleLogout,
    handleRedirect,
    menuItems,
    menuRef,
  } = useHeaderLogic();

  return (
    <header className="header">
      <nav className="header__nav">
        <h1 className="header__nav-title">{currentPage}</h1>
        <div className="header__container">
          {requestsToJoin.length > 0 && (
            <div className="header__message">
              <p>{requestsToJoin[0].message}</p>
              <div className="header__actions">
                <div className="header__action header__action--reject">
                  <Image
                    {...{
                      props: {
                        src: CloseIcon,
                        alt: "close icon",
                        height: 24,
                        width: 24,
                      },
                    }}
                  />
                </div>
                <div className="header__action header__action--accept">
                  <Image
                    {...{
                      props: {
                        src: SuccessIcon,
                        alt: "success icon",
                        height: 24,
                        width: 24,
                      },
                    }}
                  />
                </div>
              </div>
            </div>
          )}
          <div className="header__profile">
            <img
              id="profile-image"
              src={
                user?.profileImagePath && user.profileImagePath !== ""
                  ? import.meta.env.VITE_SERVER_URL +
                    `/${user.profileImagePath}`
                  : randomPic
              }
              alt="profile image"
              onClick={() => {
                toggleMenu();
              }}
            />
          </div>
        </div>
        {/*  <div className="header__profile">
          <img
            id="profile-image"
            src={
              user?.profileImagePath && user.profileImagePath !== ""
                ? import.meta.env.VITE_SERVER_URL + `/${user.profileImagePath}`
                : randomPic
            }
            alt="imagem de perfil"
            onClick={() => {
              toggleMenu();
            }}
          />
        </div> */}
      </nav>
      <HeaderMenu
        {...{ isMenuOpen, handleRedirect, handleLogout, menuRef, menuItems }}
      />
    </header>
  );
}

export default Header;
