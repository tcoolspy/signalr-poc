#FROM ubuntu:latest
#LABEL authors="C0350"
#
#ENTRYPOINT ["top", "-b"]

# Stage 1: Build the Angular application
FROM node:21-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build -- --configuration development
#RUN npm run build -- --configuration production

# Stage 2: Serve the application with Nginx
FROM nginx:alpine
COPY --from=builder /app/dist/FoodOrderingClient /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
