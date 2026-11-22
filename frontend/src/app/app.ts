import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CitasService } from './services/citas.service';
import { NotificacionesService } from './services/notificaciones.service';
import { Cita, EstadoCita, PagedResult } from './models/cita.model';
import { Notificacion, StatsResponse } from './models/notificacion.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent implements OnInit {
  activeSection: 'citas' | 'estados' | 'notificaciones' = 'citas';

  // Endpoint activo desplegado (estilo Swagger UI)
  endpointAbierto: string = 'getCitas';

  // Respuestas de cada endpoint (para mostrar Status y JSON devuelto)
  respuestas: { [key: string]: { status?: number; data?: any; error?: string; loading?: boolean } } = {};

  // 1. GET /api/Citas
  resultadoGetAll: Cita[] = [];

  // 2. GET /api/Citas/paged
  paramsPaged = {
    pageNumber: 1,
    pageSize: 5,
    filter: '',
    orderBy: 'CitaID desc'
  };

  // 3. GET /api/Citas/{id}
  paramGetById: number = 1;

  // 4. POST /api/Citas
  bodyCreate: Partial<Cita> = {
    pacienteID: 101,
    medicoID: 12,
    especialidadID: 1,
    consultorioID: 105,
    estadoCitaID: 1,
    fechaHoraInicio: new Date(Date.now() + 86400000).toISOString().slice(0, 16),
    fechaHoraFin: new Date(Date.now() + 88200000).toISOString().slice(0, 16),
    motivo: 'Consulta médica general',
    observaciones: 'Paciente citado desde interfaz web',
    usuarioCreacionID: 1
  };

  // 5. PUT /api/Citas/{id}
  paramPutId: number = 1;
  bodyUpdate: Partial<Cita> = {
    citaID: 1,
    pacienteID: 101,
    medicoID: 12,
    especialidadID: 1,
    consultorioID: 105,
    estadoCitaID: 2, // 2: Confirmada
    fechaHoraInicio: new Date().toISOString().slice(0, 16),
    motivo: 'Consulta médica actualizada',
    observaciones: 'Modificada desde el cliente web'
  };

  // 6. DELETE /api/Citas/{id}
  paramDeleteId: number = 1;

  // 7. GET /api/EstadosCita
  resultadoEstados: EstadoCita[] = [];

  // 8. GET /api/EstadosCita/{id}
  paramEstadoId: number = 1;

  // 9. GET /api/Notificaciones (Worker)
  resultadoNotificaciones: Notificacion[] = [];
  paramFiltroEstadoNotif: string = '';
  paramFiltroCanalNotif: string = '';

  // 10. GET /api/Notificaciones/stats
  resultadoStats: StatsResponse | null = null;

  // 11. POST /api/Notificaciones/{id}/reintentar
  paramReintentarNotifId: number = 1;

  constructor(
    private citasService: CitasService,
    private notificacionesService: NotificacionesService
  ) {}

  ngOnInit(): void {
    // Inicializar ejecutando el listado básico
    this.ejecutarGetAll();
  }

  toggleEndpoint(nombre: string): void {
    this.endpointAbierto = this.endpointAbierto === nombre ? '' : nombre;
  }

  // 1. GET /api/Citas
  ejecutarGetAll(): void {
    this.respuestas['getAll'] = { loading: true };
    this.citasService.getAll().subscribe({
      next: (data) => {
        this.resultadoGetAll = data;
        this.respuestas['getAll'] = { status: 200, data };
      },
      error: (err) => {
        this.respuestas['getAll'] = { status: err.status || 500, error: err.message };
      }
    });
  }

  // 2. GET /api/Citas/paged
  ejecutarGetPaged(): void {
    this.respuestas['getPaged'] = { loading: true };
    this.citasService
      .getPaged(
        this.paramsPaged.pageNumber,
        this.paramsPaged.pageSize,
        this.paramsPaged.filter,
        this.paramsPaged.orderBy
      )
      .subscribe({
        next: (data) => {
          this.respuestas['getPaged'] = { status: 200, data };
        },
        error: (err) => {
          this.respuestas['getPaged'] = { status: err.status || 500, error: err.message };
        }
      });
  }

  // 3. GET /api/Citas/{id}
  ejecutarGetById(): void {
    if (!this.paramGetById) return;
    this.respuestas['getById'] = { loading: true };
    this.citasService.getById(this.paramGetById).subscribe({
      next: (data) => {
        this.respuestas['getById'] = { status: 200, data };
        // Precargar en el formulario de Update para conveniencia del usuario
        this.paramPutId = data.citaID || this.paramGetById;
        this.bodyUpdate = { ...data };
      },
      error: (err) => {
        this.respuestas['getById'] = { status: err.status || 404, error: err.message };
      }
    });
  }

  // 4. POST /api/Citas
  ejecutarPostCita(): void {
    this.respuestas['postCita'] = { loading: true };
    this.citasService.create(this.bodyCreate).subscribe({
      next: (data) => {
        this.respuestas['postCita'] = { status: 201, data };
        this.ejecutarGetAll(); // Refrescar listado
      },
      error: (err) => {
        this.respuestas['postCita'] = { status: err.status || 400, error: err.message };
      }
    });
  }

  // 5. PUT /api/Citas/{id}
  ejecutarPutCita(): void {
    if (!this.paramPutId) return;
    this.respuestas['putCita'] = { loading: true };
    const citaActualizar: Cita = {
      citaID: this.paramPutId,
      pacienteID: Number(this.bodyUpdate.pacienteID) || 1,
      medicoID: Number(this.bodyUpdate.medicoID) || 1,
      especialidadID: this.bodyUpdate.especialidadID ? Number(this.bodyUpdate.especialidadID) : undefined,
      consultorioID: this.bodyUpdate.consultorioID ? Number(this.bodyUpdate.consultorioID) : undefined,
      estadoCitaID: Number(this.bodyUpdate.estadoCitaID) || 1,
      fechaHoraInicio: this.bodyUpdate.fechaHoraInicio || new Date().toISOString(),
      fechaHoraFin: this.bodyUpdate.fechaHoraFin,
      motivo: this.bodyUpdate.motivo,
      observaciones: this.bodyUpdate.observaciones
    };

    this.citasService.update(this.paramPutId, citaActualizar).subscribe({
      next: () => {
        this.respuestas['putCita'] = { status: 204, data: { mensaje: '204 NoContent - Cita actualizada exitosamente' } };
        this.ejecutarGetAll();
      },
      error: (err) => {
        this.respuestas['putCita'] = { status: err.status || 400, error: err.message };
      }
    });
  }

  // 6. DELETE /api/Citas/{id}
  ejecutarDeleteCita(): void {
    if (!this.paramDeleteId) return;
    this.respuestas['deleteCita'] = { loading: true };
    this.citasService.delete(this.paramDeleteId).subscribe({
      next: () => {
        this.respuestas['deleteCita'] = { status: 204, data: { mensaje: '204 NoContent - Cita eliminada exitosamente' } };
        this.ejecutarGetAll();
      },
      error: (err) => {
        this.respuestas['deleteCita'] = { status: err.status || 404, error: err.message };
      }
    });
  }

  // 7. GET /api/EstadosCita
  ejecutarGetEstados(): void {
    this.respuestas['getEstados'] = { loading: true };
    this.citasService.getEstados().subscribe({
      next: (data) => {
        this.resultadoEstados = data;
        this.respuestas['getEstados'] = { status: 200, data };
      },
      error: (err) => {
        this.respuestas['getEstados'] = { status: err.status || 500, error: err.message };
      }
    });
  }

  // 8. GET /api/EstadosCita/{id}
  ejecutarGetEstadoById(): void {
    if (!this.paramEstadoId) return;
    this.respuestas['getEstadoById'] = { loading: true };
    this.citasService.getEstadoById(this.paramEstadoId).subscribe({
      next: (data) => {
        this.respuestas['getEstadoById'] = { status: 200, data };
      },
      error: (err) => {
        this.respuestas['getEstadoById'] = { status: err.status || 404, error: err.message };
      }
    });
  }

  // 9. GET /api/Notificaciones
  ejecutarGetNotificaciones(): void {
    this.respuestas['getNotificaciones'] = { loading: true };
    this.notificacionesService.getAll(this.paramFiltroEstadoNotif, this.paramFiltroCanalNotif).subscribe({
      next: (data) => {
        this.resultadoNotificaciones = data;
        this.respuestas['getNotificaciones'] = { status: 200, data };
      },
      error: (err) => {
        this.respuestas['getNotificaciones'] = { status: err.status || 500, error: err.message };
      }
    });
  }

  // 10. GET /api/Notificaciones/stats
  ejecutarGetStats(): void {
    this.respuestas['getStats'] = { loading: true };
    this.notificacionesService.getStats().subscribe({
      next: (data) => {
        this.resultadoStats = data;
        this.respuestas['getStats'] = { status: 200, data };
      },
      error: (err) => {
        this.respuestas['getStats'] = { status: err.status || 500, error: err.message };
      }
    });
  }

  // 11. POST /api/Notificaciones/{id}/reintentar
  ejecutarReintentarNotificacion(): void {
    if (!this.paramReintentarNotifId) return;
    this.respuestas['reintentarNotif'] = { loading: true };
    this.notificacionesService.reintentar(this.paramReintentarNotifId).subscribe({
      next: (data) => {
        this.respuestas['reintentarNotif'] = { status: 200, data };
        this.ejecutarGetNotificaciones();
      },
      error: (err) => {
        this.respuestas['reintentarNotif'] = { status: err.status || 404, error: err.message };
      }
    });
  }
}

export { AppComponent as App };
