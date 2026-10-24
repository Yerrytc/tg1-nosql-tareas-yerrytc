using System.Text.Json;
using Microsoft.Azure.Cosmos;
using TareasApi.Models;
using TareasApi.Services;

var b = WebApplication.CreateBuilder(args);
var cfg = b.Configuration.GetSection("Cosmos");
var opt = new CosmosClientOptions
{
    UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
};
var cli = new CosmosClient(cfg["Endpoint"], cfg["Key"], opt);
var db = (await cli.CreateDatabaseIfNotExistsAsync(cfg["Database"] ?? "tareasdb")).Database;
var con = (await db.CreateContainerIfNotExistsAsync(cfg["Container"] ?? "tareas", "/id")).Container;
b.Services.AddSingleton(new TareasRepo(con));

var app = b.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", async (TareasRepo r) => Results.Ok(new { estado = "ok", base_datos = "Azure Cosmos DB", tareas = await r.Contar() }));

var g = app.MapGroup("/api/tareas");

g.MapGet("/", async (TareasRepo r) => Results.Ok(await r.Listar()));

g.MapGet("/{id}", async (string id, TareasRepo r) =>
    await r.Buscar(id) is Tarea t ? Results.Ok(t) : Results.NotFound());

g.MapPost("/", async (TareaIn x, TareasRepo r) =>
{
    if (string.IsNullOrWhiteSpace(x.Titulo))
        return Results.BadRequest(new { error = "El titulo es obligatorio" });
    var t = await r.Crear(new Tarea { Titulo = x.Titulo.Trim(), Hecha = x.Hecha });
    return Results.Created($"/api/tareas/{t.Id}", t);
});

g.MapPut("/{id}", async (string id, TareaIn x, TareasRepo r) =>
{
    var t = await r.Buscar(id);
    if (t is null)
        return Results.NotFound();
    t.Titulo = x.Titulo.Trim();
    t.Hecha = x.Hecha;
    return Results.Ok(await r.Actualizar(t));
});

g.MapDelete("/{id}", async (string id, TareasRepo r) =>
{
    if (await r.Buscar(id) is null)
        return Results.NotFound();
    await r.Borrar(id);
    return Results.NoContent();
});

app.Run();
