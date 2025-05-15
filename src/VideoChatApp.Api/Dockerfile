#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/VideoChatApp.Api/VideoChatApp.Api.csproj", "src/VideoChatApp.Api/"]
COPY ["src/VideoChatApp.Application/VideoChatApp.Application.csproj", "src/VideoChatApp.Application/"]
COPY ["src/VideoChatApp.Common/VideoChatApp.Common.csproj", "src/VideoChatApp.Common/"]
COPY ["src/VideoChatApp.Contracts/VideoChatApp.Contracts.csproj", "src/VideoChatApp.Contracts/"]
COPY ["src/VideoChatApp.Domain/VideoChatApp.Domain.csproj", "src/VideoChatApp.Domain/"]
COPY ["src/VideoChatApp.Infrastructure/VideoChatApp.Infrastructure.csproj", "src/VideoChatApp.Infrastructure/"]
RUN dotnet restore "./src/VideoChatApp.Api/VideoChatApp.Api.csproj"
COPY . .
WORKDIR "/src/src/VideoChatApp.Api"
RUN dotnet build "./VideoChatApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VideoChatApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoChatApp.Api.dll"]