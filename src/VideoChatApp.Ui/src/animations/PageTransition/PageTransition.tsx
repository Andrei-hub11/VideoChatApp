import { motion } from "framer-motion";

import "./_PageTransition.scss";

import useAuthTransitionStore from "../../hooks/animation/useAuthTransitionStore";

interface PageTransitionProps {
  children: React.ReactNode;
}

function PageTransition({ children }: PageTransitionProps) {
  const isRedirectingToAuth = useAuthTransitionStore(
    (state) => state.isRedirectingToAuth,
  );

  const items = Array(8).fill(1);

  const pageTransitionAnimation = {
    initial: { display: "flex" },
    animate: { display: "none" },
    exit: isRedirectingToAuth
      ? {
          display: "flex",
          transition: { duration: 1 },
        }
      : {},
    transition: () => ({
      delay: 0.4 * items.length,
      ease: "linear",
    }),
  };

  const itemAnimation = (index: number) => ({
    initial: { y: 1000 },
    animate: { y: [1000, 100, -100, 1000] },
    exit: isRedirectingToAuth
      ? {
          y: [1000, 100, -100, 1000],
          transition: { duration: 2, delay: 0.1 * index, ease: "linear" },
        }
      : {},
    transition: {
      duration: 2,
      delay: 0.1 * index,
      ease: "linear",
    },
  });

  return (
    <section className="page">
      {children}
      <motion.div
        className="page-transition"
        {...pageTransitionAnimation}
        transition={pageTransitionAnimation.transition()}
      >
        {items.map((_, index) => (
          <motion.div
            key={index}
            {...itemAnimation(index)}
            className="page-transition__item"
          ></motion.div>
        ))}
      </motion.div>
    </section>
  );
}

export default PageTransition;
