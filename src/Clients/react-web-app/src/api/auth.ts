import { apiClient } from './client';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  roles?: string[];
}

export interface VerifyIdentityRequest {
  tckn: string;
  birthDate: string;
  serialNumber: string;
  expiryDate: string;
}

export interface VerifyIdentityResponse {
  success: boolean;
  verificationToken: string;
  message: string;
}

export interface ResetPasswordRequest {
  verificationToken: string;
  newPassword: string;
}

export interface UserSessionProfile {
  id: string;
  email: string;
  role: 'STUDENT' | 'ACADEMIC' | 'STAFF' | 'ADMIN';
  roles?: string[];
  fullName: string;
  studentNumber?: string;
  department: string;
  faculty: string;
  currentProgram: string;
  availablePrograms: string[];
}

export const authApi = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await apiClient.post<LoginResponse>('/api/v1/auth/login', credentials);
    return response.data;
  },

  verifyIdentity: async (data: VerifyIdentityRequest): Promise<VerifyIdentityResponse> => {
    const response = await apiClient.post<VerifyIdentityResponse>('/api/v1/auth/forgot-password/verify-identity', data);
    return response.data;
  },

  resetPassword: async (data: ResetPasswordRequest): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.post<{ success: boolean; message: string }>('/api/v1/auth/forgot-password/reset', data);
    return response.data;
  },

  refreshToken: async (token: string): Promise<LoginResponse> => {
    const response = await apiClient.post<LoginResponse>('/api/v1/auth/refresh', { refreshToken: token });
    return response.data;
  },

  logout: async (sessionId?: string): Promise<void> => {
    try {
      await apiClient.post('/api/v1/auth/logout', { sessionId });
    } catch {
      // Offline veya sunucu hatası olsa bile yerel oturumu kapat
    } finally {
      localStorage.removeItem('obs_access_token');
      localStorage.removeItem('obs_refresh_token');
      localStorage.removeItem('obs_user_profile');
    }
  },
};
