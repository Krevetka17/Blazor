# Используем .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы и собираем приложение
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /publish

# Используем Nginx для раздачи статических файлов (если Blazor WASM)
FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /publish/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
