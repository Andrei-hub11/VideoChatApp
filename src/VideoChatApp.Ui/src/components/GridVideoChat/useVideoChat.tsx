import Peer, { PeerJSOption } from "peerjs";
import { useEffect, useRef, useState, useCallback } from "react";

import useSignalRVideoChat from "./useSignalRVideoChat";

import { useUserStore } from "@hooks/exports";
import useVideoChatState from "@hooks/useChat/useVideoChatState";

import { GridVideoChatProps, Participant, RoomMember } from "@contracts/index";

type VideoChatState = {
  currentRoomId: string | null;
  peerId: string | null;
  isMuted: boolean;
  newMessage: string;
};

type ChatState = {
  isCopied: boolean;
  isVisible: boolean;
};

const peerConfig: PeerJSOption = {
  host: import.meta.env.VITE_PEER_HOST || "localhost",
  port: Number(import.meta.env.VITE_PEER_PORT) || 9000,
  path: "/peerjs",
  debug: 3,
};

type UseVideoChatProps = GridVideoChatProps;

const useVideoChat = ({ roomName, roomId, isNewCall }: UseVideoChatProps) => {
  const localVideoRef = useRef<HTMLVideoElement>(null);
  const participantsVideos = useRef<HTMLVideoElement[]>([]);

  const [peer, setPeer] = useState<Peer | null>(null);
  const [participants, setParticipants] = useState<Participant[]>([]);
  const [videoChatState, setVideoChatState] = useState<VideoChatState>({
    currentRoomId: null,
    peerId: null,
    isMuted: true,
    newMessage: "",
  });
  const [localStream, setLocalStream] = useState<MediaStream | null>(null);
  const [isInitialized, setIsInitialized] = useState<boolean>(false);
  const [chatState, setChatState] = useState<ChatState>({
    isCopied: false,
    isVisible: true,
  });
  const [isRoomCreated, setIsRoomCreated] = useState<boolean>(false);
  const [isJoiningRoom, setIsJoiningRoom] = useState<boolean>(false);
  const [isPeerIdSet, setIsPeerIdSet] = useState<boolean>(false);
  const localStreamRef = useRef<MediaStream | null>(null);
  const peerRef = useRef<Peer | null>(null);
  const isNewPeerConnected = useRef<boolean>(false);

  const { user } = useUserStore();
  const { reset } = useVideoChatState();

  const {
    connection,
    room,
    messages,
    membersLeft,
    newMembers,
    setNewMembers,
    handleCreateRoom,
    handleSetPeerId,
    handleJoinRoom,
    handleSendMessageToRoom,
  } = useSignalRVideoChat();

  // Initialize local stream
  useEffect(() => {
    const initializeStream = async () => {
      try {
        const stream = await navigator.mediaDevices.getUserMedia({
          video: true,
          audio: true,
        });
        localStreamRef.current = stream;
        setLocalStream(stream);
        setIsInitialized(true);
      } catch (err) {
        console.error("Failed to get local stream", err);
      }
    };

    initializeStream();

    return () => {
      if (localStreamRef.current) {
        localStreamRef.current.getTracks().forEach((track) => track.stop());
        localStreamRef.current = null;
      }
    };
  }, []);

  useEffect(() => {
    if (
      videoChatState.peerId &&
      !isPeerIdSet &&
      connection?.state === "Connected"
    ) {
      handleSetPeerId(videoChatState.peerId);
      setIsPeerIdSet(true);
    }

    if (isNewCall && !isRoomCreated && connection?.state === "Connected") {
      handleCreateRoom(roomName);
      setIsRoomCreated(true);
    }

    if (
      room &&
      isRoomCreated &&
      user &&
      !participants.some((p) => p.id === user.id)
    ) {
      setVideoChatState((prev) => ({
        ...prev,
        currentRoomId: room.id,
      }));

      setParticipants((prev) => [
        ...prev,
        {
          id: user.id,
          peerId: room.id,
          roomId: room.id,
          stream: localStream!,
        },
      ]);
    }

    if (
      !isNewCall &&
      !isJoiningRoom &&
      roomId &&
      connection?.state === "Connected"
    ) {
      handleJoinRoom(roomId);
      setIsJoiningRoom(true);
      setVideoChatState((prev) => ({
        ...prev,
        currentRoomId: roomId,
      }));
    }
  }, [
    connection?.state,
    room,
    isRoomCreated,
    isPeerIdSet,
    videoChatState.peerId,
    localStream,
    user,
    participants,
    isJoiningRoom,
    isNewCall,
    roomName,
    roomId,
    handleSetPeerId,
    handleCreateRoom,
    handleJoinRoom,
  ]);

  // Connect to new peer
  const connectToPeer = useCallback(
    async (newMember: RoomMember) => {
      if (!peer || !localStream) return;

      try {
        const call = peer.call(newMember.peerId, localStream, {
          metadata: {
            userId: user?.id,
          },
        });

        call.on("stream", (remoteStream) => {
          console.log("participants", participants);

          setParticipants((prev) => {
            // Avoid duplicate participants
            if (prev.some((p) => p.id === newMember.userId)) return prev;

            return [
              ...prev,
              {
                id: newMember.userId,
                peerId: newMember.peerId,
                roomId: newMember.roomId,
                stream: remoteStream,
              },
            ];
          });
        });

        call.on("error", (error) => {
          console.error("Peer call error:", error);
        });

        call.on("close", () => {
          setParticipants((prev) =>
            prev.filter((p) => p.id !== newMember.userId),
          );
        });
      } catch (err) {
        console.error("Error connecting to peer:", err);
      }
    },
    [peer, localStream, user?.id, participants],
  );

  useEffect(() => {
    if (newMembers.size === 0) return;

    if (isNewPeerConnected.current) return;

    console.log("isNewPeerConnected", isNewPeerConnected.current);

    isNewPeerConnected.current = true;
    //connect to new member
    newMembers.forEach((member) => {
      if (member.userId === user?.id) return;

      if (participants.some((p) => p.id === member.userId)) return;

      connectToPeer(member);
      isNewPeerConnected.current = false;
    });
  }, [connectToPeer, newMembers, participants, setNewMembers, user?.id]);

  const MAX_RETRY_ATTEMPTS = 4;
  const RETRY_DELAY = 1000; // 1 second

  useEffect(() => {
    const initializePeer = async (retryCount = 0) => {
      try {
        const peerId = crypto.randomUUID();
        const newPeer = new Peer(peerId, peerConfig);

        newPeer.on("open", (id) => {
          setVideoChatState((prev) => ({ ...prev, peerId: id }));
          setIsPeerIdSet(false);
        });

        newPeer.on("error", async (error) => {
          newPeer.destroy();

          if (retryCount < MAX_RETRY_ATTEMPTS) {
            await new Promise((resolve) => setTimeout(resolve, RETRY_DELAY));
            initializePeer(retryCount + 1);
          }

          console.log("error", error);
        });

        newPeer.on("call", (call) => {
          if (!localStream) {
            console.error("No local stream available");
            return;
          }

          call.answer(localStream);

          call.on("stream", (remoteStream) => {
            setParticipants((prev) => {
              if (prev.some((p) => p.peerId === call.peer)) return prev;

              return [
                ...prev,
                {
                  id: call.metadata.userId,
                  peerId: call.peer,
                  roomId: roomId,
                  stream: remoteStream,
                },
              ];
            });
          });
        });

        peerRef.current = newPeer;
        setPeer(newPeer);
      } catch (err) {
        if (retryCount < MAX_RETRY_ATTEMPTS) {
          await new Promise((resolve) => setTimeout(resolve, RETRY_DELAY));
          initializePeer(retryCount + 1);
        }
      }
    };

    if (isInitialized) {
      initializePeer();
    }

    return () => {
      if (peerRef.current) {
        peerRef.current.destroy();
        peerRef.current = null;
      }
    };
  }, [
    isInitialized,
    localStream,
    roomId,
    videoChatState.currentRoomId,
    user?.id,
  ]);

  useEffect(() => {
    if (localVideoRef.current && localStream) {
      localVideoRef.current.srcObject = localStream;
    }

    const videoElement = localVideoRef.current;

    if (videoElement) {
      setVideoChatState((prev) => ({
        ...prev,
        isMuted: videoElement.muted,
      }));
    }
  }, [localStream, videoChatState.isMuted]);

  useEffect(() => {
    if (membersLeft.length === 0) return;

    setParticipants((prev) => prev.filter((p) => !membersLeft.includes(p.id)));
  }, [membersLeft]);

  const handleCopyRoomId = useCallback(() => {
    navigator.clipboard.writeText(videoChatState.currentRoomId!);
    setChatState((prev) => ({ ...prev, isCopied: true }));
    setTimeout(() => {
      setChatState((prev) => ({ ...prev, isCopied: false }));
    }, 3000);
  }, [videoChatState.currentRoomId]);

  const handleMute = useCallback(() => {
    if (localVideoRef.current) {
      localStream?.getAudioTracks().forEach((track) => {
        track.enabled = !track.enabled;
      });
      localVideoRef.current.muted = !localVideoRef.current.muted;
      setVideoChatState((prev) => ({ ...prev, isMuted: !prev.isMuted }));
    }
  }, [localVideoRef, localStream]);

  const handleLeave = useCallback(() => {
    participants.forEach((participant) => {
      if (participant.stream) {
        participant.stream.getTracks().forEach((track) => {
          track.enabled = false;
          track.stop();
        });
        participant.stream = null;
      }
    });

    if (localVideoRef.current) {
      localVideoRef.current.srcObject = null;
    }

    reset();
  }, [participants, localVideoRef, reset]);

  const handleToggleChat = useCallback(() => {
    setChatState((prev) => ({ ...prev, isVisible: !prev.isVisible }));
  }, []);

  const handleSendMessage = useCallback(() => {
    if (videoChatState.newMessage.trim() === "") return;

    if (!videoChatState.currentRoomId) {
      throw new Error("Room ID is not set");
    }

    if (!user?.id) {
      throw new Error("User ID is not set");
    }

    const memberId = room?.members.find((m) => m.userId === user.id)?.memberId;

    if (!memberId) {
      throw new Error("Member ID is not set");
    }

    handleSendMessageToRoom({
      roomId: videoChatState.currentRoomId!,
      content: videoChatState.newMessage,
      memberId,
    });

    setVideoChatState((prev) => ({ ...prev, newMessage: "" }));
  }, [
    handleSendMessageToRoom,
    user?.id,
    videoChatState.currentRoomId,
    videoChatState.newMessage,
    room?.members,
  ]);

  return {
    localVideoRef,
    participantsVideos,
    participants,
    videoChatState,
    messages,
    chatState,
    user,
    setVideoChatState,
    handleCopyRoomId,
    handleMute,
    handleLeave,
    handleToggleChat,
    handleSendMessage,
  };
};

export default useVideoChat;
