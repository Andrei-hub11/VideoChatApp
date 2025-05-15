import { create } from "zustand";

interface VideoChatStore {
  isNewCall: boolean;
  isCall: boolean;
  roomName: string;
  roomId: string;
  setIsNewCall: (isNewCall: boolean) => void;
  setIsCall: (isCall: boolean) => void;
  setRoomName: (roomName: string) => void;
  setRoomId: (roomId: string) => void;
  leaveCall: () => void;
  reset: () => void;
}

const useVideoChatState = create<VideoChatStore>((set) => ({
  isNewCall: false,
  isCall: false,
  roomName: "",
  roomId: "",
  setIsNewCall: (isNewCall) => set({ isNewCall }),
  setIsCall: (isCall) => set({ isCall }),
  setRoomName: (roomName) => set({ roomName }),
  setRoomId: (roomId) => set({ roomId }),
  leaveCall: () => set({ isNewCall: false, roomName: "", roomId: "" }),
  reset: () =>
    set({ isNewCall: false, isCall: false, roomName: "", roomId: "" }),
}));

export default useVideoChatState;
