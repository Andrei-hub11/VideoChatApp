import dayjs from "dayjs";
import { motion } from "framer-motion";

import "./_GridVideoChat.scss";

import useVideoChat from "./useVideoChat";

import { Image } from "@components/exports";

import { GridVideoChatProps } from "@contracts/index";

import ShowChatIcon from "../../assets/icons/basil_caret-left-solid.svg";
import HiddenChatIcon from "../../assets/icons/basil_caret-right-solid.svg";
import MuteIcon from "../../assets/icons/bxs_microphone-off.svg";
import SendIcon from "../../assets/icons/bxs_send.svg";
import ChatIcon from "../../assets/icons/mdi_chat.svg";
import MicrophoneIcon from "../../assets/icons/mdi_microphone.svg";
import SuccessIcon from "../../assets/icons/mdi_success.svg";
import CopyIcon from "../../assets/icons/uil_copy.svg";

function GridVideoChat({ roomName, roomId, isNewCall }: GridVideoChatProps) {
  const {
    participants,
    videoChatState,
    localVideoRef,
    messages,
    chatState,
    user,
    participantsVideos,
    setVideoChatState,
    handleCopyRoomId,
    handleMute,
    handleLeave,
    handleToggleChat,
    handleSendMessage,
  } = useVideoChat({ roomName, roomId, isNewCall });

  return (
    <div className="video-chat">
      <div className="video-chat__item">
        <div className="video-chat__item-video">
          <video autoPlay playsInline ref={localVideoRef}></video>
        </div>
        <div className="video-chat__item--videos">
          {participants.map((participant) => {
            if (participant.id === user?.id) return null;

            return (
              <div
                className="video-chat__item-participant"
                key={participant.id}
              >
                <video
                  autoPlay
                  playsInline
                  ref={(el) => {
                    if (el) {
                      el.srcObject = participant.stream;

                      if (!participantsVideos.current.includes(el)) {
                        participantsVideos.current.push(el);
                      }
                    }
                  }}
                ></video>
              </div>
            );
          })}
        </div>
        <div className="video-chat__item-actions">
          <div className="video-chat__item-actions--mute" onClick={handleMute}>
            <Image
              {...{
                props: {
                  src: !videoChatState.isMuted ? MicrophoneIcon : MuteIcon,
                  alt: "mute icon",
                  height: 24,
                  width: 24,
                },
              }}
            />
          </div>
          <div
            className="video-chat__item-actions--leave"
            onClick={handleLeave}
          >
            <p>Leave</p>
          </div>
          <div
            className="video-chat__item-actions--chat"
            onClick={handleToggleChat}
          >
            <Image
              {...{
                props: {
                  src: ChatIcon,
                  alt: "chat icon",
                  width: 24,
                  height: 24,
                },
              }}
            />
            <Image
              {...{
                props: {
                  src: chatState.isVisible ? HiddenChatIcon : ShowChatIcon,
                  alt: "hidden chat icon",
                  width: 24,
                  height: 24,
                },
              }}
            />
          </div>
        </div>
      </div>
      <div
        className={`video-chat__item ${
          !chatState.isVisible ? "video-chat__item--variant" : ""
        }`}
      >
        <div
          className={`video-chat__item-info ${
            !chatState.isVisible ? "video-chat__item-info--variant" : ""
          }`}
        >
          <h1 className="video-chat__item-info-title">
            Participants ({participants.length})
          </h1>
        </div>
        <div
          className={`video-chat__messages ${
            chatState.isVisible ? "video-chat__messages--visible" : ""
          }`}
        >
          {messages.map((message) => (
            <div
              className={`video-chat__messages-item ${
                message.senderId === user?.id
                  ? "video-chat__messages-item--variant"
                  : ""
              }`}
              key={message.messageId}
            >
              <p className="video-chat__messages-text">{message.content}</p>
              <div className="video-chat__messages-info">
                <div
                  className="video-chat__messages-image"
                  onClick={handleCopyRoomId}
                >
                  {chatState.isCopied ? (
                    <motion.img
                      initial={{ opacity: 0 }}
                      animate={{ opacity: [0.4, 0.6, 1] }}
                      transition={{ duration: 0.3 }}
                      src={SuccessIcon}
                      alt="success icon"
                      className="success-icon"
                      width={24}
                      height={24}
                    />
                  ) : (
                    <img
                      src={CopyIcon}
                      alt="copy icon"
                      width={24}
                      height={24}
                    />
                  )}
                </div>
                <p className="video-chat__messages-time">
                  {dayjs(message.sentAt).format("HH:mm")}
                </p>
              </div>
            </div>
          ))}
        </div>
        <div
          className={`video-chat__item-input ${chatState.isVisible ? "video-chat__item-input--visible" : ""}`}
        >
          <input
            type="text"
            placeholder="Type a message"
            value={videoChatState.newMessage}
            onChange={(e) => {
              setVideoChatState((prev) => ({
                ...prev,
                newMessage: e.target.value,
              }));
            }}
          />
          <div className="btn-send" onClick={handleSendMessage}>
            <img src={SendIcon} alt="send icon" width={24} height={24} />
          </div>
        </div>
      </div>
    </div>
  );
}

export default GridVideoChat;
