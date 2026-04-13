# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости отдельным слоем (кэшируется)
COPY ["PerformanceAnalysis/PerformanceAnalysis.csproj", "PerformanceAnalysis/"]
RUN dotnet restore "PerformanceAnalysis/PerformanceAnalysis.csproj"

# Копируем всё остальное и собираем
COPY PerformanceAnalysis/ PerformanceAnalysis/
WORKDIR "/src/PerformanceAnalysis"
RUN dotnet build "PerformanceAnalysis.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PerformanceAnalysis.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage — только ASP.NET runtime, без SDK (меньший образ)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PerformanceAnalysis.dll"]
