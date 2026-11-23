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
  tipoNotificacion?: string;
  tipo?: string;
  leida?: boolean;
  fechaActualizacion?: string;
  intentos: number;
  detalleRespuesta?: string;
}

/** Valores usados en los selectores de filtros (evita texto libre). */
export const ESTADOS_NOTIFICACION: string[] = ['Pendiente', 'Enviado', 'Fallido'];
export const CANALES_NOTIFICACION: string[] = ['WhatsApp', 'SMS', 'Email'];
export const TIPOS_NOTIFICACION: string[] = [
  'Recordatorio',
  'Confirmacion',
  'Reprogramacion',
  'Resultado',
  'Cancelacion'
];

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
