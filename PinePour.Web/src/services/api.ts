const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim() ?? "";

function normalizeBaseUrl(value: string) {
  return value.trim().replace(/\/$/, "");
}

function getDefaultApiBaseUrl() {
  if (!import.meta.env.DEV) {
    return "";
  }

  // In development, prefer same-origin requests and rely on the Vite dev-server
  // proxy rules (see `vite.config.ts`) to forward `/api` and `/menu` to the API.
  //
  // This avoids CORS/mixed-content issues and keeps asset URLs stable whether the
  // app is accessed directly on `:5173` or proxied through the API on `:8080`.
  return "";
}

export const API_BASE_URL = configuredApiBaseUrl
  ? normalizeBaseUrl(configuredApiBaseUrl)
  : normalizeBaseUrl(getDefaultApiBaseUrl());

const absoluteUrlPattern = /^(?:[a-z]+:)?\/\//i;

export function resolveApiAssetUrl(url?: string | null) {
  if (!url) {
    return "";
  }

  if (absoluteUrlPattern.test(url) || !API_BASE_URL) {
    return url;
  }

  return url.startsWith("/") ? `${API_BASE_URL}${url}` : `${API_BASE_URL}/${url}`;
}
