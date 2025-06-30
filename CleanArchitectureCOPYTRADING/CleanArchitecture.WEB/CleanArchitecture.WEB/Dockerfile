FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia de los .csproj necesarios
COPY ["CleanArchitecture.WEB/CleanArchitecture.WEB/CleanArchitecture.WEB.csproj", "CleanArchitecture.WEB/CleanArchitecture.WEB/"]
COPY ["CleanArchitecture.Application/CleanArchitecture.Application.csproj", "CleanArchitecture.Application/"]
COPY ["CleanArchitecture.Domain/CleanArchitecture.Domain.csproj", "CleanArchitecture.Domain/"]
COPY ["CleanArchitecture.Data/CleanArchitecture.Infrastructure.csproj", "CleanArchitecture.Data/"]
COPY ["CleanArchitecture.Identity/CleanArchitecture.Identity.csproj", "CleanArchitecture.Identity/"]

# Restauración de paquetes
RUN dotnet restore "CleanArchitecture.WEB/CleanArchitecture.WEB/CleanArchitecture.WEB.csproj"

# Copia todo el código fuente
COPY . .

# Build del proyecto
WORKDIR "/src/CleanArchitecture.WEB/CleanArchitecture.WEB"
RUN dotnet build "CleanArchitecture.WEB.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CleanArchitecture.WEB.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ✅ Copiar los modelos ML.NET al contenedor
# Asegúrate que la ruta es correcta: wwwroot/Model/
COPY ["CleanArchitecture.WEB/CleanArchitecture.WEB/wwwroot/Model", "wwwroot/Model"]

ENTRYPOINT ["dotnet", "CleanArchitecture.WEB.dll"]
