export interface Cita {
  citaID?: number;
  pacienteID: number;
  medicoID: number;
  especialidadID?: number;
  consultorioID?: number;
  estadoCitaID: number;
  fechaHoraInicio: string;
  fechaHoraFin?: string;
  motivo?: string;
  observaciones?: string;
  usuarioCreacionID?: number;
  fechaCreacion?: string;
}

export interface EstadoCita {
  estadoCitaID: number;
  codigo: string;
  nombre: string;
}

export interface PagedResult<T> {
  data: T[];
  totalRecords: number;
  pageSize: number;
  currentPage: number;
  totalPage: number;
}
