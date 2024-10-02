import { BrowserRouter as Router } from "react-router-dom";

import AnimatedRoutes from "./animations/PageTransition/AnimatedRoutes";

function App() {
  return (
    <>
      <Router>
        <AnimatedRoutes />
      </Router>
    </>
  );
}

export default App;
