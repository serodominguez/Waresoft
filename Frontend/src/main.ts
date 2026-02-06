// Core Vue
import { createApp } from 'vue'
import App from './App.vue'

// Plugins y Rutas
import router from './router'
import pinia from './stores'
import { vuetify, i18n } from './plugins/vuetify'
import { loadFonts } from './plugins/webfontloader'
import permissionsPlugin from './plugins/permissions'

// Configuración de Axios
import { configureAxiosDefaults, setupAxiosInterceptors } from './plugins/axiosInterceptor'

// Store de autenticación
import { useAuthStore } from './stores/auth'

// Toast
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'

// Configurar Axios
configureAxiosDefaults()
setupAxiosInterceptors()

// Cargar fuentes
loadFonts()

// Opciones para Toast
const toastOptions = {
  position: 'top-center',
  timeout: 1500,
  closeOnClick: true,
  pauseOnFocusLoss: true,
  pauseOnHover: true,
  draggable: true,
  draggablePercent: 0.6,
  showCloseButtonOnHover: false,
  closeButton: 'button',
  icon: true,
  rtl: false,
  transition: 'Vue-Toastification__bounce',
  maxToasts: 3,
  newestOnTop: true,
}

// Inicialización
 async function initializeApp() {
  // Crear la aplicación
  const app = createApp(App)

  app.use(router)
  app.use(pinia)
  app.use(vuetify)
  app.use(i18n)
  app.use(Toast, toastOptions)
  app.use(permissionsPlugin)

  // Global error handler
  app.config.errorHandler = (err, instance, info) => {
    console.error('[Vue Error Handler]', {
      error: err,
      instance,
      info,
    })
  }

  // Espera a que se cargue la autenticación
   const authStore = useAuthStore()
   await authStore.initializeAuth()

  // Montar la app después de que la autenticación esté lista
  app.mount('#app')

  // Ocultar loading screen
  document.body.classList.add('app-mounted')
}

// Inicializar la app
initializeApp()