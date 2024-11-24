import "./_CallPopupStyle.scss";

import useCallPopupLogic from "./useCallPopupLogic";

import { Button } from "@components/exports";

import { CallPopupProps } from "@contracts/index";

import CloseIcon from "../../assets/icons/ic_round-close.svg";

function CallPopup({
  handleClose,
  isOpen,
  title,
  placeholder,
}: CallPopupProps) {
  const { handleSubmit, handleChange, error, refInput } = useCallPopupLogic();

  return (
    <div className={`call-popup ${isOpen ? "call-popup--active" : ""}`}>
      <div className="call-popup__icon">
        <img src={CloseIcon} alt="close-icon" onClick={handleClose} />
      </div>
      <h1 className="call-popup__title">{title}</h1>
      <div className="call-popup__container">
        <input
          className="call-popup__input"
          type="text"
          placeholder={placeholder}
          ref={refInput}
          onChange={handleChange}
        />
        <small className="call-popup__msg call-popup__msg--variant">
          {error}
        </small>
      </div>
      <div className="call-popup__action">
        <Button btn={{ variant_key: "popupAction", onClick: handleSubmit }}>
          Enviar
        </Button>
      </div>
    </div>
  );
}

export default CallPopup;
