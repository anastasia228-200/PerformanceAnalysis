# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PerformanceAnalysis.API/PerformanceAnalysis.API.csproj", "PerformanceAnalysis.API/"]
COPY ["PerformanceAnalysis.Application/PerformanceAnalysis.Application.csproj", "PerformanceAnalysis.Application/"]
COPY ["PerformanceAnalysis.Infrastructure/PerformanceAnalysis.Infrastructure.csproj", "PerformanceAnalysis.Infrastructure/"]
COPY ["PerformanceAnalysis.Domain/PerformanceAnalysis.Domain.csproj", "PerformanceAnalysis.Domain/"]
RUN dotnet restore "PerformanceAnalysis.API/PerformanceAnalysis.API.csproj"

COPY . .
WORKDIR "/src/PerformanceAnalysis.API"
RUN dotnet build "PerformanceAnalysis.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PerformanceAnalysis.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PerformanceAnalysis.API.dll"]
