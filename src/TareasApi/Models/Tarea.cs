namespace TareasApi.Models;

public class Tarea
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Titulo { get; set; } = "";
    public bool Hecha { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}

public record TareaIn(string Titulo, bool Hecha = false);
