import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import vuetify from 'vite-plugin-vuetify'
import { VitePWA } from 'vite-plugin-pwa'
import { fileURLToPath, URL } from 'node:url'

export default defineConfig({
  plugins: [
    vue(),
    vuetify({ autoImport: true }),
    VitePWA({
      registerType: 'autoUpdate',
      manifest: {
        name: 'Waresoft',
        short_name: 'WS',
      }
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  define: {
    __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: false,
  },
  optimizeDeps: {
    include: [
    'vuetify',
    'vuetify/components/VAlert',
    'vuetify/components/VApp',
    'vuetify/components/VAppBar',
    'vuetify/components/VAutocomplete',
    'vuetify/components/VAvatar',
    'vuetify/components/VBtn',
    'vuetify/components/VCard',
    'vuetify/components/VCheckbox',
    'vuetify/components/VDataTable',
    'vuetify/components/VDateInput',
    'vuetify/components/VDialog',
    'vuetify/components/VFooter',
    'vuetify/components/VGrid',
    'vuetify/components/VIcon',
    'vuetify/components/VImg',
    'vuetify/components/VList',
    'vuetify/components/VMain',
    'vuetify/components/VNavigationDrawer',
    'vuetify/components/VProgressCircular',
    'vuetify/components/VSelect',
    'vuetify/components/VSwitch',
    'vuetify/components/VTextField',
    'vuetify/components/VToolbar',
    'vuetify/components/VTooltip',
    'vuetify/directives',
    'vuetify/components/VChip', 
    'vuetify/components/VDivider', 
    'vuetify/components/VForm'
    ]
  },
  test: {
    environment: 'happy-dom',
    globals: true,
  }
})