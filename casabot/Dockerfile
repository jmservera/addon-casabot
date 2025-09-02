ARG BUILD_FROM
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Set working directory
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY . ./

# Build and publish the application
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM $BUILD_FROM

# Install .NET runtime
RUN \
  apk add --no-cache \
    aspnetcore8-runtime \
    dotnet8-runtime

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=build /app/publish/ ./

# Expose port
EXPOSE 8000

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:8000
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
CMD ["dotnet", "CasaBot.dll"]