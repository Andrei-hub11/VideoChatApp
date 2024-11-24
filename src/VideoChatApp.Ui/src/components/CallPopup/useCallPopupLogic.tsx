import { useRef, useState } from "react";

const useCallPopupLogic = () => {
  const [error, setError] = useState<string>("");
  const refInput = useRef<HTMLInputElement>(null);

  const handleSubmit = () => {
    if (!refInput.current?.value) {
      setError("Cannot be empty");
      return;
    }

    console.log("submit");
  };

  const handleChange = () => {
    if (!refInput.current) return;

    if (refInput.current.value.trim() === "") {
      setError("Cannot be empty");
      return;
    }

    if (refInput.current.value) {
      setError("");
    }
  };

  return {
    handleSubmit,
    handleChange,
    error,
    refInput,
  };
};

export default useCallPopupLogic;
