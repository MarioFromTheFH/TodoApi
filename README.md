# Aufgabe 5:

Erstellen Sie eine Service-Klasse, welche per Dependency Injection aus Ihren Controller-Methoden oder Service-Klassen aufgerufen wird.

## Lösung:

DI entspricht hier der aufgerufenen Datenbankverbindung, die in der "Progrmm.cs" Datei an die Service Klasse weitergegeben wird.
Dies erleichtert die Überarbeitung des Codes bei Änderung der Datenbank --> Anstatt in jeder Service-Klasse den Code zu ändern, muss er nur einmal in der "Programm.cs" Datei geändert werden.

## Umsetzung:

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



---



# Aufgabe 9:

Verwenden Sie Einträge aus der „appsettings.json“ für einen beliebigen Zweck

## Lösung:

Durch Erstellung eines neuen Parkplatz wird eine Information darüber im Terminal ausgegeben, z.B.: "ParkingLot 'TEST9' wurde erstellt (ID=17)"

## Umsetzung:

"appsettings.json": Hier wurde eine Funktion für das Logging der Service Klasse erstellt, die ausgibt, ob ein Parkplatz erstellt wurde. Der Parameter kann für Ausgabe auf True gestellt werden und wenn keine Ausgabe des Loggings gemacht werden soll: False.

"ParkingSettings": {
"EnableCreateLogging": true



"ParkingLotService.cs": Hier wird in der json Datei nachgeschaut ob True oder False gesetzt ist. Wenn der Wert auf True gesetzt ist, wird folgender Code ausgeführt (Information das ein Parkplatz erstellt wurde):

if (\_enableCreateLogging)
{
Console.WriteLine(
$"ParkingLot '{parkingLot.Name}' wurde erstellt (ID={parkingLot.Id})");
}

