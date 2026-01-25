Just execute 

```
docker compose up --build
```
I had some issues with the Database resulting in various errors.

If some weird errors occur on startup, consinder running these commands on the host first:

```
dotnet restore
dotnet build
dotnet ef migrations add InitialCreate
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0
dotnet ef migrations add InitialCreate

```
I'd love to have these in the Docker, but apparently it's not encouraged to do so. 
I'll be looking for a better solution. 

# Aufgaben gelöst
## 1 Beschreibung(en)
### 1 Service
Dieses Projekt stellt zwei Services zur Verfügung. Die obere Hierarchische Ebene `ParkingLotCrud`, die es erlaubt, administrativ tätig zu sein:
- `/api/admin/ParkingLotCrud [POST]`: Neuen Parkplatz komplett erzeugen
- `/api/admin/ParkingLotCrud/{id} [GET]`: Komplette Information über einen einzelnen Parkplatz auslesen
- `/api/admin/ParkingLotCrud/{id} [DELETE]`: Einen Parkplatz löschen
- `/api/admin/ParkingLotCrud/{id} [PUT]`: Einen Parkplatz ändern
- `/api/admin/ParkingLotCrud/{adr_id},{park_id}` Einer Adresse einen anderen Parkplatz zuweisen

Die zweite Ebene `Parking` liest direkt Parkplatzinformationen aus die sofort weiterverwendet werden können. 

### 2 Bounded Context
Diese Trennung zwischen administrativer Ebene und der Ebene die für die Benutzer/Entwickler einer bspw. Parkplatz-App interessant ist, ist der Bounded Context. Es wird zwischen Kunden des Parkplatzes (oder Parkplatzdienstleistungen) und dem Inhaber des Parkplatzes entschieden.

### 3 Context Map
Relevanz fraglich, da die Services relativ streng voneinander getrennt sind. 

