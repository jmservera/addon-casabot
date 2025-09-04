ARG BUILD_FROM=ghcr.io/home-assistant/amd64-base:3.21
ARG BUILD_ARCH=amd64

# Runtime stage - no build stage needed, use pre-built packages
FROM $BUILD_FROM

# Set shell
SHELL ["/bin/bash", "-o", "pipefail", "-c"]

# Install .NET runtime only (no SDK needed)
RUN \
  apk add --no-cache \
    nginx \
    aspnetcore8-runtime \
    curl \
  && rm -fr \
    /etc/nginx \
    /root/.cache \
    /root/.npm \
    /root/.nrpmrc \
    /tmp/*

# Download pre-built CasaBot application from releases
ARG BUILD_ARCH
ARG CASABOT_VERSION="0.1.0"
ARG CASABOT_REPO="jmservera/casabot"

RUN \
    mkdir -p /opt/casabot && \
    case "${BUILD_ARCH}" in \
      "amd64") \
        ASSET_NAME="casabot-amd64.tar.gz" \
        ;; \
      "aarch64") \
        ASSET_NAME="casabot-aarch64.tar.gz" \
        ;; \
      "armv7") \
        ASSET_NAME="casabot-armv7.tar.gz" \
        ;; \
      *) \
        echo "Unsupported architecture: ${BUILD_ARCH}" && exit 1 \
        ;; \
    esac && \
    if [ "${CASABOT_VERSION}" = "latest" ]; then \
      DOWNLOAD_URL=$(curl -s https://api.github.com/repos/${CASABOT_REPO}/releases/latest | \
        grep "browser_download_url.*${ASSET_NAME}" | \
        cut -d '"' -f 4); \
    else \
      DOWNLOAD_URL="https://github.com/${CASABOT_REPO}/releases/download/v${CASABOT_VERSION}/${ASSET_NAME}"; \
    fi && \
    echo "Downloading from: ${DOWNLOAD_URL}" && \
    curl -L "${DOWNLOAD_URL}" | tar -xz -C /opt/casabot && \
    chmod +x /opt/casabot/CasaBot

# Copy nginx configuration and s6 services
COPY rootfs/ /

HEALTHCHECK --start-period=10m \
    CMD curl --fail http://127.0.0.1:8000 || exit 1

# Build arguments for labels
ARG BUILD_ARCH
ARG BUILD_DATE
ARG BUILD_DESCRIPTION
ARG BUILD_NAME
ARG BUILD_REF
ARG BUILD_REPOSITORY
ARG BUILD_VERSION

# Labels
LABEL \
    io.hass.name="${BUILD_NAME}" \
    io.hass.description="${BUILD_DESCRIPTION}" \
    io.hass.arch="${BUILD_ARCH}" \
    io.hass.type="addon" \
    io.hass.version=${BUILD_VERSION} \
    maintainer="Juanma <jmservera@users.noreply.github.com>" \
    org.opencontainers.image.title="${BUILD_NAME}" \
    org.opencontainers.image.description="${BUILD_DESCRIPTION}" \
    org.opencontainers.image.vendor="Home Assistant Community Add-ons" \
    org.opencontainers.image.authors="Juanma <jmservera@users.noreply.github.com>" \
    org.opencontainers.image.licenses="MIT" \
    org.opencontainers.image.url="https://addons.community" \
    org.opencontainers.image.source="https://github.com/${BUILD_REPOSITORY}" \
    org.opencontainers.image.documentation="https://github.com/${BUILD_REPOSITORY}/blob/main/README.md" \
    org.opencontainers.image.created=${BUILD_DATE} \
    org.opencontainers.image.revision=${BUILD_REF} \
    org.opencontainers.image.version=${BUILD_VERSION}
