# Базовый образ с ASP.NET для продакшена
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DalaProject.csproj", "./"]
RUN dotnet restore "DalaProject.csproj"
COPY . .
RUN dotnet build "DalaProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DalaProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DalaProject.dll"]