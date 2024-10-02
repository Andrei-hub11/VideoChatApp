import Swal from "sweetalert2";

import {
  NotFoundError,
  UnknownError,
  ValidationError,
} from "../../types/http/types";

export const showValidationErrors = (apiError: ValidationError) => {
  const errorMessages = Object.entries(apiError.Errors)
    .map(([key, errors]) => {
      return `
      <div>
        <strong>${key}:</strong>
        <ul>
          ${errors.map((err) => `<li>${err.Description}</li>`).join("")}
        </ul>
      </div>
    `;
    })
    .join("");

  Swal.fire({
    icon: "warning",
    title: apiError.Title,
    html: `<div class="custom-div">
           <ul>
             ${errorMessages}
           </ul>
         </div>`,
    customClass: {
      container: "custom-container",
      popup: "custom-swal-popup",
      title: "custom-swal-title",
      confirmButton: "custom-swal-confirm",
    },
  });
};

export const showUserNotFoundError = (apiError: NotFoundError) => {
  Swal.fire({
    title: apiError.title, // Título do erro
    html: `
        <div class="custom-div">
          <p>${apiError.detail}</p>
        </div>
      `,
    icon: "error",
    confirmButtonText: "OK",
    customClass: {
      popup: "custom-swal-popup",
      title: "custom-swal-title",
      confirmButton: "custom-swal-confirm",
    },
  });
};

export const showAuthError = (apiError: UnknownError) => {
  Swal.fire({
    title: apiError.title,
    html: `
        <div class="custom-div">
          <p>${apiError.detail}</p>
        </div>
      `,
    icon: "warning",
    confirmButtonText: "OK",
    customClass: {
      popup: "custom-swal-popup",
      title: "custom-swal-title",
      confirmButton: "custom-swal-confirm",
    },
  });
};

export const showUnknowError = (apiError: UnknownError) => {
  Swal.fire({
    title: apiError.title,
    html: `
          <div class="custom-div">
            <p>${apiError.detail}</p>
          </div>
        `,
    icon: "warning",
    confirmButtonText: "OK",
    customClass: {
      popup: "custom-swal-popup",
      title: "custom-swal-title",
      confirmButton: "custom-swal-confirm",
    },
    scrollbarPadding: false,
  });
};
