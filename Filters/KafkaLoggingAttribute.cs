using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ParkingProject.Services;
using ParkingProject.Models;

namespace ParkingProject.Filters
{
    public class LogToKafkaAttribute : ActionFilterAttribute
    {
        // Wir brauchen Zugriff auf den Service, aber Attribute können keine Constructor Injection.
        // Deshalb holen wir den Service im Kontext.
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Führe die eigentliche Controller-Methode aus
            var executedContext = await next();

            // 2. Danach: Loggen
            try
            {
                var kafkaService = context.HttpContext.RequestServices.GetService<KafkaProducerService>();
                
                if (kafkaService != null)
                {
                    var actionName = context.ActionDescriptor.DisplayName ?? "Unbekannte Aktion";
                    // Verwende ?. um sicher auf den Header zuzugreifen und den Null-Forgiving Operator (!)
                    // weil wir wissen, dass die Log-Struktur Strings akzeptiert.
                    var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString() ?? "N/A";
                    
                    // Ergebnis auslesen
                    string status = "Unknown";
                    string? resultValue = null;

                    if (executedContext.Result is ObjectResult objResult)
                    {
                        status = objResult.StatusCode.ToString() ?? "200";
                        resultValue = objResult.Value?.ToString();
                    }
                    else if (executedContext.Result is StatusCodeResult statusResult)
                    {
                        status = statusResult.StatusCode.ToString() ?? "N/A";
                    }

                    // Parkplatz ID aus der Route holen (wenn vorhanden)
                    int? parkingId = null;
                    if (context.RouteData.Values.ContainsKey("id"))
                    {
                        int.TryParse(context.RouteData.Values["id"]?.ToString(), out int pId);
                        parkingId = pId;
                    }

                    var log = new ApiAccessLog
                    {
                        Action = actionName,
                        ParkingLotId = parkingId,
                        UserAgent = userAgent,
                        ResultStatus = status,
                        ResultValue = resultValue // Das Feld, das wir in 2/6 hinzugefügt haben
                    };

                    await kafkaService.LogAccessAsync(log);
                }
            }
            catch (Exception ex)
            {
                // 1. Konsolen-Ausgabe (Damit Exceptions nicht mehr "geschluckt" werden)
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[KAFKA ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Aktion: {context.ActionDescriptor.DisplayName}");
                Console.WriteLine($"Fehler: {ex.Message}");
                Console.ResetColor();

                // 2. In Datei schreiben
                try 
                {
                    string logPath = Path.Combine(AppContext.BaseDirectory, "logs");
                    string filePath = Path.Combine(logPath, "kafka_fallback_errors.log");

                    // Sicherstellen, dass der Ordner existiert
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }

                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR in {context.ActionDescriptor.DisplayName}{Environment.NewLine}" +
                                    $"Message: {ex.Message}{Environment.NewLine}" +
                                    $"Stacktrace: {ex.StackTrace}{Environment.NewLine}" +
                                    $"{new string('-', 50)}{Environment.NewLine}";

                    // Anhängen an die Datei (erstellt die Datei, falls sie nicht existiert)
                    File.AppendAllText(filePath, logEntry);
                }
                catch (Exception fileEx)
                {
                    // Falls sogar das Schreiben in die Datei fehlschlägt (z.B. keine Rechte)
                    Console.WriteLine($"[CRITICAL] Could not write to log file: {fileEx.Message}");
                }
            }
        }
    }
}