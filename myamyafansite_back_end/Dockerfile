FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN apt-get update && apt-get install -y \
    ffmpeg \
    libavcodec-extra \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["myamyafansite_back_end/myamyafansite_back_end.csproj", "myamyafansite_back_end/"]
RUN dotnet restore "myamyafansite_back_end/myamyafansite_back_end.csproj"
COPY . .
WORKDIR "/src/myamyafansite_back_end"
RUN dotnet build "myamyafansite_back_end.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "myamyafansite_back_end.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "myamyafansite_back_end.dll"]
