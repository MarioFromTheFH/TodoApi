1\. Erweiterung des Datenmodells (Models/ParkingLot.cs)



Änderung: Das Dokument wurde um die Eigenschaft CurrentFreeSpots erweitert.



Hintergrund: Diese dient als dynamischer Speicherplatz im Objekt. Da die freien Plätze live berechnet werden, fungiert dieses Feld als "Gefäß", um die vom System generierten Werte aufzunehmen, bevor sie an den User gesendet werden.



2\. Implementierung der Service-Klasse (Services/ParkingLotService.cs)



Was: Diese neue Klasse bildet nun das Herzstück der Logik.



Zuständigkeit: Hier werden die Datenbankabfragen, die Simulation freier Plätze und das Logging zentral gebündelt. Anstatt die Logik im Controller zu verteilen, wird sie hier an einem Ort verwaltet.



3\. Anmeldung der Dienste (Program.cs)



Was: In diesem Dokument wurde die Anmeldung via builder.Services.AddScoped<ParkingLotService>(); vorgenommen.



Warum: Hierdurch "registrieren" wir den Service im System. Das Framework weiß nun, wie es den ParkingLotService erstellen und überall dort einfügen muss, wo er im Konstruktor verlangt wird.



4\. Umbau des Controllers (Controllers/ParkingController.cs)



Änderung: Der Controller wurde komplett umgebaut und "schlank" gemacht.



Logik: Er verlangt im Konstruktor nur noch den ParkingLotService. Die eigentliche Arbeit (wie GetDetailsAsync) wird an den Service delegiert. Der Controller kümmert sich somit nur noch um die HTTP-Anfragen (Separation of Concerns).



5\. Zentrale Konfiguration (appsettings.json)



Was: Hier wurde ein neuer Eintrag für ParkingSettings (z. B. ein Schwellenwert für freie Plätze) hinzugefügt.



Vorteil: Anstatt Zahlen fest im Code zu verankern ("Hardcoding"), nutzen wir dieses Dokument, um die App flexibel steuerbar zu machen, ohne den Quellcode ändern zu müssen.



6\. Nutzung der Konfiguration via DI (Services/ParkingLotService.cs)



Technik: Der ParkingLotService nutzt nun das Interface IConfiguration via Dependency Injection.



Anwendung: Der Service liest die Werte direkt aus der appsettings.json aus, um beispielsweise bei Erreichen eines Schwellenwerts automatisch Warnmeldungen auf der Konsole auszugeben.





* Entkopplung: Controller und Logik sind nun sauber getrennt.
* Wartbarkeit: Änderungen an der Berechnung müssen nur noch in ParkingLotService.cs vorgenommen werden.
* Flexibilität: Schwellenwerte lassen sich einfach über die appsettings.json anpassen.
