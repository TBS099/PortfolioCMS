# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM node:22-alpine AS build
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
