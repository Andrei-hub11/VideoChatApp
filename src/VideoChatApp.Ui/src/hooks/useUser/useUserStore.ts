import { create } from "zustand";

import { UserResponse } from "@contracts/httpResponse/types";

interface UserState {
  user: UserResponse | null;
  setUser: (user: UserResponse) => void;
  removeUser: () => void;
}

const useUserStore = create<UserState>((set) => ({
  user: null,
  setUser: (user) => set({ user: user }),
  removeUser: () => set({ user: null }),
}));

export default useUserStore;
