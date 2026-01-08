using Microsoft.EntityFrameworkCore;
using ParkingProject;
using ParkingProject.Data;
using ParkingProject.Services;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<ParkingLotDB>(opt => opt.UseInMemoryDatabase("ParkingLot"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Wirft eine Exception, wenn "PostgresConnection" fehlt -> Das ist gut so!
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") 
    ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");

builder.Services.AddDbContext<ParkingLotDB>(options =>
    options.UseNpgsql(connectionString));

// Registriert die Controller
builder.Services.AddControllers();

// Registiert Mock-Up und den Kafka-Service
builder.Services.AddSingleton<MockSpotGenerator>();
builder.Services.AddSingleton<KafkaProducerService>();

//Serviceergänzung für DI
builder.Services.AddScoped<IParkingLotService, ParkingLotService>();

// Fügt nur die NSwag-Services für OpenAPI hinzu
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ParkingSpotAPI";
    config.Title = "ParkingSpotAPI v1";
    config.Version = "v1";
});


var app = builder.Build();

// Swagger Konfiguration
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "ParkingSpotAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

// Datenbank Initialisierung & Seeding (Alles in einem Block!)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ParkingLotDB>();
        
        // Wendet Migrationen an ODER erstellt die DB, falls sie fehlt
        // Wenn du noch keine Migrationen hast, nutze: context.Database.EnsureCreated();
        context.Database.Migrate(); 
        
        // Füllt die Testdaten ein
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ein Fehler ist beim Initialisieren der Datenbank aufgetreten.");
    }
}

app.MapControllers();
app.Run();