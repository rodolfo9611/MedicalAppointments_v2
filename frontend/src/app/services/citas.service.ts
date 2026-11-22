import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cita, EstadoCita, PagedResult } from '../models/cita.model';

@Injectable({
  providedIn: 'root'
})
export class CitasService {
  private apiUrl = 'http://localhost:5000/api';

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

  // 4. POST /api/Citas
  create(cita: Partial<Cita>): Observable<{ citaID: number }> {
    return this.http.post<{ citaID: number }>(`${this.apiUrl}/Citas`, cita);
  }

  // 5. PUT /api/Citas/{id}
  update(id: number, cita: Cita): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Citas/${id}`, cita);
  }

  // 6. DELETE /api/Citas/{id}
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Citas/${id}`);
  }

  // 7. GET /api/EstadosCita
  getEstados(): Observable<EstadoCita[]> {
    return this.http.get<EstadoCita[]>(`${this.apiUrl}/EstadosCita`);
  }

  // 8. GET /api/EstadosCita/{id}
  getEstadoById(id: number): Observable<EstadoCita> {
    return this.http.get<EstadoCita>(`${this.apiUrl}/EstadosCita/${id}`);
  }
}
