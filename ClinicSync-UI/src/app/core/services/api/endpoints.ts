export const API_ENDPOINTS = {
  // Auth endpoints
  AUTH: {
    LOGIN: '/api/auth/login',
    REGISTER: '/api/auth/register',
    LOGOUT: '/api/auth/logout',
    ME: '/api/auth/me',
    VERIFY_EMAIL: '/api/auth/verify-email',
    FORGOT_PASSWORD: '/api/auth/forgot-password',
    RESET_PASSWORD: '/api/auth/reset-password'
  },
  
  // Admin endpoints
  ADMIN: {
    DOCTORS: '/api/admin/doctors',
    USERS: '/api/admin/users'
  }
} as const;