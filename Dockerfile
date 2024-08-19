FROM mcr.microsoft.com/dotnet/runtime:8.0.8-bookworm-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0.401-bookworm-slim-arm32v7 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["charlie.api/charlie.api.csproj", "charlie.api/"]
COPY ["charlie.bll/charlie.bll.csproj", "charlie.bll/"]
COPY ["charlie.common/charlie.common.csproj", "charlie.common/"]
COPY ["charlie.dal/charlie.dal.csproj", "charlie.dal/"]
COPY ["charlie.dto/charlie.dto.csproj", "charlie.dto/"]
COPY ["charlie.security/charlie.security.csproj", "charlie.security/"]
RUN dotnet restore "charlie.api/charlie.api.csproj"
COPY . .
WORKDIR "/src/charlie.api"
RUN dotnet build "charlie.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "charlie.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "charlie.api.dll" ]
# C:\Users\Nic\source\repos\charlie> docker build --platform linux/arm/v7 -t nickleas/charlie:arm32v7 .
# C:\Users\Nic\source\repos\charlie> docker push nickleas/charlie:arm32v7