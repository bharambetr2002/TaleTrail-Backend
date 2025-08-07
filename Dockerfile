# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY TaleTrail.API/*.csproj ./TaleTrail.API/
RUN dotnet restore ./TaleTrail.API/TaleTrail.API.csproj

# Copy the entire source
COPY . .

# Build the app
RUN dotnet publish TaleTrail.API/TaleTrail.API.csproj -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "TaleTrail.API.dll"]
