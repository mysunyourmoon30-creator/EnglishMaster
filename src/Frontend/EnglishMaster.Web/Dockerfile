# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["NuGet.Config", "./"]
COPY ["src/Frontend/EnglishMaster.Web/EnglishMaster.Web.csproj", "src/Frontend/EnglishMaster.Web/"]
COPY ["src/Backend/EnglishMaster.Contracts/EnglishMaster.Contracts.csproj", "src/Backend/EnglishMaster.Contracts/"]
COPY ["src/Backend/EnglishMaster.Shared/EnglishMaster.Shared.csproj", "src/Backend/EnglishMaster.Shared/"]

RUN dotnet restore "src/Frontend/EnglishMaster.Web/EnglishMaster.Web.csproj"

COPY . .
RUN dotnet publish "src/Frontend/EnglishMaster.Web/EnglishMaster.Web.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM base AS final
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EnglishMaster.Web.dll"]
