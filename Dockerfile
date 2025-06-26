# -------- Stage 1: Build --------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /app
    
    # Copy csproj and restore dependencies
    COPY TaleTrail.API/*.csproj ./TaleTrail.API/
    RUN dotnet restore ./TaleTrail.API/TaleTrail.API.csproj
    
    # Copy everything and publish
    COPY . .
    WORKDIR /app/TaleTrail.API
    RUN dotnet publish -c Release -o /app/out
    
    # -------- Stage 2: Runtime --------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    
    # Copy from the build output
    COPY --from=build /app/out ./
    
    # Start the app
    ENTRYPOINT ["dotnet", "TaleTrail.API.dll"]
    