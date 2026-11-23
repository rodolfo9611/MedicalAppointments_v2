/** Evento del historial clínico de un paciente. */
export interface HistorialEvento {
  historialID?: number;
  eventoID?: number;
  id?: number;
  pacienteID?: number;
  tipoEvento?: string;
  tipo?: string;
  descripcion?: string;
  observaciones?: string;
  fechaEvento?: string;
  fecha?: string;
  citaID?: number;
  [key: string]: unknown;
}

export interface Paciente {
  /** El backend expone Id; pacienteID es un alias cómodo para el cliente. */
  id?: number;
  pacienteID?: number;
  codigoPaciente?: string;
  nombreCompleto?: string;
  /** Campos del contrato de escritura (Create/UpdatePacienteDto). */
  nombre?: string;
  apellido?: string;
  documento?: string;
  documentoIdentidad?: string;
  correoElectronico?: string;
  telefono?: string;
  fechaNacimiento?: string;
  historiales?: HistorialEvento[];
  [key: string]: unknown;
}
