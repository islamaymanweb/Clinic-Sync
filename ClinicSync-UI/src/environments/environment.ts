export const environment = {
  production: false,
  
 
  apiUrl: 'https://localhost:7095',
 
  appName: 'ClinicSync',
  version: '1.0.0',
 
 /*  auth: {
    cookieName: 'ClinicSync.Auth',
    tokenRefreshInterval: 300000, // 5 دقائق
    sessionTimeout: 3600000 // ساعة واحدة
  }, */
  auth: {
    tokenKey: 'HealthSync.Auth',
    userKey: 'healthsync_current_user',
    rememberMeDuration: 7, // أيام
    sessionDuration: 1 // يوم
  },
  app: {
    defaultLanguage: 'ar',
    supportedLanguages: ['ar', 'en'],
    pageSize: 10,
    enableDebug: true
  },
  
   
  endpoints: {
    auth: {
      login: '/api/auth/login',
      register: '/api/auth/register',
      logout: '/api/auth/logout',
      me: '/api/auth/me',
 
      forgotPassword: '/api/auth/forgot-password',
      resetPassword: '/api/auth/reset-password',
 
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
  },
  
  // إعدادات الـ Features
  features: {
    enableEmailVerification: true,
    enablePasswordReset: true,
    enableDoctorRegistration: true,
    enablePatientRegistration: true,
    enableAppointmentBooking: true
  }
};