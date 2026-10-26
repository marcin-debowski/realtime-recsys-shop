# One Dockerfile for both backend processes (ADR-0002, ADR-0014).
#   docker build --target api .
#   docker build --target event-writer .
ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Restore first, so the layer is cached until a .csproj changes
COPY src/Shop.Core/Shop.Core.csproj src/Shop.Core/
COPY src/Shop.Infrastructure/Shop.Infrastructure.csproj src/Shop.Infrastructure/
COPY src/Shop.Api/Shop.Api.csproj src/Shop.Api/
COPY src/Shop.EventWriter/Shop.EventWriter.csproj src/Shop.EventWriter/
RUN dotnet restore src/Shop.Api/Shop.Api.csproj \
 && dotnet restore src/Shop.EventWriter/Shop.EventWriter.csproj

COPY src/ src/
RUN dotnet publish src/Shop.Api/Shop.Api.csproj -c Release --no-restore -o /out/api \
 && dotnet publish src/Shop.EventWriter/Shop.EventWriter.csproj -c Release --no-restore -o /out/event-writer

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS api
WORKDIR /app
COPY --from=build /out/api .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Shop.Api.dll"]

FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_VERSION} AS event-writer
WORKDIR /app
COPY --from=build /out/event-writer .
USER app
ENTRYPOINT ["dotnet", "Shop.EventWriter.dll"]
