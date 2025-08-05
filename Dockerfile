# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["TaleTrail.API/TaleTrail.API.csproj", "TaleTrail.API/"]
RUN dotnet restore "TaleTrail.API/TaleTrail.API.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/TaleTrail.API"
RUN dotnet build "TaleTrail.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TaleTrail.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Set environment to production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV RENDER=true

# Render will provide the PORT environment variable
EXPOSE $PORT

# Use HTTP only (Render handles HTTPS termination)
ENV ASPNETCORE_URLS=http://*:$PORT

ENTRYPOINT ["dotnet", "TaleTrail.API.dll"]