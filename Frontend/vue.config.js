const { defineConfig } = require('@vue/cli-service')
const webpack = require('webpack');

module.exports = defineConfig({
  transpileDependencies: true,
  pwa: {
    name: 'Waresoft',
    short_name: 'WS',
  },
  pages: {
    index: {
      entry: 'src/main.ts',
      title: 'Waresoft'
    }
  },

  configureWebpack: {
    plugins: [
      new webpack.DefinePlugin({
        __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: 'false', // o false según tu necesidad
      })
    ],
  },

  pluginOptions: {
    vuetify: {
      // https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vuetify-loader
    }
  }
})
