import { motion } from "framer-motion";

import "./_Home.scss";

import Header from "../../components/Header/Header";

import VideoIcon from "../../assets/icons/bxs_videos.svg";
import PlusIcon from "../../assets/icons/mingcute_plus-fill.svg";

function Home() {
  return (
    <motion.main
      initial={{ opacity: 0 }}
      animate={{ opacity: [0, 1] }}
      exit={{ y: "100vh" }}
      transition={{ duration: 0.5, delay: 2.2 }}
      className="home"
    >
      <Header />
      <div className="home__container">
        <div className="home__container-item primary--item">
          <div className="home__item-icon">
            <img src={VideoIcon} alt="ícone de vídeo" height={24} width={24} />
          </div>
          <h2 className="home__item-title">Nova chamada</h2>
          <p className="home__item-p">Crie uma nova chamada.</p>
        </div>
        <div className="home__container-item">
          <div className="home__item-icon">
            <img src={PlusIcon} alt="ícone do mais" height={24} width={24} />
          </div>
          <h2 className="home__item-title">Junte-se a uma sala</h2>
          <p className="home__item-p">Já tem o ID de uma sala?</p>
        </div>
      </div>
    </motion.main>
  );
}

export default Home;
