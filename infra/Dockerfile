# syntax=docker/dockerfile:1
# Build: docker build --build-arg PROJECT_PATH=src/Gateway --build-arg PROJECT_NAME=SystemDesign.Payments.Gateway -f infra/Dockerfile .
ARG PROJECT_PATH
ARG PROJECT_NAME

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJECT_PATH
ARG PROJECT_NAME
WORKDIR /src
COPY Directory.Build.props ./
COPY src/Shared.Contracts/ src/Shared.Contracts/
COPY ${PROJECT_PATH}/ ${PROJECT_PATH}/
RUN dotnet restore "${PROJECT_PATH}/${PROJECT_NAME}.csproj" \
 && dotnet publish "${PROJECT_PATH}/${PROJECT_NAME}.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
# Override with: command: ["dotnet", "SystemDesign.Payments.<Service>.dll"]
