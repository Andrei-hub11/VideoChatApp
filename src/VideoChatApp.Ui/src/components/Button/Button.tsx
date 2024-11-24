import React from "react";

import "./_ButtonStyle.scss";

import { ButtonProps, Variants } from "../../contracts";

interface BtnProps {
  btn: ButtonProps;
  children: React.ReactNode;
}

function Button({ btn, children }: BtnProps) {
  const { variant_key, onClick, disabled } = btn;

  const button: Variants = {
    primary: () => (
      <a role="button" onClick={onClick} className="btn primary">
        {children}
      </a>
    ),
    logout: () => (
      <a role="button" onClick={onClick} className="btn btn--logout">
        {children}
      </a>
    ),
    formAction: () => (
      <a
        role="button"
        onClick={disabled ? (e) => e.preventDefault() : onClick}
        className="btn primary btn--action"
      >
        {children}
      </a>
    ),
    popupAction: () => (
      <a role="button" onClick={onClick} className="btn primary btn--popup">
        {children}
      </a>
    ),
  };

  return button[variant_key]();
}

export default Button;
