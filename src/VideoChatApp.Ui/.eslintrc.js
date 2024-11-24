module.exports = {
  settings: {
    "import/resolver": {
      node: {
        extensions: [".js", ".vue", ".ts", ".tsx", ".d.ts"],
      },
      alias: {
        map: [
          ["@components", "./src/components"],
          ["@pages", "./src/pages"],
          ["@store", "./src/store"],
          ["@contracts", "./src/contracts"],
          ["@utils", "./src/utils"],
        ],
        extensions: [".js", ".vue", ".ts", ".tsx", ".scss", ".d.ts"],
      },
    },
  },
};
