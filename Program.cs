using Microsoft.EntityFrameworkCore;
using ParkingProject;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ParkingLotDB>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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


app.Run();