### 4 Datenmodell
Besteht nur aus Parkplätzen und deren Adressen als eigene Tabellen. Mehr Tabellen würden nur unnötig viel Komplexität erzeugen. Jede Adresse hat genau einen Parkplatz. Das bedeutet im Umkehrschluss, dass ein Parkplatz über mehrere Adressen verfügen könnte. (Was aber effektiv nicht vorkommen kann

### 5 Datenvalidierung
Passiert über das mapping. 

## 2 ASP.NET Core Web.API
### CRUD-Funktionaltitäten
Effektiv dieses Projekt. Der `ParkingLotCrudController` bietet `Create` (erstellen neuer Parkplätze), `Replace`/`Update` (ändern von bestehenden Parkplätzen und Adressen, das überschneidet sich) und `Delete` (löschen von bestehenden Parkplätzen.

###  HTTP-Verben
- `POST` wird zur zur Erzeugung von neuen Parkplätzen (Mit Adresse in eigener Tabelle) verwendet. Post deswegen, damit wir uns mit der Länge keine Gedanken machen müssen
- `GET` wird für alle lesenden Zugriffe verwendet
- `DELETE` zum Entfernen eines Parkplatzes
- `PUT` zum Ändern von Parkplätzen und Neuzuweisungen von Adressen

Hier ein Beispielcode zum Hinzufügen eines neuen Parkplatzes:
```json
{
  "name": "Test-Parkplatz",
  "totalSpots": 75,
  "latitude": 47.449680306633184, 
  "longitude": 15.267162534779795,
  "isUnderground": true,
  "pricePerHour": 10,
  "address": {

    "street": "Finkenweg",
    "houseNumber": "4/z",
    "zipCode": "8605",
    "city": "Kapfenberg"
  }
}

```

### Status-Codes
`200`: Kommt mit den meisten GET-Requests mit, die nur zur Abfrage dienen
`201`: Erfolgreiches Erstellen eines Parkplatzes/Adresse 
`400 Error: Bad Request`: bei fehlerhaftem Input für neuen Parkplatz
`404 Error: Not Found`: bei Eingabe einer nicht Existenten ID

Im Ordner `models` gibt es Klassen für `ApiAccessLog`, `ParkingLot`, `ParkingLotResponse`, `ParkingLotSummary`  diese werden dann auf die entsprechenden Datenbanken gemappt.

## 3 Logging/Messaging
Zum Logging wird `Apache Kafka` verwendet. Da ich mit diesem Framework selbst ein Neuling bin, habe ich eine Kafka-UI mitlaufen: http://localhost:8081/

Unter `KafkaLoggingAttribute.cs` gibt es die omnipräsente Methode die alles mitprotokolliert. 

Wenn ein Parkplatz weniger als 10% Kapazität hat wird eine Message an Kafka gesendet. Das passiert in der `ParkingController`.
Es gibt jetzt in Kafka unter `Consumers` die `alert-service-group` die benachrichtigt werden, wenn zu wenige Parkplätze frei sind.


## Aufgabe 5:

Erstellen Sie eine Service-Klasse, welche per Dependency Injection aus Ihren Controller-Methoden oder Service-Klassen aufgerufen wird.

### Lösung:

DI entspricht hier der aufgerufenen Datenbankverbindung, die in der "Progrmm.cs" Datei an die Service Klasse weitergegeben wird.
Dies erleichtert die Überarbeitung des Codes bei Änderung der Datenbank --> Anstatt in jeder Service-Klasse den Code zu ändern, muss er nur einmal in der "Programm.cs" Datei geändert werden.

### Umsetzung:

Unter Services wird eine neue Datei "IParkingLotService.cs" erstellt. Diese Datei dient als Interface eines Parkplatz und wird in der "Program.cs" Datei eingebunden. Folgende Tasks sind hier deklariert:

* "GetByIdAsync" = Abrufen von Informationen eines Parkplatz",
* "CreateAsync" = Erstellen eines neuen Parkplatz in der Datenbank,
* "RemoveAsync" = Löschen eines Parkplatz,
* "UpdateParkingLotAsync" = Update der Informationen eines Parkplatz,
* "UpdateAddress" = Zuweisung einer neuen Adresse

In der Datei "Program.cs" wird "IParkingLotService.cs" zum Aufruf des passenden Interfaces eingebunden und zur Erstellung einer passenden Instanz über "ParkingLotService.cs" aufgerufen (Interface wird mit Logik Klasse verknüpft):

"builder.Services.AddScoped<IParkingLotService, ParkingLotService>();"


Außerdem wird hier eine Verbindung zur Datenbank angemeldet, die dann auch an "ParkingLotService.cs" weitergegeben wird:

"var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");

builder.Services.AddDbContext<ParkingLotDB>(options =>
options.UseNpgsql(connectionString));"


"ParkingLotService.cs": Nimmt die Datenbankverbindung und Interface aus "Programm.cs" entgegen und erstellt eine angefragte Instanz.

Hier wird dann die Datenbankverbindung gespeichert:
"private readonly ParkingLotDB \_context;"


## Aufgabe 9:

Verwenden Sie Einträge aus der „appsettings.json“ für einen beliebigen Zweck

### Lösung:

Durch Erstellung eines neuen Parkplatz wird eine Information darüber im Terminal ausgegeben, z.B.: "ParkingLot 'TEST9' wurde erstellt (ID=17)"

### Umsetzung:

"appsettings.json": Hier wurde eine Funktion für das Logging der Service Klasse erstellt, die ausgibt, ob ein Parkplatz erstellt wurde. Der Parameter kann für Ausgabe auf True gestellt werden und wenn keine Ausgabe des Loggings gemacht werden soll: False.

"ParkingSettings": {
"EnableCreateLogging": true


"ParkingLotService.cs": Hier wird in der json Datei nachgeschaut ob True oder False gesetzt ist. Wenn der Wert auf True gesetzt ist, wird folgender Code ausgeführt (Information das ein Parkplatz erstellt wurde):

if (\_enableCreateLogging)
{
Console.WriteLine(
$"ParkingLot '{parkingLot.Name}' wurde erstellt (ID={parkingLot.Id})");
}


## 6 Client
Den gibt es hier: https://github.com/delorenzo222/ParkClientApp

# 11. Funktionierende Gesamtlösung
:) :) :)

# Swagger

Access to Swagger: http://localhost:5251/swagger/index.html

# Console

The console is provided here: https://github.com/delorenzo222/ParkClient/tree/main

# Kafka

As I am myself very new to Kafka, I rely and a UI. After running the docker compose commands you find the Kafka UI here: http://localhost:8081

## Settings for Kafka

I work with what I know from Python as "Decorators", in order to modify the Class to modify Kafka Logging find it here:

`Filters/KafkaLoggingAttribute.cs`
