export interface Notificacion {
  notificacionID: number;
  citaID: number;
  pacienteID: number;
  pacienteNombre: string;
  destinatario: string;
  canal: string;
  mensaje: string;
  fechaProgramadaCita: string;
  fechaRegistro: string;
  fechaEnvio?: string;
  estado: string;
  intentos: number;
  detalleRespuesta?: string;
}

export interface StatsResponse {
  totalNotificaciones: number;
  enviadas: number;
  pendientes: number;
  fallidas: number;
  tasaEfectividadPorcentaje: number;
  desgloseCanales: { canal: string; cantidad: number }[];
  metricasBusinessCase: {
    citasRecuperadasEstimadas: number;
    ahorroEstimadoUSD: number;
    tasaAusentismoProyectada: string;
    tasaAusentismoInicial: string;
    reduccionRelativa: string;
  };
}
