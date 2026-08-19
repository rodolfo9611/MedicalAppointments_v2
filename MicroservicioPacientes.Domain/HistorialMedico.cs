namespace MicroservicioPacientes.Domain;

public class HistorialMedico
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
}