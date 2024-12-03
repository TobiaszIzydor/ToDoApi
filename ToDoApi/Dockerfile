# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ToDoApi/ToDoApi.csproj", "ToDoApi/"]
COPY ["ToDo.Application/ToDo.Application.csproj", "ToDo.Application/"]
COPY ["ToDo.Domain/ToDo.Domain.csproj", "ToDo.Domain/"]
COPY ["ToDo.Infrastructure/ToDo.Infrastructure.csproj", "ToDo.Infrastructure/"]
RUN dotnet restore "./ToDoApi/ToDoApi.csproj"
COPY . .
WORKDIR "/src/ToDoApi"
RUN dotnet build "./ToDoApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ToDoApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoApi.dll"]