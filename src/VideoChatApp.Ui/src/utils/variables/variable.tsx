import { RouteObject } from "react-router-dom";

import ProtectedRoute from "../../components/ProtectedRoute/ProtectedRoute";

import PageTransition from "../../animations/PageTransition/PageTransition";
import Home from "../../pages/Home/Home";
import Login from "../../pages/Login/Login";
import Register from "../../pages/Register/Register";

export const routes: RouteObject[] = [
  {
    path: "/register",
    element: <Register />,
  },
  {
    path: "/login",
    element: <Login />,
  },
  {
    path: "/home",
    element: (
      <ProtectedRoute>
        <PageTransition>
          <Home />
        </PageTransition>
      </ProtectedRoute>
    ),
  },
];
