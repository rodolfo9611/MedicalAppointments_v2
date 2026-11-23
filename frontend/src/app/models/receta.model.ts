/** Respuesta de consulta de una receta médica. */
export interface RecetaMedica {
  recetaMedicaID?: number;
  recetaID?: number;
  id?: number;
  citaID?: number;
  citaId?: number;
  pacienteID?: number;
  pacienteId?: number;
  diagnostico?: string;
  indicaciones?: string;
  tratamiento?: string;
  observaciones?: string;
  fechaEmision?: string;
  fechaCreacion?: string;
  esAnulada?: boolean;
  anulada?: boolean;
  activa?: boolean;
  estado?: string;
  [key: string]: unknown;
}

/** Cuerpo de POST /api/Citas/{citaId}/RecetasMedicas. */
export interface CrearRecetaDto {
  diagnostico: string;
  indicaciones: string;
  tratamiento: string;
  observaciones?: string;
}

export interface RecetaCreadaResponse {
  recetaMedicaID?: number;
  recetaID?: number;
  id?: number;
  mensaje?: string;
  [key: string]: unknown;
}

export function idReceta(receta: RecetaMedica | RecetaCreadaResponse | null | undefined): number | undefined {
  return receta?.recetaMedicaID ?? receta?.recetaID ?? receta?.id;
}

export function citaIdReceta(receta: RecetaMedica | null | undefined): number | undefined {
  return receta?.citaID ?? receta?.citaId;
}

export function recetaAnulada(receta: RecetaMedica): boolean {
  return receta.esAnulada === true || receta.anulada === true || receta.activa === false || (receta.estado ?? '').toLowerCase().includes('anul');
}
