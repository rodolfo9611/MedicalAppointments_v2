import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResult } from '../models/cita.model';
import { HistorialEvento, Paciente } from '../models/paciente.model';

@Injectable({
  providedIn: 'root'
})
export class PacientesService {
  private apiUrl = environment.api.pacientes;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Paciente[]> {
    return this.http.get<Paciente[]>(`${this.apiUrl}/Pacientes`);
  }

  getById(id: number): Observable<Paciente> {
    return this.http.get<Paciente>(`${this.apiUrl}/Pacientes/${id}`);
  }

  getPaged(pageNumber: number = 1, pageSize: number = 10, filter?: string, orderBy?: string): Observable<PagedResult<Paciente>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    if (filter?.trim()) params = params.set('filter', filter.trim());
    if (orderBy?.trim()) params = params.set('orderBy', orderBy.trim());

    return this.http.get<PagedResult<Paciente>>(`${this.apiUrl}/Pacientes/paged`, { params });
  }

  create(paciente: Partial<Paciente>): Observable<Paciente> {
    return this.http.post<Paciente>(`${this.apiUrl}/Pacientes`, paciente);
  }

  update(id: number, paciente: Partial<Paciente>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Pacientes/${id}`, paciente);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Pacientes/${id}`);
  }

  getHistorial(pacienteId: number): Observable<HistorialEvento[]> {
    return this.http.get<HistorialEvento[]>(`${this.apiUrl}/Pacientes/${pacienteId}/historial`);
  }

  addHistorialEvento(pacienteId: number, evento: Partial<HistorialEvento>): Observable<HistorialEvento> {
    return this.http.post<HistorialEvento>(`${this.apiUrl}/Pacientes/${pacienteId}/historial/eventos`, evento);
  }
}
