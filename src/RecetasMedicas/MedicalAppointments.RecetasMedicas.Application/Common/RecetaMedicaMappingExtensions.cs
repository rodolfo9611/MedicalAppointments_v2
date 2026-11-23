using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Entities;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.RecetasMedicas.Application.Common;

public static class RecetaMedicaMappingExtensions
{
    public static RecetaMedicaDto ToDto(this RecetaMedica receta) => new()
    {
        RecetaMedicaID = receta.RecetaMedicaID,
        CitaID = receta.CitaID,
        PacienteID = receta.PacienteID,
        NombrePaciente = receta.NombrePaciente,
        FechaConsulta = receta.FechaConsulta,
        MedicoID = receta.MedicoID,
        NombreMedico = receta.NombreMedico,
        Especialidad = receta.Especialidad,
        Diagnostico = receta.Diagnostico,
        Indicaciones = receta.Indicaciones,
        Tratamiento = receta.Tratamiento,
        Observaciones = receta.Observaciones,
        FechaEmision = receta.FechaEmision,
        EsAnulada = receta.EsAnulada,
        FechaAnulacion = receta.FechaAnulacion,
        MotivoAnulacion = receta.MotivoAnulacion,
        RecetaAnteriorID = receta.RecetaAnteriorID
    };

    public static PagedResult<RecetaMedicaDto> ToPagedDto(this PagedResult<RecetaMedica> paged) => new()
    {
        Data = paged.Data.Select(ToDto).ToList(),
        TotalRecords = paged.TotalRecords,
        PageSize = paged.PageSize,
        CurrentPage = paged.CurrentPage
    };
}
