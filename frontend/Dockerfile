# Shop SPA: Angular production build served by nginx (ADR-0014).
ARG NODE_VERSION=24.15.0
ARG NGINX_VERSION=1.29

FROM node:${NODE_VERSION}-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:${NGINX_VERSION}-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/shop/browser /usr/share/nginx/html
EXPOSE 80
