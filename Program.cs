using Microsoft.EntityFrameworkCore;
using ParkingProject;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ParkingLotDB>(opt => opt.UseInMemoryDatabase("ParkingLot"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Registriert die Controller
builder.Services.AddControllers();

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



app.Run();

