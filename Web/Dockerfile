FROM node:latest as build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY [".", "."]

RUN npm run build -- --output-path=dist/out

FROM nginx:latest

COPY --from=build /app/dist/out/ /var/www/dist

COPY ["nginx.conf", "/etc/nginx/"]
