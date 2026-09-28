# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PortfolioCMS.csproj .
RUN dotnet restore PortfolioCMS.csproj

COPY . .
RUN dotnet publish PortfolioCMS.csproj -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Non-root user: the aspnet base image ships one already
USER app

COPY --from=build --chown=app:app /app/publish .

# Uploaded files live under ContentRootPath/uploads/files - mount a volume
# here in compose so they survive container recreation.
VOLUME ["/app/uploads"]

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PortfolioCMS.dll"]
