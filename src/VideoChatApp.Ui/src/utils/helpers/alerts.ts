import Swal from "sweetalert2";

export const showResetPasswordAlert = (email: string) => {
  Swal.fire({
    title: "Reset Link Sent",
    html: `
        <div class="custom-div">
          <p>A link to reset your password has been sent to ${email}.</p>
        </div>
      `,
    icon: "success",
    confirmButtonText: "OK",
    customClass: {
      popup: "custom-swal-popup",
      title: "custom-swal-title",
      confirmButton: "custom-swal-confirm",
    },
  });
};

export const showPasswordUpdateSuccessAlert = () => {
  Swal.fire({
    title: "Password Updated",
    html: `
        <div class="custom-div">
          <p>Your password has been successfully updated. You can now log in with the new password.</p>
        </div>
      `,
    icon: "success",
    confirmButtonText: "OK",
    customClass: {
      popup: "custom-swal-popup",
      title: "custom-swal-title",
      confirmButton: "custom-swal-confirm",
    },
  });
};
