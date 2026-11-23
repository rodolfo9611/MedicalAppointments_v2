using System.ComponentModel.DataAnnotations;

namespace MicroservicioPacientes.Application.Validation;

/// <summary>
/// Valida que una fecha no sea posterior al día actual.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotFutureDateAttribute : ValidationAttribute
{
    public NotFutureDateAttribute()
        : base("La fecha no puede ser futura.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is DateTime date)
            return date.Date <= DateTime.Today;

        return false;
    }
}
