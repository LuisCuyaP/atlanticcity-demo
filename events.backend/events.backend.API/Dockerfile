# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first for better layer caching
COPY events.backend.API/events.backend.API.csproj events.backend.API/
COPY events.backend.Application/events.backend.Application.csproj events.backend.Application/
COPY events.backend.Infrastructure/events.backend.Infrastructure.csproj events.backend.Infrastructure/
COPY events.backend.Domain/events.backend.Domain.csproj events.backend.Domain/
COPY events.backend.CrossCutting/events.backend.CrossCutting.csproj events.backend.CrossCutting/

RUN dotnet restore events.backend.API/events.backend.API.csproj

# Copy the rest of the source
COPY . .

RUN dotnet publish events.backend.API/events.backend.API.csproj -c Release -o /app/publish --no-restore


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "events.backend.API.dll"]
