FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["CarServiceBookingSystem.API/CarServiceBookingSystem.API.csproj", "CarServiceBookingSystem.API/"]
COPY ["CarServiceBookingSystem.Application/CarServiceBookingSystem.Application.csproj", "CarServiceBookingSystem.Application/"]
COPY ["CarServiceBookingSystem.Domain/CarServiceBookingSystem.Domain.csproj", "CarServiceBookingSystem.Domain/"]
COPY ["CarServiceBookingSystem.Infrastructure/CarServiceBookingSystem.Infrastructure.csproj", "CarServiceBookingSystem.Infrastructure/"]

RUN dotnet restore "CarServiceBookingSystem.API/CarServiceBookingSystem.API.csproj"

COPY . .

WORKDIR "/src/CarServiceBookingSystem.API"
RUN dotnet publish "CarServiceBookingSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CarServiceBookingSystem.API.dll"]