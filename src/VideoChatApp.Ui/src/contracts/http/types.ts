export interface NotFoundError {
  type: string;
  title: string;
  status: number;
  detail: string;
  instance: string;
}

export interface ValidationErrorDetail {
  Code: string;
  Description: string;
}

export interface ValidationErrors {
  [field: string]: ValidationErrorDetail[];
}

export interface ValidationError {
  Type: string;
  Title: string;
  Status: number;
  Detail: string;
  Instance: string;
  Errors: ValidationErrors;
}

export interface UnknownError {
  status: number;
  type: string;
  title: string;
  detail: string;
  instance?: string;
}

export type ErrorTypes = UnknownError | ValidationError | NotFoundError;
