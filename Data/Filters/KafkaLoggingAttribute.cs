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
            catch (Exception)
            {
                // Logging soll niemals den Request crashen lassen
            }
        }
    }
}