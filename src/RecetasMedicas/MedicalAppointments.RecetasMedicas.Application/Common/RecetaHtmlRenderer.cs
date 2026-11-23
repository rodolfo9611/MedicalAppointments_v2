using System.Net;
using System.Text;
using MedicalAppointments.RecetasMedicas.Application.DTOs;

namespace MedicalAppointments.RecetasMedicas.Application.Common;

/// <summary>Genera el documento HTML imprimible de una receta médica.</summary>
public static class RecetaHtmlRenderer
{
    public static string Render(RecetaMedicaDto receta)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"es\"><head><meta charset=\"utf-8\" />");
        sb.AppendLine("<title>Receta médica</title>");
        sb.AppendLine("<style>");
        sb.AppendLine("@page { size: A4; margin: 18mm; }");
        sb.AppendLine("body { font-family: Arial, sans-serif; color: #1f2937; font-size: 12pt; }");
        sb.AppendLine("header { border-bottom: 2px solid #0f766e; padding-bottom: 10px; margin-bottom: 18px; }");
        sb.AppendLine("h1 { margin: 0; font-size: 20pt; color: #0f766e; }");
        sb.AppendLine("h2 { font-size: 13pt; margin-top: 22px; border-bottom: 1px solid #d1d5db; padding-bottom: 4px; }");
        sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-bottom: 12px; }");
        sb.AppendLine("td { padding: 6px 8px; border: 1px solid #d1d5db; vertical-align: top; }");
        sb.AppendLine("td.etiqueta { width: 22%; font-weight: bold; background: #f3f4f6; }");
        sb.AppendLine(".parrafo { white-space: pre-wrap; min-height: 42px; }");
        sb.AppendLine(".anulada { color: #b91c1c; font-weight: bold; }");
        sb.AppendLine("footer { margin-top: 36px; border-top: 1px solid #d1d5db; padding-top: 8px; font-size: 9pt; color: #6b7280; }");
        sb.AppendLine("@media print {");
        sb.AppendLine("  body { background: #fff; }");
        sb.AppendLine("  .no-print { display: none !important; }");
        sb.AppendLine("  h2 { page-break-after: avoid; }");
        sb.AppendLine("  table, .parrafo { page-break-inside: avoid; }");
        sb.AppendLine("}");
        sb.AppendLine("</style></head><body>");
        sb.AppendLine("<header><h1>Receta médica</h1>");
        sb.AppendLine($"<div>Emisión: {Encode(receta.FechaEmision.ToString("dd/MM/yyyy HH:mm"))} &nbsp;|&nbsp; N°. {Encode(receta.RecetaMedicaID.ToString())}</div>");
        if (receta.EsAnulada)
        {
            sb.AppendLine($"<div class=\"anulada\">ANULADA el {Encode(receta.FechaAnulacion?.ToString("dd/MM/yyyy HH:mm"))} — {Encode(receta.MotivoAnulacion)}</div>");
        }
        sb.AppendLine("</header>");

        sb.AppendLine("<h2>Datos del paciente</h2><table>");
        Fila(sb, "Paciente", receta.NombrePaciente);
        Fila(sb, "Identificador", receta.PacienteID.ToString());
        Fila(sb, "Cita", receta.CitaID.ToString());
        Fila(sb, "Fecha de consulta", receta.FechaConsulta.ToString("dd/MM/yyyy HH:mm"));
        sb.AppendLine("</table>");

        sb.AppendLine("<h2>Profesional</h2><table>");
        Fila(sb, "Médico", receta.NombreMedico ?? receta.MedicoID?.ToString() ?? "No registrado");
        Fila(sb, "Especialidad", receta.Especialidad);
        sb.AppendLine("</table>");

        sb.AppendLine("<h2>Diagnóstico e indicaciones</h2><table>");
        Fila(sb, "Diagnóstico", receta.Diagnostico, true);
        Fila(sb, "Indicaciones", receta.Indicaciones, true);
        Fila(sb, "Tratamiento", receta.Tratamiento, true);
        Fila(sb, "Observaciones", receta.Observaciones, true);
        sb.AppendLine("</table>");
        sb.AppendLine("<footer>Documento generado por el microservicio de Recetas Médicas. Consulta fecha: " +
                      Encode(receta.FechaConsulta.ToString("dd/MM/yyyy HH:mm")) + ".</footer>");
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    private static void Fila(StringBuilder sb, string etiqueta, string? valor, bool parrafo = false)
    {
        sb.AppendLine($"<tr><td class=\"etiqueta\">{Encode(etiqueta)}</td>" +
                      $"<td{(parrafo ? " class=\"parrafo\"" : string.Empty)}>{Encode(valor)}</td></tr>");
    }

    private static string Encode(string? valor) => WebUtility.HtmlEncode(valor ?? string.Empty);
}
