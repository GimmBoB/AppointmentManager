FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AppointmentManager.API/AppointmentManager.API.csproj", "AppointmentManager.API/"]
RUN dotnet restore "AppointmentManager.API/AppointmentManager.API.csproj"
COPY . .
WORKDIR "/src/AppointmentManager.API"
RUN dotnet build "AppointmentManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentManager.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentManager.API.dll"]
