FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy csproj
COPY BadNews.csproj ./

# Restore
RUN dotnet restore

# Copy source
COPY . ./

# Build
RUN dotnet build -c Release -o /app/build

# Publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/build .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BadNews.dll"]
