FROM node:22-alpine AS build
WORKDIR /app
COPY src/frontend/Angular/doing-tasks/package*.json ./
RUN npm ci
COPY src/frontend/Angular/doing-tasks/ .
RUN npm run build -- --configuration production

FROM nginx:alpine AS final
COPY --from=build /app/dist/doing-tasks/browser /usr/share/nginx/html
EXPOSE 80