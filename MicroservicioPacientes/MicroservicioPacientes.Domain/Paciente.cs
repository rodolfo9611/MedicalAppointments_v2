namespace MicroservicioPacientes.Domain;

public class Paciente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }

    // Relación con la segunda tabla
    public List<HistorialMedico> Historiales { get; set; } = new();
}