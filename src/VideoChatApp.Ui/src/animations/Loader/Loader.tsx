import { motion } from "framer-motion";

import LoaderIcon from "../../assets/icons/loader.svg";

function Loader() {
  const rotation = {
    initial: {
      rotate: 0,
    },
    animated: {
      rotate: 360,
      transition: {
        repeat: Infinity,
        ease: "linear",
        duration: 0.5,
      },
    },
  };

  return (
    <motion.div variants={rotation} initial="initial" animate="animated">
      <img src={LoaderIcon} alt="Loader" className="loader" />
    </motion.div>
  );
}

export default Loader;
