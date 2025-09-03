#!/usr/bin/with-contenv bashio

# This script is no longer the main entrypoint
# It's kept for configuration parsing and could be used by the s6 service
# The main entrypoint is now /init (s6-overlay)

bashio::log.info "CasaBot add-on configuration script"

# Parse configuration options
CONFIG_PATH="/data/options.json"

if bashio::fs.file_exists "${CONFIG_PATH}"; then
    bashio::log.info "Loading configuration from ${CONFIG_PATH}"
    # Add any configuration parsing logic here
else
    bashio::log.info "No configuration file found at ${CONFIG_PATH}"
fi

# Export any needed environment variables for the CasaBot application
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:8000"

bashio::log.info "Configuration complete"