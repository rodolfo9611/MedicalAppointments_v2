import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResult } from '../models/cita.model';
import { Notificacion, StatsResponse } from '../models/notificacion.model';

@Injectable({
  providedIn: 'root'
})
export class NotificacionesService {
  private apiUrl = environment.api.notificaciones;

  constructor(private http: HttpClient) {}

  getAll(estado?: string, canal?: string): Observable<Notificacion[]> {
    let params = new HttpParams();
    if (estado) params = params.set('estado', estado);
    if (canal) params = params.set('canal', canal);

    return this.http.get<Notificacion[]>(`${this.apiUrl}/Notificaciones`, { params });
  }

  getById(id: number): Observable<Notificacion> {
    return this.http.get<Notificacion>(`${this.apiUrl}/Notificaciones/${id}`);
  }

  getPaged(
    pageNumber: number = 1,
    pageSize: number = 10,
    estado?: string,
    canal?: string
  ): Observable<PagedResult<Notificacion>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    if (estado) params = params.set('estado', estado);
    if (canal) params = params.set('canal', canal);

    return this.http.get<PagedResult<Notificacion>>(`${this.apiUrl}/Notificaciones/paged`, { params });
  }

  getStats(): Observable<StatsResponse> {
    return this.http.get<StatsResponse>(`${this.apiUrl}/Notificaciones/stats`);
  }

  create(notificacion: Partial<Notificacion>): Observable<Notificacion> {
    return this.http.post<Notificacion>(`${this.apiUrl}/Notificaciones`, notificacion);
  }

  update(id: number, notificacion: Partial<Notificacion>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Notificaciones/${id}`, notificacion);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Notificaciones/${id}`);
  }

  reintentar(id: number): Observable<Notificacion> {
    return this.http.post<Notificacion>(`${this.apiUrl}/Notificaciones/${id}/reintentar`, {});
  }
}
