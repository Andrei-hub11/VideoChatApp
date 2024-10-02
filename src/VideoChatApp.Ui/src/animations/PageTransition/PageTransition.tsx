import { motion } from "framer-motion";

import "./_PageTransition.scss";

interface PageTransitionProps {
  children: React.ReactNode;
}

function PageTransition({ children }: PageTransitionProps) {
  const items = Array(8).fill(1);

  return (
    <section className="page">
      {children}
      <motion.div
        className="page-transition"
        initial={{ display: "flex" }}
        animate={{ display: "none" }}
        exit={{
          display: "flex",
          transition: { duration: 1 },
        }}
        transition={{
          delay: 0.4 * items.length,
          ease: "linear",
        }}
      >
        {items.map((_, index) => (
          <motion.div
            key={index}
            initial={{ y: 1000 }}
            animate={{ y: [1000, 100, -100, 1000] }}
            exit={{
              y: [1000, 100, -100, 1000],
              transition: { duration: 2, delay: 0.1 * index, ease: "linear" },
            }}
            transition={{
              duration: 2,
              delay: 0.1 * index,
              ease: "linear",
            }}
            className="page-transition__item"
          ></motion.div>
        ))}
      </motion.div>
    </section>
  );
}

export default PageTransition;
