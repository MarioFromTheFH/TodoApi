# 1. BUILD-STAGE: Nutzt das SDK-Image, um das Projekt zu kompilieren und zu veröffentlichen.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Wir stellen sicher, dass wir nur die TodoApi-Projektebene betrachten.
# Wir kopieren die gesamte Projektdatei (oder nur die .csproj) in ein Unterverzeichnis,
# um den Kontext im folgenden WORKDIR sauber zu halten.

# Kopiere die .csproj-Datei, um Abhängigkeiten (NuGet-Pakete) wiederherzustellen.
# Der Pfad muss relativ zum Kontext (./TodoApi) sein.
COPY TodoApi.csproj .

# Stelle Abhängigkeiten wieder her.
RUN dotnet restore

# Kopiere den gesamten Rest des Projektordners (den Quellcode)
COPY . .

# Führe den Build und Publish durch.
# Da sich die .csproj jetzt im WORKDIR /src befindet, muss sie nur mit ihrem Namen referenziert werden.
RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish --no-restore

# 2. FINAL-STAGE: Nutzt das schlankere ASP.NET Laufzeit-Image.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
# ASP.NET Core Container nutzen standardmäßig Port 8080 oder 80.
# Wir nutzen 8080, da die Compose-Datei es auf 5251 mappt.
EXPOSE 8080 

# Kopiere die veröffentlichten Dateien aus der 'build'-Stage.
COPY --from=build /app/publish .

# Definiert den Einstiegspunkt für die Anwendung.
# Wichtig: Der Name der .dll muss mit dem Namen des Projekts übereinstimmen.
ENTRYPOINT ["dotnet", "TodoApi.dll"]