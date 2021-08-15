# build stage
FROM node:12 as build-stage
WORKDIR /app
COPY ["src/Presentation/Chat.VuejsApp/package*.json", "."]
RUN ls -al
RUN npm install
COPY ./src/Presentation/Chat.VuejsApp/ .
RUN npm run build

# production stage
FROM nginx:stable-alpine as production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]