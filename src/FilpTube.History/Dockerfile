#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/FilpTube.History/FilpTube.History.csproj", "src/FilpTube.History/"]
RUN dotnet restore "src/FilpTube.History/FilpTube.History.csproj"
COPY . .
WORKDIR "/src/src/FilpTube.History"
RUN dotnet build "FilpTube.History.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FilpTube.History.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FilpTube.History.dll"]