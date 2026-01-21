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

### Status-Codes
`200`: Kommt mit den meisten GET-Requests mit, die nur zur Abfrage dienen
`201`: Erfolgreiches Erstellen eines Parkplatzes/Adresse 
`400 Error: Bad Request`: bei fehlerhaftem Input für neuen Parkplatz
`404 Error: Not Found`: bei Eingabe einer nicht Existenten ID

Im Ordner `models` gibt es Klassen für `ApiAccessLog`, `ParkingLot`, `ParkingLotResponse`, `ParkingLotSummary`  diese werden dann auf die entsprechenden Datenbanken gemappt.

## 3 Logging/Messaging
Zum Logging wird `Apache Kafka` verwendet. Da ich mit diesem Framework selbst ein Neuling bin, habe ich eine Kafka-UI mitlaufen: http://localhost:8081/

Unter `KafkaLoggingAttribute.cs` gibt es die omnipräsente Methode die alles mitprotokolliert. 

Wenn ein Parkplatz weniger als 10% Kapazität hat wird eine Message an Kafka gesendet. Das passiert in der `ParkingController`

# 11. Funktionierende Gesamtlösung
:) :) :)
