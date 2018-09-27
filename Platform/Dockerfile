FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 20000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Platform/Platform.csproj Platform/
RUN dotnet restore Platform/Platform.csproj
COPY . .
WORKDIR /src/Platform
RUN dotnet build Platform.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Platform.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Platform.dll"]