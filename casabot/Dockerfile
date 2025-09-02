ARG BUILD_FROM
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder
WORKDIR /src
COPY ../src/casabot/ ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM $BUILD_FROM

# Set shell
SHELL ["/bin/bash", "-o", "pipefail", "-c"]

# Setup base system
ARG TEMPIO_VERSION="2021.09.0"
RUN \
    curl -L -s "https://github.com/home-assistant/tempio/releases/download/${TEMPIO_VERSION}/tempio_amd64" \
        -o /usr/bin/tempio \
    && chmod a+x /usr/bin/tempio

# Install s6-overlay, nginx and .NET runtime
RUN \
  apk add --no-cache \
    nginx \
    aspnetcore8-runtime \
    s6-overlay

# Create nginx directories
RUN \
  mkdir -p /var/log/nginx \
  && mkdir -p /var/lib/nginx/tmp \
  && mkdir -p /run/nginx

# Copy CasaBot application
COPY --from=builder /app/publish /opt/casabot/

# Copy nginx configuration and s6 services
COPY rootfs/ /

# Set permissions
RUN \
  chmod a+x /etc/services.d/casabot/run \
  && chmod a+x /etc/services.d/casabot/finish \
  && chmod a+x /etc/services.d/nginx/run \
  && chmod a+x /etc/services.d/nginx/finish \
  && chmod a+x /run.sh

# Set entrypoint
ENTRYPOINT ["/init"]