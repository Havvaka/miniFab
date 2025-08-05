# Frontend Dockerfile
FROM node:18-alpine AS build

WORKDIR /app

# Package.json ve package-lock.json dosyalarını kopyala
COPY package*.json ./

# Dependencies'leri yükle (dev dependencies dahil build için gerekli)
RUN npm ci

# Kaynak kodunu kopyala
COPY . .

# Uygulamayı build et
RUN npm run build

# Production stage
FROM nginx:stable-alpine

# Build edilen dosyaları nginx'e kopyala
COPY --from=build /app/dist /usr/share/nginx/html

# Nginx konfigürasyon dosyası
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
