// Styles
import 'material-design-icons-iconfont/dist/material-design-icons.css'
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

// Vuetify
import { createVuetify } from 'vuetify';
import { VDateInput } from 'vuetify/labs/VDateInput';
import { aliases, md } from 'vuetify/iconsets/md';
import { mdi } from 'vuetify/iconsets/mdi'; 
import { es, en } from 'vuetify/locale';
import { ThemeDefinition } from 'vuetify';
import { createI18n, I18n } from 'vue-i18n';
import type { LocaleMessages } from '@intlify/core-base';

// Definición de mensajes para la localización
const messages: LocaleMessages<any> =  {
  es: {
    ...es,
  },
  en: {
    ...en,
  },
};

// Definición del tema personalizado
const myCustomLightTheme: ThemeDefinition = {
  dark: true,
  colors: {
    background: '#FFFFFF',
    surface: '#FFFFFF',
    'surface-bright': '#FFFFFF',
    'surface-light': '#EEEEEE',
    'surface-variant': '#424242',
    'on-surface-variant': '#EEEEEE',
    primary: '#1867C0',
    'primary-darken-1': '#1F5592',
    secondary: '#48A9A6',
    'secondary-darken-1': '#018786',
    error: '#B00020',
    info: '#2196F3',
    success: '#4CAF50',
    warning: '#FB8C00',
  },
  variables: {
    'border-color': '#000000',
    'border-opacity': 0.12,
    'high-emphasis-opacity': 0.87,
    'medium-emphasis-opacity': 0.60,
    'disabled-opacity': 0.38,
    'idle-opacity': 0.04,
    'hover-opacity': 0.04,
    'focus-opacity': 0.12,
    'selected-opacity': 0.08,
    'activated-opacity': 0.12,
    'pressed-opacity': 0.12,
    'dragged-opacity': 0.08,
    'theme-kbd': '#212529',
    'theme-on-kbd': '#FFFFFF',
    'theme-code': '#F5F5F5',
    'theme-on-code': '#000000',
  },
};

// Creación de la instancia de i18n
const i18n: I18n = createI18n({
  legacy: false,
  locale: 'es',
  fallbackLocale: 'en',  
  messages,
});

// Creación de la instancia de Vuetify
const vuetify = createVuetify({
  components: {
    VDateInput,
  },
  locale: {
    locale: 'es',
    messages: {
      es,
      en,
    },
  },
  icons: {
    defaultSet: 'md',
    aliases,
    sets: {
      md,
      mdi,
    },
  },
  theme: {
    defaultTheme: 'myCustomLightTheme',
    themes: {
      myCustomLightTheme,
    },
  },
});

// Exportación de las instancias de Vuetify e i18n
export { vuetify, i18n };
