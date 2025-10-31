export const environment = {
  production: true,
  
  // 🔗 رابط الـ Backend في الإنتاج
  apiUrl: 'https://api.healthsync.com',
  
  appName: 'HealthSync',
  version: '1.0.0',
  
  auth: {
    cookieName: 'ClinicSync.Auth',
    tokenRefreshInterval: 300000,
    sessionTimeout: 3600000
  },
  
  app: {
    defaultLanguage: 'ar',
    supportedLanguages: ['ar', 'en'],
    pageSize: 20,
    enableDebug: false
  },
  
  endpoints: {
    auth: {
      login: '/api/auth/login',
      register: '/api/auth/register',
      logout: '/api/auth/logout',
      me: '/api/auth/me',
      verifyEmail: '/api/auth/verify-email',
      forgotPassword: '/api/auth/forgot-password',
      resetPassword: '/api/auth/reset-password'
    },
    admin: {
      doctors: '/api/admin/doctors',
      users: '/api/admin/users',
      specialties: '/api/admin/specialties'
    },
    patient: {
      appointments: '/api/patient/appointments',
      profile: '/api/patient/profile'
    },
    doctor: {
      appointments: '/api/doctor/appointments',
      schedule: '/api/doctor/schedule',
      profile: '/api/doctor/profile'
    }
  }
};