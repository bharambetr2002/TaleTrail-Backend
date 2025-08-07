# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY TaleTrail.API/TaleTrail.API.csproj TaleTrail.API/
RUN dotnet restore TaleTrail.API/TaleTrail.API.csproj

# Copy the rest of the application code
COPY . .

# Build the application
WORKDIR /app/TaleTrail.API
RUN dotnet build TaleTrail.API.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish TaleTrail.API.csproj -c Release -o /app/publish

# Use the official .NET 8 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN addgroup --system --gid 1001 dotnetuser
RUN adduser --system --uid 1001 --gid 1001 dotnetuser

# Copy the published application
COPY --from=build /app/publish .

# Create logs directory and set permissions
RUN mkdir -p /app/logs && chown -R dotnetuser:dotnetuser /app

# Switch to non-root user
USER dotnetuser

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "TaleTrail.API.dll"]