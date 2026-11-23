import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, map, of, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResult } from '../models/cita.model';
import {
  CrearRecetaDto,
  RecetaCreadaResponse,
  RecetaMedica
} from '../models/receta.model';

@Injectable({
  providedIn: 'root'
})
export class RecetasService {
  private apiUrl = environment.api.recetas;

  constructor(private http: HttpClient) {}

  /** POST anidado: la receta se emite desde una cita Concluida. */
  emitirParaCita(citaId: number, receta: CrearRecetaDto): Observable<RecetaCreadaResponse> {
    return this.http.post<RecetaCreadaResponse>(`${this.apiUrl}/Citas/${citaId}/RecetasMedicas`, receta);
  }

  /** GET /api/RecetasMedicas/cita/{citaId}: receta activa o 404. */
  getActivaPorCita(citaId: number): Observable<RecetaMedica> {
    return this.http.get<RecetaMedica>(`${this.apiUrl}/RecetasMedicas/cita/${citaId}`);
  }

  /** Variante para el flujo de Citas: 404 se traduce a null (no hay receta activa). */
  getActivaPorCitaSeguro(citaId: number): Observable<RecetaMedica | null> {
    return this.getActivaPorCita(citaId).pipe(
      catchError((err: { status?: number }) => (err?.status === 404 ? of(null) : throwError(() => err)))
    );
  }

  /** GET /api/RecetasMedicas devuelve una página; se expone como arreglo para la UI. */
  getAll(): Observable<RecetaMedica[]> {
    return this.http
      .get<PagedResult<RecetaMedica>>(`${this.apiUrl}/RecetasMedicas`)
      .pipe(map((result) => result?.data ?? []));
  }

  getById(id: number): Observable<RecetaMedica> {
    return this.http.get<RecetaMedica>(`${this.apiUrl}/RecetasMedicas/${id}`);
  }

  getHistorialPaciente(pacienteId: number): Observable<RecetaMedica[]> {
    return this.http.get<RecetaMedica[]>(`${this.apiUrl}/RecetasMedicas/historial/paciente/${pacienteId}`);
  }

  getAnuladas(): Observable<RecetaMedica[]> {
    return this.http.get<RecetaMedica[]>(`${this.apiUrl}/RecetasMedicas/anuladas`);
  }

  getPaged(
    pageNumber: number = 1,
    pageSize: number = 10,
    pacienteId?: number,
    diagnostico?: string,
    fechaDesde?: string,
    fechaHasta?: string,
    incluirAnuladas: boolean = false
  ): Observable<PagedResult<RecetaMedica>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('incluirAnuladas', incluirAnuladas.toString());

    if (pacienteId && pacienteId > 0) params = params.set('pacienteId', pacienteId.toString());
    if (diagnostico?.trim()) params = params.set('diagnostico', diagnostico.trim());
    if (fechaDesde?.trim()) params = params.set('fechaDesde', fechaDesde.trim());
    if (fechaHasta?.trim()) params = params.set('fechaHasta', fechaHasta.trim());

    return this.http.get<PagedResult<RecetaMedica>>(`${this.apiUrl}/RecetasMedicas/paged`, { params });
  }

  getUrlImprimible(id: number): string {
    return `${this.apiUrl}/RecetasMedicas/${id}/imprimible`;
  }

  anular(id: number, motivo: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/RecetasMedicas/${id}`, {
      params: new HttpParams().set('motivo', motivo)
    });
  }
}
