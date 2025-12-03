using Microsoft.EntityFrameworkCore;
using ParkingProject;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ParkingLotDB>(opt => opt.UseInMemoryDatabase("ParkingLot"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ?? 
                       "Host=db;Database=parking_db;Username=user;Password=password"; 

builder.Services.AddDbContext<ParkingLotDB>(options =>
    options.UseNpgsql(connectionString));

// Registriert die Controller
builder.Services.AddControllers();
// Registiert Mock-Up
builder.Services.AddSingleton<MockSpotGenerator>();

// Fügt nur die NSwag-Services für OpenAPI hinzu
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ParkingSpotAPI";
    config.Title = "ParkingSpotAPI v1";
    config.Version = "v1";
});


var app = builder.Build();

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


// Stellt sicher, dass das Routing und die Controller-Endpunkte erkannt werden
app.MapControllers();

// Datenbankerstellung und Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ParkingLotDB>();
    
    // Stellt sicher, dass die Datenbank existiert und wendet Migrationen an.
    // Dies ist idempotempotent (passiert nur einmal).
    context.Database.Migrate(); 
    
    // Führe das Seeding aus (muss eine eigene Methode in Data/Seeder.cs sein)
    // Seeder.SeedInitialData(context); // Beispiel-Aufruf
}

app.Run();

