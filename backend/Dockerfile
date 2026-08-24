FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/ParkAhead.Domain/*.csproj src/ParkAhead.Domain/
COPY src/ParkAhead.Application/*.csproj src/ParkAhead.Application/
COPY src/ParkAhead.Infrastructure/*.csproj src/ParkAhead.Infrastructure/
COPY src/ParkAhead.Api/*.csproj src/ParkAhead.Api/
RUN dotnet restore src/ParkAhead.Api/ParkAhead.Api.csproj

COPY src/ src/
RUN dotnet publish src/ParkAhead.Api/ParkAhead.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ParkAhead.Api.dll"]
