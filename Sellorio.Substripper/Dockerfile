# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.



# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
# Install curl and xz-utils for downloading and extracting FFmpeg
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl xz-utils ca-certificates && \
    rm -rf /var/lib/apt/lists/*
# Download and install static FFmpeg binary
RUN curl -L https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz \
    | tar xJ && \
    mv ffmpeg-*-amd64-static/ffmpeg /usr/local/bin/ && \
    mv ffmpeg-*-amd64-static/ffprobe /usr/local/bin/ && \
    rm -rf ffmpeg-*-amd64-static
# Verify
RUN ffmpeg -version
USER $APP_UID
WORKDIR /app




# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sellorio.Substripper/Sellorio.Substripper.csproj", "Sellorio.Substripper/"]
RUN dotnet restore "./Sellorio.Substripper/Sellorio.Substripper.csproj"
COPY . .
WORKDIR "/src/Sellorio.Substripper"
RUN dotnet build "./Sellorio.Substripper.csproj" -c $BUILD_CONFIGURATION -o /app/build




# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sellorio.Substripper.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false




# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sellorio.Substripper.dll"]