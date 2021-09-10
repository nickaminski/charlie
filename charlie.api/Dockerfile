FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["charlie.api/*.csproj", "./charlie.api/"]
COPY ["charlie.bll/*.csproj", "./charlie.bll/"]
COPY ["charlie.dto/*.csproj", "./charlie.dto/"]
COPY ["charlie.dal/*.csproj", "./charlie.dal/"]
COPY ["charlie.security/*.csproj", "charlie.security/"]
RUN dotnet restore -r linux-arm "charlie.api/charlie.api.csproj"
COPY . .
WORKDIR "/src/charlie.api"
RUN dotnet build "charlie.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "charlie.api.csproj" -c Release -o /app/publish -r linux-arm --self-contained false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm32v7
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "charlie.api.dll" ]
# C:\Users\Nic\source\repos\charlie> docker build -t nickleas/charlie:arm7 -f charlie.api/Dockerfile .
# C:\Users\Nic\source\repos\charlie> docker push nickleas/charlie:arm7