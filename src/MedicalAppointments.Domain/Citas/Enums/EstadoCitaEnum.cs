using System.ComponentModel;

namespace MedicalAppointments.Domain.Citas.Enums;

/// <summary>
/// Estados oficiales del proceso clínico de una cita. El valor numérico coincide
/// con el identificador canónico del catálogo Estados_Cita.
/// </summary>
public enum EstadoCitaEnum
{
    [Description("Cita agendada, pendiente de la primera consulta.")]
    Agendada = 1,

    [Description("Post-consulta: requiere intervención quirúrgica.")]
    Citada = 2,

    [Description("Paciente ya evaluado; misma cita con fecha reprogramable.")]
    PendienteResultados = 3,

    [Description("Alta clínica; habilita la emisión explícita de receta.")]
    Concluida = 4,

    [Description("Cita cancelada.")]
    Cancelada = 5
}
