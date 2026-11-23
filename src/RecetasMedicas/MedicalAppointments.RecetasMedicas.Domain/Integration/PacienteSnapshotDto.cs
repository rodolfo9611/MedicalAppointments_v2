namespace MedicalAppointments.RecetasMedicas.Domain.Integration;

/// <summary>Snapshot del paciente devuelto por el microservicio de Pacientes.</summary>
public sealed class PacienteSnapshotDto
{
    public long PacienteID { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? Telefono { get; set; }
}
