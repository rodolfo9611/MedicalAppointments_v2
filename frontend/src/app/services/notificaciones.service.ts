import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Notificacion, StatsResponse } from '../models/notificacion.model';

@Injectable({
  providedIn: 'root'
})
export class NotificacionesService {
  private apiUrl = 'http://localhost:5100/api';

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

  getStats(): Observable<StatsResponse> {
    return this.http.get<StatsResponse>(`${this.apiUrl}/Notificaciones/stats`);
  }

  create(notificacion: Partial<Notificacion>): Observable<Notificacion> {
    return this.http.post<Notificacion>(`${this.apiUrl}/Notificaciones`, notificacion);
  }

  reintentar(id: number): Observable<Notificacion> {
    return this.http.post<Notificacion>(`${this.apiUrl}/Notificaciones/${id}/reintentar`, {});
  }
}
