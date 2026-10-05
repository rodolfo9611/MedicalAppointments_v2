using Newtonsoft.Json;

namespace MedicalAppointments.Core.Citas.DTOs;

/// <summary>
/// Contrato de los datos que devuelve el microservicio de Pacientes.
/// Usa [JsonProperty] de Newtonsoft porque es el serializador que usa la librería HTTP.
/// </summary>
public class GetPacienteExternoDto
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonProperty("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [JsonProperty("documentoIdentidad")]
    public string DocumentoIdentidad { get; set; } = string.Empty;

    [JsonProperty("fechaNacimiento")]
    public DateTime FechaNacimiento { get; set; }
}
