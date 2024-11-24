import { motion } from "framer-motion";

import Image from "../../components/Image/Image";

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
      <Image
        {...{
          props: { src: LoaderIcon, alt: "Loader", width: 15, height: 15 },
        }}
      />
    </motion.div>
  );
}

export default Loader;
