/** Estados oficiales del proceso clínico (coinciden con el enum del backend). */
export type EstadoCitaEnum = 'Agendada' | 'Citada' | 'PendienteResultados' | 'Concluida' | 'Cancelada';

/** Catálogo de estados usado por los selectores (nunca se escribe texto libre). */
export interface EstadoCita {
  estadoCitaID: number;
  codigo: EstadoCitaEnum;
  nombre: string;
  descripcion?: string;
}

export const ESTADOS_CITA: EstadoCita[] = [
  {
    estadoCitaID: 1,
    codigo: 'Agendada',
    nombre: 'Agendada',
    descripcion: 'Cita agendada, pendiente de la primera consulta.'
  },
  {
    estadoCitaID: 2,
    codigo: 'Citada',
    nombre: 'Citada',
    descripcion: 'Post-consulta: requiere intervención quirúrgica.'
  },
  {
    estadoCitaID: 3,
    codigo: 'PendienteResultados',
    nombre: 'Pendiente de resultados',
    descripcion: 'Paciente evaluado; misma cita con fecha reprogramable.'
  },
  {
    estadoCitaID: 4,
    codigo: 'Concluida',
    nombre: 'Concluida',
    descripcion: 'Alta clínica; habilita la emisión explícita de receta.'
  },
  {
    estadoCitaID: 5,
    codigo: 'Cancelada',
    nombre: 'Cancelada',
    descripcion: 'Cita cancelada.'
  }
];

/** Convierte el identificador canonical del catálogo a su valor de enum. */
export function estadoCitaFromId(id: number | string | null | undefined): EstadoCitaEnum | undefined {
  const numericId = typeof id === 'string' ? Number(id) : id;
  return ESTADOS_CITA.find((e) => e.estadoCitaID === numericId)?.codigo;
}

export interface Cita {
  citaID?: number;
  pacienteID: number;
  medicoID: number;
  especialidadID?: number;
  consultorioID?: number;
  /** Identificador del catálogo (compatibilidad con el modelo anterior). */
  estadoCitaID: number;
  /** Estado devuelto por GetCitaDto como string. */
  estado?: EstadoCitaEnum;
  estadoNombre?: string;
  nombreMedico?: string;
  especialidad?: string;
  fecha?: string;
  hora?: string;
  fechaHoraInicio: string;
  fechaHoraFin?: string;
  motivo?: string;
  observaciones?: string;
  usuarioCreacionID?: number;
  fechaCreacion?: string;
}

export interface RegistroAtencionDto {
  secuencia: number;
  fechaAtencion?: string;
  observaciones?: string;
}

export interface ReprogramacionDto {
  nuevaFechaHoraInicio: string;
  nuevaFechaHoraFin?: string;
  motivo: string;
}

/** Cuerpo aceptado por PUT /api/Citas/{id}. */
export interface UpdateCitaDto {
  estado: EstadoCitaEnum;
  fechaHoraInicio?: string;
  fechaHoraFin?: string;
  motivo?: string;
  observaciones?: string;
  registroAtencion?: RegistroAtencionDto;
  reprogramacion?: ReprogramacionDto;
}

export interface PagedResult<T> {
  data: T[];
  totalRecords: number;
  pageSize: number;
  currentPage: number;
  totalPage: number;
}
