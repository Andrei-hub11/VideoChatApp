import { useRef, useState } from "react";

type CallPopupLogicProps = {
  handleAction: (value: string) => void;
};

const useCallPopupLogic = ({ handleAction }: CallPopupLogicProps) => {
  const [error, setError] = useState<string>("");
  const refInput = useRef<HTMLInputElement>(null);

  const handleSubmit = () => {
    if (!refInput.current?.value) {
      setError("Cannot be empty");
      return;
    }

    handleAction(refInput.current.value);
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
