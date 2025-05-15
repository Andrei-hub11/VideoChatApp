import { ChangeEvent, CSSProperties, RefObject } from "react";
import * as yup from "yup";

import { UserResponse } from "./account/types";

export interface Field {
  name: string;
  label: string;
  validation: yup.StringSchema<yup.Maybe<string>>;
  type?: string;
  iconSrc: string;
  iconOptional?: string;
}

export interface UseToggleReturn {
  isOpen: boolean;
  open: () => void;
  close: () => void;
  toggle: () => void;
}

export interface RequestToJoin {
  message: string;
  createdAt: string;
}

export type FormProps = {
  fields: Field[];
  handleFormAction: (values: unknown) => Promise<boolean>;
  forgotPasswordAction?: () => void;
};

export type UpdateProfileFormProps = {
  fields: Field[];
  handleFormAction: (values: unknown) => Promise<UserResponse | null>;
  forgotPasswordAction?: () => void;
};

type ExpectedKeys = "primary" | "logout" | "formAction" | "popupAction";

export type ButtonProps = {
  variant_key: ExpectedKeys;
  onClick?: (
    event: React.MouseEvent<HTMLAnchorElement, MouseEvent>,
  ) => void | Promise<void>;
  disabled?: boolean;
};

export type Variants = Record<ExpectedKeys, () => React.ReactNode>;

export type InputIconState = Record<string, boolean>;

export type ImageProps = {
  src: string;
  alt: string;
  width: number;
  height: number;
  style?: CSSProperties;
};

export type ImageUploaderProps = {
  imagePreview: string | null;
  handleImageInputClick: () => void;
  handleTrashClick: () => void;
  handleImageChange: (event: ChangeEvent<HTMLInputElement>) => void;
  profileImageInputRef: RefObject<HTMLInputElement>;
};

export type HeaderProps = {
  currentPage: string;
};

export type HeaderMenuRedirect = "/home" | "/profile";

export type HeaderMenuProps = {
  isMenuOpen: boolean;
  handleRedirect: (path: HeaderMenuRedirect) => void;
  handleLogout: () => void;
  menuRef: React.RefObject<HTMLDivElement>;
  menuItems: MenuItem[];
};

export type MenuItem = {
  label: string;
  icon: string;
  route: HeaderMenuRedirect;
};

export type CallPopupProps = {
  isOpen: boolean;
  title: string;
  placeholder: string;
  handleAction: (value: string) => void;
  handleClose: () => void;
};

export type GridVideoChatProps = {
  roomName: string;
  roomId: string;
  isNewCall: boolean;
};

export type Participant = {
  id: string;
  peerId: string;
  roomId: string;
  stream: MediaStream | null;
};

export type RoomMember = {
  memberId: string;
  roomId: string;
  userId: string;
  peerId: string;
  role: string;
};

export type JoinRoomRequest = {
  requesterName: string;
  requesterId: string;
};

export type Room = {
  id: string;
  roomName: string;
  members: RoomMember[];
};

export type NewMessage = {
  roomId: string;
  content: string;
  memberId: string;
};
