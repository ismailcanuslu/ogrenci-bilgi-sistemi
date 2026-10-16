import axios, { type InternalAxiosRequestConfig, type AxiosResponse, type AxiosError } from 'axios';

// Vite proxy veya doğrudan backend URL'si
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 15000,
});

// Request Interceptor: JWT Token Ekleme
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('obs_access_token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => Promise.reject(error)
);

// Response Interceptor: 401 Yönetimi
apiClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error: AxiosError) => {
    if (error.response && error.response.status === 401) {
      // Oturum süresi dolmuşsa temizle
      localStorage.removeItem('obs_access_token');
      localStorage.removeItem('obs_refresh_token');
      localStorage.removeItem('obs_user_profile');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);
