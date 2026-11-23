import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Cita,
  EstadoCita,
  PagedResult,
  UpdateCitaDto,
  estadoCitaFromId
} from '../models/cita.model';

@Injectable({
  providedIn: 'root'
})
export class CitasService {
  private apiUrl = environment.api.citas;

  constructor(private http: HttpClient) {}

  // 1. GET /api/Citas
  getAll(): Observable<Cita[]> {
    return this.http.get<Cita[]>(`${this.apiUrl}/Citas`);
  }

  // 2. GET /api/Citas/paged
  getPaged(pageNumber: number = 1, pageSize: number = 10, filter?: string, orderBy?: string): Observable<PagedResult<Cita>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (filter && filter.trim()) {
      params = params.set('filter', filter.trim());
    }
    if (orderBy && orderBy.trim()) {
      params = params.set('orderBy', orderBy.trim());
    }

    return this.http.get<PagedResult<Cita>>(`${this.apiUrl}/Citas/paged`, { params });
  }

  // 3. GET /api/Citas/{id}
  getById(id: number): Observable<Cita> {
    return this.http.get<Cita>(`${this.apiUrl}/Citas/${id}`);
  }

  // 4. GET /api/Citas/one
  getOneBy(filter: string): Observable<Cita> {
    return this.http.get<Cita>(`${this.apiUrl}/Citas/one`, { params: new HttpParams().set('filter', filter) });
  }

  // 5. POST /api/Citas
  create(cita: Partial<Cita>): Observable<{ citaID: number }> {
    return this.http.post<{ citaID: number }>(`${this.apiUrl}/Citas`, cita);
  }

  /**
   * PUT /api/Citas/{id}.
   * El backend espera UpdateCitaDto. Se acepta también la entidad Cita antigua
   * para no romper llamadas existentes: en ese caso se convierte al DTO.
   */
  update(id: number, datos: Cita | UpdateCitaDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Citas/${id}`, this.toUpdateDto(datos));
  }

  /** Alias explícitamente tipado para el nuevo contrato del proceso clínico. */
  updateProceso(id: number, datos: UpdateCitaDto): Observable<void> {
    return this.update(id, datos);
  }

  // DELETE /api/Citas/{id}
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Citas/${id}`);
  }

  // GET /api/EstadosCita
  getEstados(): Observable<EstadoCita[]> {
    return this.http.get<EstadoCita[]>(`${this.apiUrl}/EstadosCita`);
  }

  // GET /api/EstadosCita/{id}
  getEstadoById(id: number): Observable<EstadoCita> {
    return this.http.get<EstadoCita>(`${this.apiUrl}/EstadosCita/${id}`);
  }

  private toUpdateDto(datos: Cita | UpdateCitaDto): UpdateCitaDto {
    if ('estado' in datos && datos.estado) {
      return datos as UpdateCitaDto;
    }

    const cita = datos as Cita;
    const estado = cita.estado ?? estadoCitaFromId(cita.estadoCitaID) ?? 'Agendada';
    const dto: UpdateCitaDto = {
      estado,
      fechaHoraInicio: cita.fechaHoraInicio,
      fechaHoraFin: cita.fechaHoraFin,
      motivo: cita.motivo,
      observaciones: cita.observaciones
    };
    return dto;
  }
}
