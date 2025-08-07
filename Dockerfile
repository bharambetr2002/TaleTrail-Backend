# Multi-stage build optimized for Render
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Set working directory
WORKDIR /src

# Copy project file and restore dependencies
COPY ["TaleTrail.API/TaleTrail.API.csproj", "TaleTrail.API/"]
RUN dotnet restore "TaleTrail.API/TaleTrail.API.csproj" --disable-parallel

# Copy source code and build
COPY . .
WORKDIR "/src/TaleTrail.API"
RUN dotnet build "TaleTrail.API.csproj" -c Release -o /app/build --no-restore

# Publish
RUN dotnet publish "TaleTrail.API.csproj" -c Release -o /app/publish --no-restore --no-build

# Final stage - runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Install curl for health checks
RUN apk add --no-cache curl

# Create app directory and user
WORKDIR /app
RUN addgroup -g 1001 -S dotnetuser && \
    adduser -S dotnetuser -u 1001 -G dotnetuser

# Copy published app
COPY --from=build /app/publish .

# Create logs directory and set ownership
RUN mkdir -p /app/logs && \
    chown -R dotnetuser:dotnetuser /app

# Switch to non-root user
USER dotnetuser

# Configure environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:10000/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "TaleTrail.API.dll"]