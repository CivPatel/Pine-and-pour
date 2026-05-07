import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

function normalizeProxyTarget(value: string) {
  const trimmed = value.trim();
  if (!trimmed) {
    return "";
  }

  try {
    const url = new URL(trimmed);
    if (url.hostname === "localhost") {
      url.hostname = "127.0.0.1";
    }

    return url.toString().replace(/\/$/, "");
  } catch {
    return trimmed.replace(/\/$/, "");
  }
}

// Default to IPv4 to avoid Node/Vite proxy issues on some machines where
// `localhost` resolves to `::1` first but the ASP.NET dev server is only
// listening on IPv4 (0.0.0.0 / 127.0.0.1).
const apiProxyTarget = normalizeProxyTarget(
  process.env.VITE_API_PROXY_TARGET ?? "http://127.0.0.1:8080",
);

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    allowedHosts: ["web"],
    proxy: {
      "/api": {
        target: apiProxyTarget,
        changeOrigin: true,
        followRedirects: true,
        secure: false,
      },
      // Only proxy actual menu asset paths (e.g. `/menu/drinks/...`) so the SPA
      // route `/menu` can still be served by Vite on refresh.
      "/menu/": {
        target: apiProxyTarget,
        changeOrigin: true,
        followRedirects: true,
        secure: false,
      },
      "/swagger": {
        target: apiProxyTarget,
        changeOrigin: true,
        followRedirects: true,
        secure: false,
      },
    },
  },
});
