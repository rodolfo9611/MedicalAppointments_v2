namespace MedicalAppointments.RecetasMedicas.Api.Requests;

/// <summary>Cuerpo opcional para la anulación (alternativo al query 'motivo').</summary>
public sealed class AnularRecetaRequest
{
    /// <summary>Motivo por el que se anula la receta.</summary>
    public string? Motivo { get; set; }
}
