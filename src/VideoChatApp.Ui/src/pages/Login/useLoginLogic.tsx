import { useNavigate } from "react-router-dom";

const useLoginLogic = () => {
  const navigate = useNavigate();

  const register = async (values: unknown): Promise<boolean> => {
    return true;
  };

  const handleRedirect = () => {
    navigate("/register");
  };

  return {
    register,
    handleRedirect,
  };
};

export default useLoginLogic;
