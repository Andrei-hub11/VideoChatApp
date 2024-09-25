import * as yup from "yup";

export interface Field {
  name: string;
  label: string;
  validation: yup.StringSchema<string>;
  type?: string;
  iconSrc?: string;
}

export type FormProps = {
  fields: Field[];
  handleFormAction: (values: unknown) => Promise<boolean>;
};

type ExpectedKeys = "primary" | "secondary" | "formAction";

export type ButtonProps = {
  variant_key: ExpectedKeys;
  onClick?: (
    event: React.MouseEvent<HTMLAnchorElement, MouseEvent>,
  ) => void | Promise<void>;
  disabled?: boolean;
};

export type Variants = Record<string, () => React.ReactNode>;
