FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ContainerWatchlistAPI.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ContainerWatchlistAPI.dll"]

