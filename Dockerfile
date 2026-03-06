FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY GuideAPI.DAL/GuideAPI.DAL.csproj GuideAPI.DAL/
COPY GuideAPI/GuideAPI.csproj GuideAPI/

# Restore dependencies
RUN dotnet restore GuideAPI/GuideAPI.csproj

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish GuideAPI/GuideAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render uses PORT env variable
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "GuideAPI.dll"]
