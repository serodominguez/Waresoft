const { defineConfig } = require("@vue/cli-service");
const webpack = require("webpack");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = defineConfig({
  transpileDependencies: true,
  pwa: {
    name: "Waresoft",
    short_name: "WS",
  },
  pages: {
    index: {
      entry: "src/main.ts",
      title: "Waresoft",
    },
  },
  chainWebpack: (config) => {
    config.optimization.minimizers.delete("css");
    config.optimization.minimizer("css").use(CssMinimizerPlugin, [
      {
        minimizerOptions: {
          preset: [
            "default",
            {
              discardDuplicates: false,
            },
          ],
        },
      },
    ]);
  },
  configureWebpack: {
    plugins: [
      new webpack.DefinePlugin({
        __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: "false",
      }),
    ],
  },
  pluginOptions: {
    vuetify: {},
  },
});
