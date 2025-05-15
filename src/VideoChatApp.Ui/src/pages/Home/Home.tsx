import { motion } from "framer-motion";

import "./_Home.scss";

import useHomeLogic from "./useHomeLogic";

import CallPopup from "@components/CallPopup/CallPopup";
import GridVideoChat from "@components/GridVideoChat/GridVideoChat";
import { Header } from "@components/exports";

import VideoIcon from "../../assets/icons/bxs_videos.svg";
import PlusIcon from "../../assets/icons/mingcute_plus-fill.svg";

function Home() {
  const {
    isCall,
    isNewCall,
    roomName,
    roomId,
    newCallPopup,
    joinRoomPopup,
    handleNewCall,
    handleJoinCall,
  } = useHomeLogic();

  return (
    <motion.main
      initial={{ opacity: 0 }}
      animate={{ opacity: [0, 1] }}
      exit={{ opacity: 0, transition: { duration: 0.2 } }}
      transition={{ duration: 0.5, delay: 2.2 }}
      className="home"
    >
      <Header {...{ currentPage: "Home" }} />
      {!isCall && (
        <>
          <div className="home__container">
            <div
              className="home__container-item primary--item"
              onClick={newCallPopup.toggle}
            >
              <div className="home__item-icon">
                <img
                  src={VideoIcon}
                  alt="ícone de vídeo"
                  height={24}
                  width={24}
                />
              </div>
              <h2 className="home__item-title">Nova chamada</h2>
              <p className="home__item-p">Crie uma nova chamada.</p>
            </div>
            <div
              className="home__container-item"
              onClick={joinRoomPopup.toggle}
            >
              <div className="home__item-icon">
                <img
                  src={PlusIcon}
                  alt="ícone do mais"
                  height={24}
                  width={24}
                />
              </div>
              <h2 className="home__item-title">Junte-se a uma sala</h2>
              <p className="home__item-p">Já tem o ID de uma sala?</p>
            </div>
          </div>
          <CallPopup
            {...{
              isOpen: newCallPopup.isOpen,
              handleClose: newCallPopup.close,
              title: "Nova chamada",
              placeholder: "Digite o nome da sala",
              handleAction: handleNewCall,
            }}
          />
          <CallPopup
            {...{
              isOpen: joinRoomPopup.isOpen,
              handleClose: joinRoomPopup.close,
              title: "Junte-se a uma sala",
              placeholder: "Digite o ID da sala",
              handleAction: handleJoinCall,
            }}
          />
        </>
      )}
      {isNewCall && isCall && (
        <GridVideoChat
          {...{
            roomName: roomName,
            roomId: roomId,
            isNewCall: isNewCall,
          }}
        />
      )}

      {!isNewCall && isCall && (
        <GridVideoChat
          {...{
            roomId: roomId,
            roomName: "",
            isNewCall: false,
          }}
        />
      )}
    </motion.main>
  );
}

export default Home;
