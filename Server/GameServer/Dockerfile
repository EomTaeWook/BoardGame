FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY Server/GameServer ./Server/GameServer

COPY Client/BoardGame/Assets/Scripts/GameContents ./Client/BoardGame/Assets/Scripts/GameContents

COPY Share ./Share
COPY Server/Dll ./Server/Dll

RUN dotnet restore "Server/GameServer/BG.GameServer.csproj"
RUN dotnet build "Server/GameServer/BG.GameServer.csproj"
RUN dotnet publish "Server/GameServer/BG.GameServer.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App

COPY --from=build-env /App/out ./Server/BG.GameServer

VOLUME ["/App/Server/BG.GameServer/logs"]

WORKDIR /App/Server/BG.GameServer

ENTRYPOINT ["dotnet", "BG.GameServer.dll"]