using Microsoft.EntityFrameworkCore;
using ParkingProject;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ParkingLotDB>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
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

app.MapGet("/parkingSpots", async (ParkingLotDB db) =>
    await db.ParkingLots.ToListAsync());

app.MapGet("/parkingSpot/Totalspots", async (ParkingLotDB db) =>
    await db.ParkingLots.Where(t => t.getSpots).ToListAsync());

app.MapGet("/parkingSpot/FreeSpots", async (ParkingLotDB db) =>
    await db.ParkingLots.Where(t => t.getFreeSpots).ToListAsync());    

app.MapGet("/parkingSpots/{id}", async (int id, ParkingLotDB db) =>
    await db.ParkingLots.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, ParkingLotDB db) =>
{
    db.ParkingLots.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, ParkingLotDB db) =>
{
    var todo = await db.ParkingLots.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, ParkingLotDB db) =>
{
    if (await db.ParkingLots.FindAsync(id) is Todo todo)
    {
        db.ParkingLots.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();