# Use aspnet runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set the environment to Development so the application uses appsettings.Development.json
ENV ASPNETCORE_ENVIRONMENT=Development

# Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Elepla.API/Elepla.API.csproj", "Elepla.API/"]
COPY ["Elepla.Service/Elepla.Service.csproj", "Elepla.Service/"]
COPY ["Elepla.Repository/Elepla.Repository.csproj", "Elepla.Repository/"]
COPY ["Elepla.Domain/Elepla.Domain.csproj", "Elepla.Domain/"]
RUN dotnet restore "./Elepla.API/Elepla.API.csproj"
COPY . .
WORKDIR "/src/Elepla.API"
RUN dotnet build "./Elepla.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Elepla.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: setup runtime environment
FROM base AS final
WORKDIR /app

# Copy the publish folder
COPY --from=publish /app/publish .

# Copy appsettings.Development.json to the container
COPY ["Elepla.API/appsettings.Development.json", "./"]

# Start the application
ENTRYPOINT ["dotnet", "Elepla.API.dll"]
