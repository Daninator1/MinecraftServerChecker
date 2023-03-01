FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MinecraftServerChecker.Console/MinecraftServerChecker.Console.csproj", "MinecraftServerChecker.Console/"]
RUN dotnet restore "MinecraftServerChecker.Console/MinecraftServerChecker.Console.csproj"
COPY . .
WORKDIR "/src/MinecraftServerChecker.Console"
RUN dotnet build "MinecraftServerChecker.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinecraftServerChecker.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinecraftServerChecker.Console.dll"]
