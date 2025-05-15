import * as signalR from "@microsoft/signalr";
import { useCallback, useEffect, useRef, useState } from "react";

import { useJwtStore, useRequestsToJoin } from "@hooks/exports";

import { showJoinDeniedAlert } from "@utils/helpers/alerts";

import {
  JoinRoomRequest,
  NewMessage,
  Room,
  RoomMember,
} from "@contracts/index";

type Message = {
  messageId: string;
  senderId: string;
  content: string;
  sentAt: string;
};

const useSignalRVideoChat = () => {
  const connection = useRef<null | signalR.HubConnection>(null);

  const [room, setRoom] = useState<Room | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [membersLeft, setMembersLeft] = useState<string[]>([]);
  const [newMembers, setNewMembers] = useState<Set<RoomMember>>(new Set());
  const [requestToJoinOnRoom, setRequestToJoinOnRoom] =
    useState<JoinRoomRequest | null>(null);

  const { token } = useJwtStore();
  const {
    setRequestsToJoin,
    setIsAnswerToJoin,
    removeRequestToJoin,
    isAcceptingRequest,
    isAnswerToJoin,
    requestsToJoin,
  } = useRequestsToJoin();

  useEffect(() => {
    if (requestToJoinOnRoom) {
      setRequestsToJoin(requestToJoinOnRoom);
    }
  }, [requestToJoinOnRoom, setRequestsToJoin]);

  useEffect(() => {
    if (!token) return;

    const startConnection = async () => {
      connection.current = new signalR.HubConnectionBuilder()
        .withUrl(import.meta.env.VITE_SERVER_URL + "/videoChatHub", {
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets,
          accessTokenFactory: () => {
            return token || "";
          },
        })
        .withAutomaticReconnect()
        .withHubProtocol(new signalR.JsonHubProtocol())
        .build();

      try {
        await connection.current.start();

        connection.current.on("RoomCreated", handleRoomCreated);
        connection.current.on("MemberJoined", handleMemberJoined);
        connection.current.on("MemberLeft", handleMemberLeft);
        connection.current.on("RequestJoinRoom", handleRequestJoinRoom);
        connection.current.on("JoinDenied", handleJoinDenied);
      } catch (error) {
        console.error(error);
      }
    };

    startConnection();

    return () => {
      if (
        connection.current &&
        connection.current.state === signalR.HubConnectionState.Connected
      ) {
        connection.current.off("RoomCreated", handleRoomCreated);
        connection.current.off("MemberJoined", handleMemberJoined);
        connection.current.off("MemberLeft", handleMemberLeft);
        connection.current.off("RequestJoinRoom", handleRequestJoinRoom);
        connection.current.off("JoinDenied", handleJoinDenied);
        connection.current.off("MessageSent", handleMessageReceived);
        connection.current.stop();
        connection.current = null;
      }
    };
  }, [token]);

  const invokeAsync = useCallback(
    async (methodName: string, ...args: unknown[]) => {
      if (!connection.current) return;
      try {
        await connection.current.invoke(methodName, ...args);
      } catch (error) {
        console.error(error);
      }
    },
    [connection],
  );

  const handleRespondToJoinRequest = useCallback(
    async (isAccept: boolean) => {
      try {
        console.log("respond to join request", room?.id);

        await invokeAsync(
          "RespondToJoinRequest",
          room?.id,
          requestsToJoin[0].requesterId,
          isAccept,
        );
      } finally {
        setIsAnswerToJoin(false);
        removeRequestToJoin(requestsToJoin[0].requesterId);
      }
    },
    [
      invokeAsync,
      setIsAnswerToJoin,
      removeRequestToJoin,
      requestsToJoin,
      room?.id,
    ],
  );

  useEffect(() => {
    if (isAnswerToJoin && connection.current?.state === "Connected") {
      handleRespondToJoinRequest(isAcceptingRequest);
    }
  }, [handleRespondToJoinRequest, isAcceptingRequest, isAnswerToJoin]);

  const handleSetPeerId = useCallback(
    async (peerId: string) => {
      await invokeAsync("SetPeerId", peerId);
    },
    [invokeAsync],
  );

  const handleCreateRoom = useCallback(
    async (roomName: string) => {
      await invokeAsync("CreateRoom", roomName);
    },
    [invokeAsync],
  );

  const handleJoinRoom = useCallback(
    async (roomId: string) => {
      await invokeAsync("JoinRoom", roomId);
    },
    [invokeAsync],
  );

  const handleSendMessageToRoom = useCallback(
    async (newMessage: NewMessage) => {
      if (newMessage.content.trim() === "") return;
      await invokeAsync("SendMessageToRoom", newMessage);
    },
    [invokeAsync],
  );

  const handleRoomCreated = (room: Room) => {
    setRoom(room);

    setMessages((prev) => [
      ...prev,
      {
        messageId: room.id,
        senderId: "server",
        content: `Hi. This is your room id: ${room.id}`,
        sentAt: new Date().toISOString(),
      },
    ]);
  };

  const handleMemberJoined = (member: RoomMember) => {
    setNewMembers((prev) => {
      if (prev.has(member)) return prev;
      return new Set([...prev, member]);
    });
  };

  const handleMemberLeft = (memberId: string) => {
    setMembersLeft((prev) => [...prev, memberId]);
  };

  const handleRequestJoinRoom = (request: JoinRoomRequest) => {
    setRequestToJoinOnRoom(request);
  };

  const handleJoinDenied = () => {
    showJoinDeniedAlert();
  };

  const handleMessageReceived = (message: Message) => {
    setMessages((prev) => [...prev, message]);
  };

  return {
    connection: connection.current,
    room,
    messages,
    membersLeft,
    newMembers,
    setNewMembers,
    handleCreateRoom,
    handleSetPeerId,
    handleJoinRoom,
    handleSendMessageToRoom,
  };
};

export default useSignalRVideoChat;
