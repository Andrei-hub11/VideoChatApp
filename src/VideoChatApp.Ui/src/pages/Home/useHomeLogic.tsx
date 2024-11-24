import { useState } from "react";

import { useToggle } from "@hooks/exports";

import { RequestToJoin } from "@contracts/index";

const useHomeLogic = () => {
  const [requestsToJoin, setRequestsToJoin] = useState<RequestToJoin[]>([]);
  const newCallPopup = useToggle();
  const joinRoomPopup = useToggle();

  return {
    newCallPopup,
    joinRoomPopup,
    requestsToJoin,
  };
};

export default useHomeLogic;
