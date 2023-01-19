#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/FilpTube.VideoStorage/FilpTube.VideoStorage.csproj", "src/FilpTube.VideoStorage/"]
RUN dotnet restore "src/FilpTube.VideoStorage/FilpTube.VideoStorage.csproj"
COPY . .
WORKDIR "/src/src/FilpTube.VideoStorage"
RUN dotnet build "FilpTube.VideoStorage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FilpTube.VideoStorage.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FilpTube.VideoStorage.dll"]