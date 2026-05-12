FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["src/ArchivalRecording.Scrapper/ArchivalRecording.Scrapper.csproj", "src/ArchivalRecording.Scrapper/"]
COPY ["src/ArchivalRecording.Shared/ArchivalRecording.Shared.csproj", "src/ArchivalRecording.Shared/"]
RUN dotnet restore "src/ArchivalRecording.Scrapper/ArchivalRecording.Scrapper.csproj"

COPY src/ src/
RUN dotnet publish "src/ArchivalRecording.Scrapper/ArchivalRecording.Scrapper.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ArchivalRecording.Scrapper.dll"]
