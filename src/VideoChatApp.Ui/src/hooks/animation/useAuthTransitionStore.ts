import { create } from "zustand";

interface AuthTransitionState {
  isRedirectingToAuth: boolean;
  setRedirectingToAuth: (value: boolean) => void;
}

// Cria um store Zustand
const useAuthTransitionStore = create<AuthTransitionState>((set) => ({
  isRedirectingToAuth: false,
  setRedirectingToAuth: (value: boolean) => set({ isRedirectingToAuth: value }),
}));

export default useAuthTransitionStore;
