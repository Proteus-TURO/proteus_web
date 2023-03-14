FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ProteusWeb/ProteusWeb.csproj", "ProteusWeb/"]
RUN dotnet restore "ProteusWeb/ProteusWeb.csproj"
COPY . .
WORKDIR "/src/ProteusWeb"
RUN dotnet build "ProteusWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProteusWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProteusWeb.dll"]
