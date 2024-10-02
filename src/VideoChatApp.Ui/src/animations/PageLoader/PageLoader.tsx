import { motion } from "framer-motion";

import "./_PageLoader.scss";

function PageLoader() {
  return (
    <motion.div
      className="loading"
      initial={{ opacity: 0 }}
      animate={{
        opacity: [0, 0, 0.6, 1],
      }}
      transition={{
        duration: 1,
        times: [0, 0.4, 0.7, 1],
      }}
    >
      <motion.div
        className="loading__box"
        initial={{ rotate: 0, scale: 1, borderRadius: "10%" }}
        animate={{
          rotate: [0, 360, 360, 0],
          scale: [1, 1, 1.5, 1],
          borderRadius: ["10%", "10%", "50%", "10%"],
        }}
        transition={{
          duration: 3,
          ease: "easeInOut",
          times: [0, 0.4, 0.7, 1],
          repeat: Infinity,
          delay: 1,
        }}
      />
      <p className="loading__p">Loading, please wait...</p>
    </motion.div>
  );
}

export default PageLoader;
