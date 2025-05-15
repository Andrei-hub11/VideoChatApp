import { describe, it, expect, beforeEach } from "vitest";

import useVideoChatState from "../../hooks/useChat/useVideoChatState";

describe("useVideoChat", () => {
  beforeEach(() => {
    // Reset the store state before each test
    const store = useVideoChatState.getState();
    store.setIsNewCall(false);
    store.setIsCall(false);
    store.setRoomName("");
    store.setRoomId("");
  });

  it("should initialize with default values", () => {
    const state = useVideoChatState.getState();

    expect(state.isNewCall).toBe(false);
    expect(state.isCall).toBe(false);
    expect(state.roomName).toBe("");
    expect(state.roomId).toBe("");
  });

  it("should update isNewCall state", () => {
    let currentState = useVideoChatState.getState();

    useVideoChatState.subscribe((state) => {
      currentState = state;
    });

    currentState.setIsNewCall(true);
    expect(currentState.isNewCall).toBe(true);

    currentState.setIsNewCall(false);
    expect(currentState.isNewCall).toBe(false);
  });

  it("should update isCall state", () => {
    let currentState = useVideoChatState.getState();

    useVideoChatState.subscribe((state) => {
      currentState = state;
    });

    currentState.setIsCall(true);
    expect(currentState.isCall).toBe(true);

    currentState.setIsCall(false);
    expect(currentState.isCall).toBe(false);
  });

  it("should update roomName state", () => {
    let currentState = useVideoChatState.getState();

    useVideoChatState.subscribe((state) => {
      currentState = state;
    });

    const testRoomName = "Test Room";
    currentState.setRoomName(testRoomName);
    expect(currentState.roomName).toBe(testRoomName);
  });

  it("should update roomId state", () => {
    let currentState = useVideoChatState.getState();

    useVideoChatState.subscribe((state) => {
      currentState = state;
    });

    const testRoomId = "123456";
    currentState.setRoomId(testRoomId);
    expect(currentState.roomId).toBe(testRoomId);
  });

  it("should reset state when leaving call", () => {
    let currentState = useVideoChatState.getState();

    useVideoChatState.subscribe((state) => {
      currentState = state;
    });

    // Set some values first
    currentState.setIsNewCall(true);
    currentState.setRoomName("Test Room");
    currentState.setRoomId("123456");

    // Leave call
    currentState.leaveCall();

    // Verify state is reset
    expect(currentState.isNewCall).toBe(false);
    expect(currentState.roomName).toBe("");
    expect(currentState.roomId).toBe("");
  });

  it("should handle multiple state updates", () => {
    let currentState = useVideoChatState.getState();

    useVideoChatState.subscribe((state) => {
      currentState = state;
    });

    currentState.setIsNewCall(true);
    currentState.setIsCall(true);
    currentState.setRoomName("Test Room");
    currentState.setRoomId("123456");

    expect(currentState.isNewCall).toBe(true);
    expect(currentState.isCall).toBe(true);
    expect(currentState.roomName).toBe("Test Room");
    expect(currentState.roomId).toBe("123456");
  });

  it("should reset state", () => {
    let currentState = useVideoChatState.getState();

    useVideoChatState.subscribe((state) => {
      currentState = state;
    });

    currentState.setIsNewCall(true);
    currentState.setIsCall(true);
    currentState.setRoomName("Test Room");
    currentState.setRoomId("123456");

    currentState.reset();

    expect(currentState.isNewCall).toBe(false);
    expect(currentState.isCall).toBe(false);
    expect(currentState.roomName).toBe("");
    expect(currentState.roomId).toBe("");
  });
});
