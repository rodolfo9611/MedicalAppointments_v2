/** Resumen devuelto por GET /api/dashboard. */
export interface DashboardResponse {
  totalPacientes: number;
  citasAgendadas: number;
  citasCitadas: number;
  citasPendienteResultados: number;
  citasConcluidas: number;
  citasCanceladas: number;
  recetasEmitidas: number;
  notificacionesGeneradas: number;
  citasHoy: number;
  recetasHoy: number;
  notificacionesHoy: number;
  [key: string]: unknown;
}

/** Filtros compartidos por los reportes. */
export interface ReporteFiltros {
  desde?: string;
  hasta?: string;
  /** Nombres reales esperados por el microservicio de Reportes. */
  FechaDesde?: string;
  FechaHasta?: string;
  estado?: string;
  tipo?: string;
  canal?: string;
  pacienteId?: number;
  citaId?: number;
  pageNumber?: number;
  pageSize?: number;
}

/** Fila genérica de un reporte (las columnas dependen del tipo de reporte). */
export interface ReporteRow {
  [key: string]: unknown;
}
