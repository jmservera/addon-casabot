ARG BUILD_FROM

# Multi-arch build arguments
ARG BUILD_ARCH

# Build stage - use .NET SDK for compilation
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder

WORKDIR /src

# Copy source code
COPY ./src/casabot/ ./

RUN echo $TARGETPLATFORM > /platform.txt
# Build application with appropriate runtime identifier
RUN case "${TARGETPLATFORM}" in \
      "amd64") \
        dotnet publish -c Release -r linux-musl-x64 -o /app/publish \
        ;; \
      "aarch64") \
        dotnet publish -c Release -r linux-musl-arm64 -o /app/publish \
        ;; \
      "armv7") \
        dotnet publish -c Release -r linux-musl-arm -o /app/publish \
        ;; \
      *) \
        echo "Unsupported architecture: ${TARGETPLATFORM}" && exit 1 \
        ;; \
    esac

# Runtime stage
FROM $BUILD_FROM

# Set shell
SHELL ["/bin/bash", "-o", "pipefail", "-c"]

# Setup base system
# ARG TEMPIO_VERSION="2021.09.0"
# RUN curl -L -s "https://github.com/home-assistant/tempio/releases/download/${TEMPIO_VERSION}/tempio_${TEMPIO_ARCH}" \
#         -o /usr/bin/tempio \
#     && chmod a+x /usr/bin/tempio

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
COPY ./casabot/rootfs/ /

# Set permissions
RUN \
  chmod a+x /etc/services.d/casabot/run \
  && chmod a+x /etc/services.d/casabot/finish \
  && chmod a+x /etc/services.d/nginx/run \
  && chmod a+x /etc/services.d/nginx/finish \
  && chmod a+x /run.sh

# Set entrypoint
ENTRYPOINT ["/init"]