# Build Aþamasý
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarýný (csproj) kopyalýyoruz.
# Docker build context "Solution Root" olacaðý için yollarý böyle veriyoruz.
COPY ["API/API.csproj", "API/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Entities/Entities.csproj", "Entities/"]

# Baðýmlýlýklarý yüklüyoruz
RUN dotnet restore "API/API.csproj"

# Tüm dosyalarý kopyalýyoruz
COPY . .

# Build ve Publish iþlemi
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Aþamasý (Uygulamayý çalýþtýracak hafif imaj)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]