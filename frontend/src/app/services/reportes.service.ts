import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { DashboardResponse, ReporteFiltros, ReporteRow } from '../models/reportes.model';

@Injectable({
  providedIn: 'root'
})
export class ReportesService {
  private apiUrl = environment.api.reportes;

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardResponse> {
    return this.http.get<DashboardResponse>(`${this.apiUrl}/dashboard`);
  }

  getPacientes(filtros: ReporteFiltros = {}): Observable<ReporteRow[]> {
    return this.get('pacientes', filtros);
  }

  getCitas(filtros: ReporteFiltros = {}): Observable<ReporteRow[]> {
    return this.get('citas', filtros);
  }

  getRecetas(filtros: ReporteFiltros = {}): Observable<ReporteRow[]> {
    return this.get('recetas', filtros);
  }

  getNotificaciones(filtros: ReporteFiltros = {}): Observable<ReporteRow[]> {
    return this.get('notificaciones', filtros);
  }

  private get(tipo: string, filtros: ReporteFiltros): Observable<ReporteRow[]> {
    let params = new HttpParams();
    Object.entries(filtros).forEach(([key, value]) => {
      if (value !== undefined && value !== null && `${value}`.trim() !== '') {
        params = params.set(key, `${value}`);
      }
    });
    return this.http.get<ReporteRow[]>(`${this.apiUrl}/reportes/${tipo}`, { params });
  }
}
