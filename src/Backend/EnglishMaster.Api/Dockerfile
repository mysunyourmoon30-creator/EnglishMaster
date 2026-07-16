# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["NuGet.Config", "./"]
COPY ["src/Backend/EnglishMaster.Api/EnglishMaster.Api.csproj", "src/Backend/EnglishMaster.Api/"]
COPY ["src/Backend/EnglishMaster.Application/EnglishMaster.Application.csproj", "src/Backend/EnglishMaster.Application/"]
COPY ["src/Backend/EnglishMaster.Contracts/EnglishMaster.Contracts.csproj", "src/Backend/EnglishMaster.Contracts/"]
COPY ["src/Backend/EnglishMaster.Domain/EnglishMaster.Domain.csproj", "src/Backend/EnglishMaster.Domain/"]
COPY ["src/Backend/EnglishMaster.Infrastructure/EnglishMaster.Infrastructure.csproj", "src/Backend/EnglishMaster.Infrastructure/"]
COPY ["src/Backend/EnglishMaster.Shared/EnglishMaster.Shared.csproj", "src/Backend/EnglishMaster.Shared/"]

RUN dotnet restore "src/Backend/EnglishMaster.Api/EnglishMaster.Api.csproj"

COPY . .
RUN dotnet publish "src/Backend/EnglishMaster.Api/EnglishMaster.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM base AS final
RUN mkdir -p /app/media /app/publishing && chown -R "$APP_UID":0 /app
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EnglishMaster.Api.dll"]
