// src/services/api.js
import axios from "axios";

const api = axios.create({
  baseURL: "https://api.exemplo.com",
  timeout: 10000,
});

export default api;
