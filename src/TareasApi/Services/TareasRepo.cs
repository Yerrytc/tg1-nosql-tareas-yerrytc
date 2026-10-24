using System.Net;
using Microsoft.Azure.Cosmos;
using TareasApi.Models;

namespace TareasApi.Services;

public class TareasRepo
{
    private readonly Container c;

    public TareasRepo(Container c)
    {
        this.c = c;
    }

    public async Task<List<Tarea>> Listar()
    {
        var q = c.GetItemQueryIterator<Tarea>("SELECT * FROM c ORDER BY c.fecha DESC");
        var r = new List<Tarea>();
        while (q.HasMoreResults)
            r.AddRange(await q.ReadNextAsync());
        return r;
    }

    public async Task<Tarea?> Buscar(string id)
    {
        try
        {
            return (await c.ReadItemAsync<Tarea>(id, new PartitionKey(id))).Resource;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Tarea> Crear(Tarea t)
    {
        return (await c.CreateItemAsync(t, new PartitionKey(t.Id))).Resource;
    }

    public async Task<Tarea> Actualizar(Tarea t)
    {
        return (await c.ReplaceItemAsync(t, t.Id, new PartitionKey(t.Id))).Resource;
    }

    public async Task Borrar(string id)
    {
        await c.DeleteItemAsync<Tarea>(id, new PartitionKey(id));
    }

    public async Task<int> Contar()
    {
        var q = c.GetItemQueryIterator<int>("SELECT VALUE COUNT(1) FROM c");
        var r = await q.ReadNextAsync();
        return r.First();
    }
}
