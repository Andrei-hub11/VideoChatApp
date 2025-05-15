import { create } from "zustand";

import { JoinRoomRequest } from "@contracts/index";

type RequestsToJoinState = {
  requestsToJoin: JoinRoomRequest[];
  isAnswerToJoin: boolean;
  isAcceptingRequest: boolean;
  setRequestsToJoin: (newRequestsToJoin: JoinRoomRequest) => void;
  setIsAnswerToJoin: (isAnswerToJoin: boolean) => void;
  setIsAcceptingRequest: (isAcceptingRequest: boolean) => void;
  removeRequestToJoin: (requestId: string) => void;
};

const useRequestsToJoin = create<RequestsToJoinState>((set) => ({
  requestsToJoin: [],
  isAnswerToJoin: false,
  isAcceptingRequest: false,
  setRequestsToJoin: (newRequestsToJoin: JoinRoomRequest) =>
    set((state) => ({
      ...state,
      requestsToJoin: [...state.requestsToJoin, newRequestsToJoin],
    })),
  setIsAnswerToJoin: (isAnswerToJoin: boolean) =>
    set((state) => ({
      ...state,
      isAnswerToJoin,
    })),
  setIsAcceptingRequest: (isAcceptingRequest: boolean) =>
    set((state) => ({
      ...state,
      isAcceptingRequest,
    })),
  removeRequestToJoin: (requestId: string) =>
    set((state) => ({
      ...state,
      requestsToJoin: state.requestsToJoin.filter(
        (request) => request.requesterId !== requestId,
      ),
    })),
}));

export default useRequestsToJoin;
