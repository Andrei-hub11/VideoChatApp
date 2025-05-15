FROM quay.io/keycloak/keycloak:latest

# Copy the exported realm file into the container
COPY realm-export.json /opt/keycloak/data/import/realm-export.json

# Set the environment variables needed for Keycloak
ENV KEYCLOAK_ADMIN=admin
ENV KEYCLOAK_ADMIN_PASSWORD=admin

# Run Keycloak with realm import
CMD ["start-dev", "--import-realm"]
