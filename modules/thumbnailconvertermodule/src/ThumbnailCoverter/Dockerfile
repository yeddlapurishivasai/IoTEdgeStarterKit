#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-bionic AS base
WORKDIR /app

# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src

COPY ["modules/thumbnailconvertermodule/src/ThumbnailCoverter/ThumbnailCoverter.csproj", "modules/thumbnailconvertermodule/src/ThumbnailCoverter/"]
RUN dotnet restore "modules/thumbnailconvertermodule/src/ThumbnailCoverter/ThumbnailCoverter.csproj"
COPY . .
WORKDIR "/src/modules/thumbnailconvertermodule/src/ThumbnailCoverter"
RUN dotnet build "ThumbnailCoverter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ThumbnailCoverter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThumbnailCoverter.dll"]