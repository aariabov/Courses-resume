import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";
import path from "path";
import { visualizer } from "rollup-plugin-visualizer";

export default defineConfig(({ mode }) => {
  // Load env file based on `mode` in the current working directory.
  // Set the third parameter to '' to load all env regardless of the
  // `VITE_` prefix.
  const env = loadEnv(mode, process.cwd(), "");
  return {
    plugins: [
      visualizer({ open: true, template: "sunburst", gzipSize: true }),
      react(),
      // Чтение md файлов, как строк, самописная реализация.
      // Можно использовать готовый пакет https://github.com/hmsk/vite-plugin-markdown, но там избыточный функционал
      {
        name: "markdown-loader",
        transform(code, id) {
          if (id.slice(-3) === ".md") {
            return `export default ${JSON.stringify(code)};`;
          }
        },
      },
    ],
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
    server: {
      port: 3001, // Можно выбрать любой другой порт
      proxy: {
        "/api": {
          target: env.BACKEND_URL,
          changeOrigin: true,
          secure: false,
        },
      },
    },
  };
});
