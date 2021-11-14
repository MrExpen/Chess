#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ChessHttpServer/ChessHttpServer.csproj", "ChessHttpServer/"]
COPY ["ChessLib/ChessLib.csproj", "ChessLib/"]
RUN dotnet restore "ChessHttpServer/ChessHttpServer.csproj"
COPY . .
WORKDIR "/src/ChessHttpServer"
RUN dotnet build "ChessHttpServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChessHttpServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChessHttpServer.dll"]
