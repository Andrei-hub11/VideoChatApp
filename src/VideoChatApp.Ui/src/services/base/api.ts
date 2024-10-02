import axios from "axios";
import camelcaseKeys from "camelcase-keys";

// Crie uma instância do axios
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

api.interceptors.response.use(
  (response) => {
    if (response.data) {
      response.data = camelcaseKeys(response.data, { deep: true });
    }
    return response;
  },
  (error) => {
    return Promise.reject(error);
  },
);

export default api;
