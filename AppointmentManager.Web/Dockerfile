FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AppointmentManager.Web/AppointmentManager.Web.csproj", "AppointmentManager.Web/"]
RUN dotnet restore "AppointmentManager.Web/AppointmentManager.Web.csproj"
COPY . .
WORKDIR "/src/AppointmentManager.Web"
RUN dotnet build "AppointmentManager.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentManager.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentManager.Web.dll"]
