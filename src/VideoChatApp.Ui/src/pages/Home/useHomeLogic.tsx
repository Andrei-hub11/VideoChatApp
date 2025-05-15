import { useState } from "react";

import { useToggle } from "@hooks/exports";
import useVideoChat from "@hooks/useChat/useVideoChatState";

export type CallState = {
  roomName: string;
  isNewCall: boolean;
  isJoinCall: boolean;
  roomId: string;
};

const useHomeLogic = () => {
  const {
    setIsNewCall,
    setIsCall,
    setRoomName,
    setRoomId,
    isNewCall,
    isCall,
    roomName,
    roomId,
  } = useVideoChat();

  const newCallPopup = useToggle();
  const joinRoomPopup = useToggle();

  const handleNewCall = async (roomName: string) => {
    try {
      /*   await createRoom(roomName); */
      setIsNewCall(true);
      setIsCall(true);
      setRoomName(roomName);
      newCallPopup.close();
    } catch (err) {
      console.error("Failed to create room:", err);
    }
  };

  const handleJoinCall = async (roomId: string) => {
    setIsCall(true);
    setIsNewCall(false);
    setRoomId(roomId);
    joinRoomPopup.close();
  };

  return {
    isNewCall,
    isCall,
    roomName,
    roomId,
    newCallPopup,
    joinRoomPopup,
    handleNewCall,
    handleJoinCall,
  };
};

export default useHomeLogic;
