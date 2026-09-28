# syntax=docker/dockerfile:1

# ---- Build stage ----
# node:22-slim (Debian/glibc), not node:22-alpine (musl): lightningcss
# (via Tailwind v4) and Vite's rolldown bindings both ship libc-specific
# native/wasm packages, and package-lock.json was generated on a glibc
# machine - `npm ci` on Alpine fails with "Missing: @emnapi/... from lock
# file" because the musl/wasm fallback variants were never resolved into
# it. Staying on glibc sidesteps the whole mismatch instead of needing a
# second, Alpine-specific lock file.
FROM node:22-slim AS build
WORKDIR /src

COPY package.json package-lock.json* ./
RUN npm ci

COPY . .

# Baked in at build time - Vite inlines VITE_* vars into the static bundle,
# so this has to be the public URL the browser will call, not a
# container-internal one. Override with --build-arg when building.
ARG VITE_API_URL=https://localhost:7174/api
ENV VITE_API_URL=$VITE_API_URL

RUN npm run build

# ---- Runtime stage ----
FROM nginx:1.27-alpine AS final

COPY --from=build /src/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
