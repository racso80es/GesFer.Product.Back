FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Api/GesFer.Api.csproj", "src/Api/"]
COPY ["src/application/GesFer.Application.csproj", "src/application/"]
COPY ["src/domain/GesFer.Domain.csproj", "src/domain/"]
COPY ["src/Infrastructure/GesFer.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Api/GesFer.Api.csproj"
COPY src/Api/ ./src/Api/
COPY src/application/ ./src/application/
COPY src/domain/ ./src/domain/
COPY src/Infrastructure/ ./src/Infrastructure/
WORKDIR "/src/src/Api"
RUN dotnet build "GesFer.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/src/Api"
RUN dotnet publish "GesFer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
# Install curl for healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GesFer.Api.dll"]
