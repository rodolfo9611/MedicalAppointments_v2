import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { catchError, forkJoin, Observable, of } from 'rxjs';
import { environment } from '../environments/environment';
import { CitasService } from './services/citas.service';
import { NotificacionesService } from './services/notificaciones.service';
import { PacientesService } from './services/pacientes.service';
import { RecetasService } from './services/recetas.service';
import { ReportesService } from './services/reportes.service';
import {
  Cita,
  ESTADOS_CITA,
  EstadoCita,
  EstadoCitaEnum,
  UpdateCitaDto,
  estadoCitaFromId
} from './models/cita.model';
import {
  CANALES_NOTIFICACION,
  ESTADOS_NOTIFICACION,
  Notificacion,
  StatsResponse,
  TIPOS_NOTIFICACION
} from './models/notificacion.model';
import { HistorialEvento, Paciente } from './models/paciente.model';
import {
  CrearRecetaDto,
  RecetaCreadaResponse,
  RecetaMedica,
  citaIdReceta,
  idReceta,
  recetaAnulada
} from './models/receta.model';
import { DashboardResponse, ReporteFiltros, ReporteRow } from './models/reportes.model';

type Seccion = 'pacientes' | 'citas' | 'estados' | 'recetas' | 'reportes' | 'notificaciones';
type SeccionClinica = 'dashboard' | 'agenda' | 'pacientes' | 'historial' | 'recetas' | 'notificaciones' | 'reportes';
type Respuesta = { status?: number; data?: any; error?: string; loading?: boolean };
type Tono = 'blue' | 'cyan' | 'green' | 'amber' | 'slate' | 'violet';

interface KpiDestino {
  vista: SeccionClinica;
  modo?: 'calendario' | 'semana' | 'kanban';
  estado?: EstadoCitaEnum;
}

interface KpiView {
  etiqueta: string;
  valor: number | string;
  detalle: string;
  icono: string;
  tono: Tono;
  destino?: KpiDestino;
}

interface EstadoView {
  nombre: string;
  cantidad: number;
  color: string;
  porcentaje: number;
}

interface TendenciaView {
  etiqueta: string;
  citas: number;
  recetas: number;
  total: number;
  altura: number;
}

interface ActividadView {
  icono: string;
  titulo: string;
  detalle: string;
  fecha: string;
  tono: Tono;
}

interface ProximaCitaView {
  citaID: number;
  paciente: string;
  medico: string;
  especialidad: string;
  fecha: string;
  hora: string;
  estado: string;
  tono: Tono;
}

interface PacienteCitaForm {
  modo: 'existente' | 'nuevo';
  pacienteID: number | null;
  nombre: string;
  apellido: string;
  documento: string;
  fechaNacimiento: string;
  telefono: string;
  correoElectronico: string;
}

interface NuevaCitaForm {
  especialidadID: number;
  medicoID: number;
  fecha: string;
  hora: string;
  motivo: string;
  canalNotificacion: string;
}

interface ResultadoNuevaCita {
  pacienteID: number;
  codigoPaciente: string;
  citaID: number;
  fechaHora: string;
  notificacionEnviada: boolean;
  detalleNotificacion: string;
}

interface CanalView {
  canal: string;
  cantidad: number;
  porcentaje: number;
}

interface DiaCalendarioView {
  fecha: Date;
  numero: number;
  mesDistinto: boolean;
  hoy: boolean;
  citas: Cita[];
}

interface DiaSemanaView {
  fecha: Date;
  etiqueta: string;
  numero: number;
  hoy: boolean;
}

interface ColumnaKanbanView {
  estado: EstadoCitaEnum;
  nombre: string;
  color: string;
  tono: Tono;
  citas: Cita[];
}

interface AccionCitaView {
  etiqueta: string;
  destino: EstadoCitaEnum;
  tono: Tono;
}

interface PacienteCardView {
  id: number;
  codigo: string;
  nombre: string;
  documento: string;
  telefono: string;
  correo: string;
  edad: number | null;
  ultimaConsulta: string;
  estado: string;
  tono: Tono;
  totalCitas: number;
  totalRecetas: number;
  inicial: string;
}

interface TimelineEventView {
  id: string;
  fecha: string;
  titulo: string;
  descripcion: string;
  tipo: string;
  icono: string;
  tono: Tono;
  citaID?: number;
  recetaID?: number;
}

interface RecetaCardView {
  id: number;
  citaID: number;
  paciente: string;
  medico: string;
  especialidad: string;
  fecha: string;
  diagnostico: string;
  indicaciones: string;
  tratamiento: string;
  observaciones: string;
  anulada: boolean;
  motivoAnulacion: string;
}

interface NotificacionCardView {
  id: number;
  tipo: string;
  titulo: string;
  estado: string;
  canal: string;
  paciente: string;
  destinatario: string;
  fecha: string;
  mensaje: string;
  intentos: number;
  tono: Tono;
  icono: string;
}

type TipoReporte = 'pacientes' | 'citas' | 'recetas' | 'notificaciones';

interface ReporteCategoriaView {
  etiqueta: string;
  cantidad: number;
  porcentaje: number;
  color: string;
}

interface ColumnaReporteView {
  clave: string;
  etiqueta: string;
}

type DrawerActivo = 'ninguno' | 'cita' | 'paciente' | 'nueva-cita';

interface PasoNavegacion {
  etiqueta: string;
  vista: SeccionClinica;
  drawer: DrawerActivo;
  devMode: boolean;
  pacienteHistorialSeleccionado: number | null;
  modoAgenda: 'calendario' | 'semana' | 'kanban';
  anioCalendario: number;
  mesCalendario: number;
  busquedaAgenda: string;
  filtroMedicoAgenda: string;
  busquedaPacientes: string;
  filtroEstadoPaciente: string;
  busquedaHistorial: string;
  filtroTipoHistorial: string;
  busquedaRecetas: string;
  pacienteRecetasFiltro: number | null;
  reporteActivo: TipoReporte;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css', './clinical.css', './agenda.css', './modules.css']
})
export class AppComponent implements OnInit {
  activeSection: Seccion = 'citas';

  // ================= NUEVA EXPERIENCIA CLÍNICA =================
  vistaClinica: SeccionClinica = 'dashboard';
  devMode = false;
  sidebarAbierta = false;
  avisoPrincipal = '';
  private pilaNavegacion: PasoNavegacion[] = [];
  private restaurandoNavegacion = false;
  private readonly maxPasosNavegacion = 40;
  cargandoDashboard = true;
  errorDashboard = '';
  hoy = new Date();

  dashboard: DashboardResponse | null = null;
  citasDashboard: Cita[] = [];
  pacientesDashboard: Paciente[] = [];
  recetasDashboard: RecetaMedica[] = [];
  notificacionesDashboard: Notificacion[] = [];
  statsDashboard: StatsResponse | null = null;

  // ========================== CREAR CITA NUEVA ===========================
  drawerNuevaCitaAbierta = false;
  guardandoNuevaCita = false;
  mensajeNuevaCita = '';
  resultadoNuevaCita: ResultadoNuevaCita | null = null;
  pacienteCitaForm: PacienteCitaForm = {
    modo: 'existente',
    pacienteID: null,
    nombre: '',
    apellido: '',
    documento: '',
    fechaNacimiento: '',
    telefono: '',
    correoElectronico: ''
  };
  nuevaCitaForm: NuevaCitaForm = {
    especialidadID: 1,
    medicoID: 1,
    fecha: '',
    hora: '09:00',
    motivo: '',
    canalNotificacion: 'WhatsApp'
  };
  /** Catálogo visual de la clínica; no requiere endpoints nuevos. */
  readonly especialidadesClinica: Array<{ id: number; nombre: string }> = [
    { id: 1, nombre: 'Medicina General' },
    { id: 2, nombre: 'Cardiología' },
    { id: 3, nombre: 'Pediatría' },
    { id: 4, nombre: 'Dermatología' },
    { id: 5, nombre: 'Ginecología' },
    { id: 6, nombre: 'Ortopedia' },
    { id: 7, nombre: 'Oftalmología' },
    { id: 8, nombre: 'Psicología' }
  ];
  readonly medicosClinica: Array<{ id: number; nombre: string; especialidadID: number }> = [
    { id: 1, nombre: 'Dra. Mariana Herrera', especialidadID: 1 },
    { id: 2, nombre: 'Dr. Andrés Molina', especialidadID: 2 },
    { id: 3, nombre: 'Dra. Camila Rojas', especialidadID: 3 },
    { id: 4, nombre: 'Dra. Elena Vargas', especialidadID: 4 },
    { id: 5, nombre: 'Dra. Patricia Menéndez', especialidadID: 5 },
    { id: 6, nombre: 'Dr. Mauricio León', especialidadID: 6 },
    { id: 7, nombre: 'Dra. Lucía Ferrer', especialidadID: 7 },
    { id: 8, nombre: 'Lic. Ana Beltrán', especialidadID: 8 },
    { id: 9, nombre: 'Dr. Javier Cordero', especialidadID: 1 },
    { id: 10, nombre: 'Dra. Sofía Ramírez', especialidadID: 2 },
    { id: 11, nombre: 'Dr. Diego Salazar', especialidadID: 3 },
    { id: 12, nombre: 'Dra. Valentina Cruz', especialidadID: 4 }
  ];

  // ============================ AGENDA MÉDICA ============================
  modoAgenda: 'calendario' | 'semana' | 'kanban' = 'calendario';
  anioCalendario = new Date().getFullYear();
  mesCalendario = new Date().getMonth();
  drawerCitaAbierta = false;
  busquedaAgenda = '';
  filtroMedicoAgenda = '';
  pacienteHistorialSeleccionado: number | null = null;

  // =========================== PACIENTES ================================
  busquedaPacientes = '';
  filtroEstadoPaciente = '';
  drawerPacienteAbierta = false;
  pacienteEditando: Partial<Paciente> = {};
  guardandoPaciente = false;
  mensajePaciente = '';
  pacienteAgendaFiltro: number | null = null;
  canalNotificacionReprogramacion = 'WhatsApp';
  mensajeReprogramacion = '';

  // ========================== HISTORIAL =================================
  historialEventos: HistorialEvento[] = [];
  historialCargando = false;
  busquedaHistorial = '';
  filtroTipoHistorial = '';

  // =========================== RECETAS ==================================
  recetasClinicas: RecetaMedica[] = [];
  recetasCargando = false;
  busquedaRecetas = '';
  incluirAnuladasRecetas = true;
  recetaAnulandoId: number | null = null;
  motivoAnulacion = '';
  mensajeRecetas = '';
  pacienteRecetasFiltro: number | null = null;

  // ======================== NOTIFICACIONES ==============================
  busquedaNotificaciones = '';
  filtroTipoNotificacion = '';
  filtroEstadoNotificacion = '';
  filtroCanalNotificacion = '';

  // ============================ REPORTES =================================
  reportesCargando = false;
  reporteActivo: TipoReporte = 'citas';
  reporteDesde = '';
  reporteHasta = '';
  reportes: Record<TipoReporte, ReporteRow[]> = {
    pacientes: [],
    citas: [],
    recetas: [],
    notificaciones: []
  };
  reporteEstado = '';
  reporteCanal = '';
  mensajeReportes = '';

  // ===================== FORMULARIOS VISUALES CLÍNICOS ===================
  nuevoEventoHistorial: {
    tipoEvento: string;
    descripcion: string;
    fechaEvento: string;
    citaID: number | null;
  } = {
    tipoEvento: 'Consulta',
    descripcion: '',
    fechaEvento: new Date().toISOString().slice(0, 16),
    citaID: null
  };
  mensajeHistorial = '';

  recetaEmissionCitaID: number | null = null;

  notificacionReintentandoId: number | null = null;
  mensajeNotificaciones = '';

  private readonly coloresEstados: Record<EstadoCitaEnum, string> = {
    Agendada: '#2563eb',
    Citada: '#0ea5e9',
    PendienteResultados: '#f59e0b',
    Concluida: '#16a34a',
    Cancelada: '#94a3b8'
  };

  readonly tonosEstados: Record<EstadoCitaEnum, Tono> = {
    Agendada: 'blue',
    Citada: 'cyan',
    PendienteResultados: 'amber',
    Concluida: 'green',
    Cancelada: 'slate'
  };

  // Endpoint activo desplegado (estilo Swagger UI)
  endpointAbierto: string = 'getCitas';

  // Respuestas de cada endpoint (para mostrar Status y JSON devuelto)
  respuestas: { [key: string]: Respuesta } = {};

  // Todas las URLs viven en environment.
  readonly baseUrls = environment.api;
  readonly estados: EstadoCita[] = ESTADOS_CITA;
  readonly estadosNotificacion = ESTADOS_NOTIFICACION;
  readonly canalesNotificacion = CANALES_NOTIFICACION;
  readonly tiposNotificacion = TIPOS_NOTIFICACION;
  readonly tiposReporte: TipoReporte[] = ['pacientes', 'citas', 'recetas', 'notificaciones'];
  readonly tiposEventoHistorial = [
    'Consulta',
    'AtencionRegistrada',
    'RecetaEmitida',
    'Reprogramacion',
    'NotificacionEnviada',
    'Seguimiento'
  ];
  readonly idReceta = idReceta;
  readonly citaIdReceta = citaIdReceta;

  // ================= CITAS =================
  resultadoGetAll: Cita[] = [];

  paramsPaged = {
    pageNumber: 1,
    pageSize: 5,
    filter: '',
    orderBy: 'CitaID desc'
  };

  paramGetById: number = 1;

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

  paramPutId: number = 1;
  /** Estado actual de la cita; se completa con GET /api/Citas/{id}. */
  estadoActualSeleccionado: EstadoCitaEnum = 'Agendada';
  citaSeleccionada: Cita | null = null;
  bodyUpdate: UpdateCitaDto = {
    estado: 'Agendada',
    fechaHoraInicio: new Date().toISOString().slice(0, 16),
    fechaHoraFin: '',
    motivo: '',
    observaciones: '',
    registroAtencion: { secuencia: 1, observaciones: '' },
    reprogramacion: { nuevaFechaHoraInicio: '', nuevaFechaHoraFin: '', motivo: '' }
  };

  paramDeleteId: number = 1;

  // Emisión de receta desde la cita (POST anidado)
  bodyReceta: CrearRecetaDto = { diagnostico: '', indicaciones: '', tratamiento: '', observaciones: '' };
  recetasCita: RecetaMedica[] = [];
  /** Resultado de GET /api/RecetasMedicas/cita/{citaId} (null = 404 sin receta activa). */
  recetaActivaCita: RecetaMedica | null = null;
  recetaActivaConsultada = false;
  ultimaRecetaId: number | null = null;

  // ================= ESTADOS =================
  resultadoEstados: EstadoCita[] = [];
  paramEstadoId: number = 1;

  // ================= PACIENTES =================
  resultadoPacientes: Paciente[] = [];
  paramPacienteId: number = 1;
  bodyPaciente: Partial<Paciente> = {
    codigoPaciente: 'PAC-000011',
    nombreCompleto: 'Mariana Quesada',
    documento: '06123466-7',
    correoElectronico: 'mariana.quesada@correo-clinica.com',
    telefono: '+503 7011 2233',
    fechaNacimiento: '1991-03-18'
  };
  paramPutPacienteId: number = 1;
  paramDeletePacienteId: number = 1;
  paramHistorialPacienteId: number = 1;
  bodyEventoHistorial: Partial<HistorialEvento> = {
    tipoEvento: 'Consulta',
    descripcion: 'Control clínico general',
    fechaEvento: new Date().toISOString().slice(0, 16)
  };

  // ================= RECETAS (solo consulta / historial / impresión) =================
  resultadoRecetas: RecetaMedica[] = [];
  paramGetRecetaId: number = 1;
  paramHistorialRecetasPacienteId: number = 1;
  paramsRecetasPaged = {
    pageNumber: 1,
    pageSize: 5,
    pacienteId: undefined as number | undefined,
    diagnostico: '',
    fechaDesde: '',
    fechaHasta: '',
    incluirAnuladas: false
  };

  // ================= REPORTES / DASHBOARD =================
  resultadoDashboard: DashboardResponse | null = null;
  filtrosReportePacientes: ReporteFiltros = { desde: '', hasta: '', pageNumber: 1, pageSize: 10 };
  filtrosReporteCitas: ReporteFiltros = { estado: '', desde: '', hasta: '', pageNumber: 1, pageSize: 10 };
  filtrosReporteRecetas: ReporteFiltros = { desde: '', hasta: '', pageNumber: 1, pageSize: 10 };
  filtrosReporteNotificaciones: ReporteFiltros = { estado: '', tipo: '', desde: '', hasta: '', pageNumber: 1, pageSize: 10 };

  // ================= NOTIFICACIONES =================
  resultadoNotificaciones: Notificacion[] = [];
  paramFiltroEstadoNotif: string = '';
  paramFiltroCanalNotif: string = '';
  paramsNotifPaged = { pageNumber: 1, pageSize: 5, estado: '', canal: '' };
  resultadoStats: StatsResponse | null = null;
  paramReintentarNotifId: number = 1;
  paramGetNotifId: number = 1;
  bodyNotificacion: Partial<Notificacion> = {
    citaID: 1,
    pacienteID: 2,
    pacienteNombre: 'Lucía Martínez',
    destinatario: '+503 7123 4567',
    canal: 'WhatsApp',
    mensaje: 'Recordatorio de cita médica: 25 nov a las 09:00.',
    estado: 'Pendiente',
    intentos: 0
  };
  paramPutNotifId: number = 1;
  paramDeleteNotifId: number = 1;

  constructor(
    private citasService: CitasService,
    private notificacionesService: NotificacionesService,
    private pacientesService: PacientesService,
    private recetasService: RecetasService,
    private reportesService: ReportesService
  ) {}

  ngOnInit(): void {
    this.cargarDashboard();
    this.ejecutarGetAll();
    this.cargarRecetas();
    this.cargarRecetaActivaDeCita();
  }

  // =====================================================================
  // SHELL CLÍNICO Y DASHBOARD
  // =====================================================================

  get puedeRetroceder(): boolean {
    return this.pilaNavegacion.length > 0;
  }

  get etiquetaPasoAnterior(): string {
    return this.pilaNavegacion.length
      ? this.pilaNavegacion[this.pilaNavegacion.length - 1].etiqueta
      : '';
  }

  get etiquetaRetroceso(): string {
    if (this.puedeRetroceder) return `Volver a ${this.etiquetaPasoAnterior}`;
    return this.devMode ? 'Volver a la vista clínica' : 'Volver al Dashboard';
  }

  get etiquetaPasoActual(): string {
    if (this.devMode) return 'API Console';
    if (this.drawerNuevaCitaAbierta) return 'Crear cita';
    if (this.drawerPacienteAbierta) {
      return this.pacienteEditando?.id ? 'Ficha del paciente' : 'Nuevo paciente';
    }
    if (this.drawerCitaAbierta) {
      const id = this.citaSeleccionada?.citaID ?? this.paramGetById;
      return id ? `Cita #${id}` : 'Detalle de cita';
    }
    const titulosCortos: Record<SeccionClinica, string> = {
      dashboard: 'Dashboard',
      agenda: 'Agenda',
      pacientes: 'Pacientes',
      historial: 'Historial',
      recetas: 'Recetas',
      notificaciones: 'Notificaciones',
      reportes: 'Reportes'
    };
    return titulosCortos[this.vistaClinica];
  }

  activarVista(vista: SeccionClinica): void {
    if (
      vista === this.vistaClinica &&
      !this.drawerCitaAbierta &&
      !this.drawerPacienteAbierta &&
      !this.drawerNuevaCitaAbierta &&
      !this.devMode
    ) {
      this.sidebarAbierta = false;
      return;
    }

    this.registrarPaso();
    this.irAVista(vista);
  }

  retroceder(): void {
    const paso = this.pilaNavegacion.pop();
    if (!paso) {
      if (this.devMode) {
        this.devMode = false;
        return;
      }
      if (this.vistaClinica !== 'dashboard') {
        this.sidebarAbierta = false;
        this.irAVista('dashboard');
      }
      return;
    }

    this.restaurandoNavegacion = true;
    this.devMode = paso.devMode;
    this.vistaClinica = paso.vista;
    this.drawerCitaAbierta = paso.drawer === 'cita';
    this.drawerPacienteAbierta = paso.drawer === 'paciente';
    this.drawerNuevaCitaAbierta = paso.drawer === 'nueva-cita';
    this.pacienteHistorialSeleccionado = paso.pacienteHistorialSeleccionado;
    this.modoAgenda = paso.modoAgenda;
    this.anioCalendario = paso.anioCalendario;
    this.mesCalendario = paso.mesCalendario;
    this.busquedaAgenda = paso.busquedaAgenda;
    this.filtroMedicoAgenda = paso.filtroMedicoAgenda;
    this.busquedaPacientes = paso.busquedaPacientes;
    this.filtroEstadoPaciente = paso.filtroEstadoPaciente;
    this.busquedaHistorial = paso.busquedaHistorial;
    this.filtroTipoHistorial = paso.filtroTipoHistorial;
    this.busquedaRecetas = paso.busquedaRecetas;
    this.pacienteRecetasFiltro = paso.pacienteRecetasFiltro;
    this.reporteActivo = paso.reporteActivo;
    this.sidebarAbierta = false;
    this.restaurandoNavegacion = false;

    if (!this.devMode) {
      if (this.vistaClinica === 'historial' && this.pacienteHistorialSeleccionado) {
        this.cargarHistorialClinico();
      }
      if (this.vistaClinica === 'recetas') {
        this.cargarRecetas();
      }
      if (this.vistaClinica === 'notificaciones') {
        this.cargarNotificacionesClinicas();
      }
      if (this.vistaClinica === 'reportes') {
        if (!this.pacientesDashboard.length) this.cargarDashboard();
        if (!this.reportes[this.reporteActivo].length) this.cargarReporteActivo();
      }
    }
  }

  toggleSidebar(): void {
    this.sidebarAbierta = !this.sidebarAbierta;
  }

  cerrarAvisoPrincipal(): void {
    this.avisoPrincipal = '';
  }

  toggleDevMode(activar?: boolean): void {
    const destino = activar ?? !this.devMode;
    if (destino === this.devMode) return;

    if (destino) {
      this.registrarPaso();
      this.devMode = true;
      this.sidebarAbierta = false;
      return;
    }

    this.retroceder();
    if (this.devMode) this.devMode = false;
  }

  private irAVista(vista: SeccionClinica): void {
    this.vistaClinica = vista;
    this.sidebarAbierta = false;
    this.drawerCitaAbierta = false;
    this.drawerPacienteAbierta = false;
    this.drawerNuevaCitaAbierta = false;

    if (vista === 'pacientes' && !this.pacientesDashboard.length) {
      this.cargarDashboard();
    }
    if (vista === 'recetas') {
      this.cargarRecetas();
    }
    if (vista === 'historial' && this.pacienteHistorialSeleccionado) {
      this.cargarHistorialClinico();
    }
    if (vista === 'notificaciones') {
      this.cargarNotificacionesClinicas();
    }
    if (vista === 'reportes') {
      if (!this.pacientesDashboard.length) this.cargarDashboard();
      if (!this.reportes[this.reporteActivo].length) this.cargarReporteActivo();
    }
  }

  private registrarPaso(): void {
    if (this.restaurandoNavegacion) return;

    this.pilaNavegacion.push({
      etiqueta: this.etiquetaPasoActual,
      vista: this.vistaClinica,
      drawer: this.drawerCitaAbierta
        ? 'cita'
        : this.drawerPacienteAbierta
          ? 'paciente'
          : this.drawerNuevaCitaAbierta
            ? 'nueva-cita'
            : 'ninguno',
      devMode: this.devMode,
      pacienteHistorialSeleccionado: this.pacienteHistorialSeleccionado,
      modoAgenda: this.modoAgenda,
      anioCalendario: this.anioCalendario,
      mesCalendario: this.mesCalendario,
      busquedaAgenda: this.busquedaAgenda,
      filtroMedicoAgenda: this.filtroMedicoAgenda,
      busquedaPacientes: this.busquedaPacientes,
      filtroEstadoPaciente: this.filtroEstadoPaciente,
      busquedaHistorial: this.busquedaHistorial,
      filtroTipoHistorial: this.filtroTipoHistorial,
      busquedaRecetas: this.busquedaRecetas,
      pacienteRecetasFiltro: this.pacienteRecetasFiltro,
      reporteActivo: this.reporteActivo
    });

    if (this.pilaNavegacion.length > this.maxPasosNavegacion) {
      this.pilaNavegacion.shift();
    }
  }

  private consumirPasoDrawer(tipo: DrawerActivo): void {
    const ultimo = this.pilaNavegacion[this.pilaNavegacion.length - 1];
    if (!ultimo || ultimo.devMode) return;
    if (ultimo.drawer === tipo || ultimo.vista === this.vistaClinica) {
      this.pilaNavegacion.pop();
    }
  }

  cargarDashboard(): void {
    this.cargandoDashboard = true;
    this.errorDashboard = '';

    forkJoin({
      resumen: this.reportesService.getDashboard().pipe(catchError(() => of(null))),
      citas: this.citasService.getAll().pipe(catchError(() => of([] as Cita[]))),
      pacientes: this.pacientesService.getAll().pipe(catchError(() => of([] as Paciente[]))),
      recetas: this.recetasService.getAll().pipe(catchError(() => of([] as RecetaMedica[]))),
      notificaciones: this.notificacionesService.getAll().pipe(catchError(() => of([] as Notificacion[]))),
      stats: this.notificacionesService.getStats().pipe(catchError(() => of(null)))
    }).subscribe({
      next: (data) => {
        this.dashboard = data.resumen;
        this.citasDashboard = data.citas ?? [];
        this.pacientesDashboard = data.pacientes ?? [];
        this.recetasDashboard = data.recetas ?? [];
        this.notificacionesDashboard = data.notificaciones ?? [];
        this.statsDashboard = data.stats;
        this.hoy = new Date();
        this.cargandoDashboard = false;
        if (!data.resumen) {
          this.errorDashboard = 'No fue posible cargar el resumen ejecutivo. Los datos locales siguen disponibles.';
        }
      },
      error: () => {
        this.errorDashboard = 'No fue posible cargar el dashboard clínico.';
        this.cargandoDashboard = false;
      }
    });
  }

  get fechaHoyTexto(): string {
    return new Intl.DateTimeFormat('es-ES', {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric'
    }).format(this.hoy);
  }

  get saludo(): string {
    const hora = this.hoy.getHours();
    if (hora < 12) return 'Buenos días';
    if (hora < 19) return 'Buenas tardes';
    return 'Buenas noches';
  }

  get tituloVista(): string {
    const titulos: Record<SeccionClinica, string> = {
      dashboard: 'Dashboard Ejecutivo',
      agenda: 'Agenda Médica',
      pacientes: 'Pacientes',
      historial: 'Historial Clínico',
      recetas: 'Recetas Médicas',
      notificaciones: 'Centro de Notificaciones',
      reportes: 'Reportes de Gestión'
    };
    return titulos[this.vistaClinica];
  }

  get iconoVista(): string {
    const iconos: Record<SeccionClinica, string> = {
      dashboard: '◫',
      agenda: '▦',
      pacientes: '◎',
      historial: '◷',
      recetas: '℞',
      notificaciones: '✉',
      reportes: '▥'
    };
    return iconos[this.vistaClinica];
  }

  get numeroBadgeCitas(): number {
    return this.numero(this.dashboard?.citasHoy);
  }

  get numeroBadgePacientes(): number {
    return this.numero(this.dashboard?.totalPacientes);
  }

  get kpis(): KpiView[] {
    const d = this.dashboard;
    const atendidosHoy = new Set(
      this.citasDashboard
        .filter((c) => this.esHoy(c.fechaHoraInicio) && this.estadoDe(c) === 'Concluida')
        .map((c) => c.pacienteID)
    ).size;

    return [
      { etiqueta: 'Total Pacientes', valor: this.numero(d?.totalPacientes), detalle: 'Pacientes registrados', icono: '◎', tono: 'blue', destino: { vista: 'pacientes' } },
      { etiqueta: 'Citas Hoy', valor: this.numero(d?.citasHoy), detalle: 'Agenda del día', icono: '◷', tono: 'cyan', destino: { vista: 'agenda', modo: 'calendario' } },
      { etiqueta: 'Atendidos Hoy', valor: atendidosHoy, detalle: 'Citas concluidas hoy', icono: '✓', tono: 'green', destino: { vista: 'agenda', modo: 'kanban', estado: 'Concluida' } },
      { etiqueta: 'Citas Agendadas', valor: this.numero(d?.citasAgendadas), detalle: 'Pendientes de atención', icono: '▤', tono: 'blue', destino: { vista: 'agenda', modo: 'kanban', estado: 'Agendada' } },
      { etiqueta: 'Citas Citadas', valor: this.numero(d?.citasCitadas), detalle: 'Intervención post-consulta', icono: '✚', tono: 'cyan', destino: { vista: 'agenda', modo: 'kanban', estado: 'Citada' } },
      { etiqueta: 'Pendiente Resultados', valor: this.numero(d?.citasPendienteResultados), detalle: 'En seguimiento', icono: '◌', tono: 'amber', destino: { vista: 'agenda', modo: 'kanban', estado: 'PendienteResultados' } },
      { etiqueta: 'Citas Concluidas', valor: this.numero(d?.citasConcluidas), detalle: 'Altas clínicas', icono: '✓', tono: 'green', destino: { vista: 'agenda', modo: 'kanban', estado: 'Concluida' } },
      { etiqueta: 'Citas Canceladas', valor: this.numero(d?.citasCanceladas), detalle: 'Registro histórico', icono: '×', tono: 'slate', destino: { vista: 'agenda', modo: 'kanban', estado: 'Cancelada' } },
      { etiqueta: 'Recetas Emitidas', valor: this.numero(d?.recetasEmitidas), detalle: `${this.numero(d?.recetasHoy)} hoy`, icono: '℞', tono: 'violet', destino: { vista: 'recetas' } },
      { etiqueta: 'Notificaciones', valor: this.numero(d?.notificacionesGeneradas), detalle: `${this.numero(d?.notificacionesHoy)} hoy`, icono: '✉', tono: 'violet', destino: { vista: 'notificaciones' } }
    ];
  }

  abrirKpi(kpi: KpiView): void {
    const destino = kpi?.destino;
    if (!destino) return;

    // Primero se registra el paso actual para que "Volver" regrese al Dashboard.
    this.activarVista(destino.vista);

    if (destino.estado) {
      this.modoAgenda = 'kanban';
      this.busquedaAgenda = this.etiquetaEstado(destino.estado);
      this.filtroMedicoAgenda = '';
      return;
    }

    if (destino.modo) {
      this.modoAgenda = destino.modo;
      this.busquedaAgenda = '';
      this.filtroMedicoAgenda = '';
      if (destino.modo === 'calendario') this.irHoyCalendario();
    }
  }

  get estadosDashboard(): EstadoView[] {
    const d = this.dashboard;
    const datos: Array<{ nombre: EstadoCitaEnum; cantidad: number }> = [
      { nombre: 'Agendada', cantidad: this.numero(d?.citasAgendadas) },
      { nombre: 'Citada', cantidad: this.numero(d?.citasCitadas) },
      { nombre: 'PendienteResultados', cantidad: this.numero(d?.citasPendienteResultados) },
      { nombre: 'Concluida', cantidad: this.numero(d?.citasConcluidas) },
      { nombre: 'Cancelada', cantidad: this.numero(d?.citasCanceladas) }
    ];
    const total = datos.reduce((sum, item) => sum + item.cantidad, 0);
    return datos.map((item) => ({
      nombre: this.etiquetaEstado(item.nombre),
      cantidad: item.cantidad,
      color: this.coloresEstados[item.nombre],
      porcentaje: total ? Math.round((item.cantidad / total) * 100) : 0
    }));
  }

  get totalCitasDashboard(): number {
    return this.estadosDashboard.reduce((sum, item) => sum + item.cantidad, 0);
  }

  get donutGradient(): string {
    const total = this.totalCitasDashboard;
    if (!total) return 'conic-gradient(#e2e8f0 0 100%)';
    let inicio = 0;
    const segmentos = this.estadosDashboard.map((item) => {
      const fin = inicio + (item.cantidad / total) * 100;
      const segmento = `${item.color} ${inicio}% ${fin}%`;
      inicio = fin;
      return segmento;
    });
    return `conic-gradient(${segmentos.join(', ')})`;
  }

  get tendenciaSemanal(): TendenciaView[] {
    return this.construirTendencia(7, 'dia');
  }

  get tendenciaMensual(): TendenciaView[] {
    return this.construirTendencia(6, 'mes');
  }

  get maxTendencia(): number {
    const todos = [...this.tendenciaSemanal, ...this.tendenciaMensual];
    return Math.max(1, ...todos.map((item) => item.total));
  }

  get canalesDashboard(): CanalView[] {
    const desglose = this.statsDashboard?.desgloseCanales ?? [];
    const total = desglose.reduce((sum, item) => sum + this.numero(item.cantidad), 0);
    return desglose.map((item) => {
      const cantidad = this.numero(item.cantidad);
      return {
        canal: item.canal || 'Sin canal',
        cantidad,
        porcentaje: total ? Math.round((cantidad / total) * 100) : 0
      };
    });
  }

  get tasaEfectividad(): number {
    return this.numero(this.statsDashboard?.tasaEfectividadPorcentaje);
  }

  get proximasCitas(): ProximaCitaView[] {
    const ahora = Date.now();
    const citas = this.citasDashboard
      .filter((c) => {
        const estado = this.estadoDe(c);
        if (estado === 'Cancelada' || estado === 'Concluida') return false;
        const fecha = this.fechaDe(c.fechaHoraInicio);
        return !fecha || fecha.getTime() >= ahora - 86400000;
      })
      .sort((a, b) => (this.fechaDe(a.fechaHoraInicio)?.getTime() ?? 0) - (this.fechaDe(b.fechaHoraInicio)?.getTime() ?? 0))
      .slice(0, 5);

    return citas.map((cita) => {
      const estado = this.estadoDe(cita);
      return {
        citaID: Number(cita.citaID ?? 0),
        paciente: this.nombrePaciente(cita.pacienteID),
        medico: this.medicoVisible(cita),
        especialidad: cita.especialidad || 'Especialidad no indicada',
        fecha: this.formatearFechaCorta(cita.fechaHoraInicio),
        hora: cita.hora || this.formatearHora(cita.fechaHoraInicio),
        estado: this.etiquetaEstado(estado),
        tono: this.tonosEstados[estado]
      };
    });
  }

  get actividadReciente(): ActividadView[] {
    const actividad: ActividadView[] = [];

    this.citasDashboard.slice(-3).forEach((cita) => {
      actividad.push({
        icono: '◷',
        titulo: `Cita #${cita.citaID ?? ''} · ${this.etiquetaEstado(this.estadoDe(cita))}`,
        detalle: `${this.nombrePaciente(cita.pacienteID)} · ${this.medicoVisible(cita)}`,
        fecha: cita.fechaCreacion || cita.fechaHoraInicio || '',
        tono: this.tonosEstados[this.estadoDe(cita)]
      });
    });

    this.recetasDashboard.slice(-3).forEach((receta) => {
      actividad.push({
        icono: '℞',
        titulo: 'Receta médica emitida',
        detalle: this.texto(receta['nombrePaciente']) || this.nombrePaciente(Number(receta.pacienteID ?? receta['pacienteId'] ?? 0)),
        fecha: receta.fechaEmision || receta.fechaCreacion || '',
        tono: 'violet'
      });
    });

    this.notificacionesDashboard.slice(-3).forEach((notificacion) => {
      actividad.push({
        icono: '✉',
        titulo: `Notificación ${this.texto(notificacion.estado).toLowerCase()}`,
        detalle: notificacion.pacienteNombre || notificacion.destinatario,
        fecha: notificacion.fechaEnvio || notificacion.fechaRegistro || '',
        tono: notificacion.estado === 'Enviado' ? 'green' : notificacion.estado === 'Fallido' ? 'slate' : 'amber'
      });
    });

    return actividad
      .filter((item) => !!item.fecha)
      .sort((a, b) => (this.fechaDe(b.fecha)?.getTime() ?? 0) - (this.fechaDe(a.fecha)?.getTime() ?? 0))
      .slice(0, 6);
  }

  // =====================================================================
  // AGENDA MÉDICA
  // =====================================================================

  get mesCalendarioTexto(): string {
    return new Intl.DateTimeFormat('es-ES', { month: 'long', year: 'numeric' })
      .format(new Date(this.anioCalendario, this.mesCalendario, 1));
  }

  get medicosAgenda(): string[] {
    return Array.from(
      new Set(this.citasDashboard.map((cita) => this.medicoVisible(cita)).filter((medico): medico is string => !!medico))
    ).sort();
  }

  get citasAgendaFiltradas(): Cita[] {
    const busqueda = this.busquedaAgenda.trim().toLowerCase();
    return this.citasDashboard.filter((cita) => {
      if (this.filtroMedicoAgenda && this.medicoVisible(cita) !== this.filtroMedicoAgenda) return false;
      if (!busqueda) return true;
      const texto = [
        this.nombrePaciente(cita.pacienteID),
        this.medicoVisible(cita),
        cita.especialidad ?? '',
        cita.motivo ?? '',
        this.etiquetaEstado(this.estadoDe(cita))
      ].join(' ').toLowerCase();
      return texto.includes(busqueda);
    });
  }

  get diasCalendario(): DiaCalendarioView[] {
    const primero = new Date(this.anioCalendario, this.mesCalendario, 1);
    const desplazamiento = (primero.getDay() + 6) % 7;
    const inicio = new Date(this.anioCalendario, this.mesCalendario, 1 - desplazamiento);
    const hoy = new Date();

    return Array.from({ length: 42 }, (_, indice) => {
      const fecha = new Date(inicio.getFullYear(), inicio.getMonth(), inicio.getDate() + indice);
      const clave = this.claveDia(fecha);
      return {
        fecha,
        numero: fecha.getDate(),
        mesDistinto: fecha.getMonth() !== this.mesCalendario,
        hoy: this.claveDia(hoy) === clave,
        citas: this.citasAgendaFiltradas.filter((cita) => this.claveDia(cita.fechaHoraInicio) === clave)
      };
    });
  }

  get diasSemana(): DiaSemanaView[] {
    const hoy = new Date();
    return Array.from({ length: 7 }, (_, indice) => {
      const fecha = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate() + indice);
      return {
        fecha,
        etiqueta: new Intl.DateTimeFormat('es-ES', { weekday: 'short' }).format(fecha).replace('.', ''),
        numero: fecha.getDate(),
        hoy: indice === 0
      };
    });
  }

  get horasAgenda(): number[] {
    return Array.from({ length: 12 }, (_, indice) => indice + 8);
  }

  get columnasKanban(): ColumnaKanbanView[] {
    return ESTADOS_CITA.map((estado) => ({
      estado: estado.codigo,
      nombre: estado.nombre,
      color: this.coloresEstados[estado.codigo],
      tono: this.tonosEstados[estado.codigo],
      citas: this.citasAgendaFiltradas.filter((cita) => this.estadoDe(cita) === estado.codigo)
    }));
  }

  get accionesCita(): AccionCitaView[] {
    const actual = this.citaDrawer ? this.estadoDe(this.citaDrawer) : 'Agendada';
    switch (actual) {
      case 'Agendada':
        return [
          { etiqueta: 'Marcar como Citada', destino: 'Citada', tono: 'cyan' },
          { etiqueta: 'Pendiente de resultados', destino: 'PendienteResultados', tono: 'amber' },
          { etiqueta: 'Concluir atención', destino: 'Concluida', tono: 'green' }
        ];
      case 'Citada':
        return [
          { etiqueta: 'Pendiente de resultados', destino: 'PendienteResultados', tono: 'amber' },
          { etiqueta: 'Concluir atención', destino: 'Concluida', tono: 'green' }
        ];
      case 'PendienteResultados':
        return [
          { etiqueta: 'Marcar como Citada', destino: 'Citada', tono: 'cyan' },
          { etiqueta: 'Concluir atención', destino: 'Concluida', tono: 'green' }
        ];
      default:
        return [];
    }
  }

  get citaDrawer(): Cita | null {
    return this.citaSeleccionada;
  }

  get nombrePacienteDrawer(): string {
    return this.citaDrawer ? this.nombrePaciente(this.citaDrawer.pacienteID) : '';
  }

  get pacienteDrawer(): Paciente | null {
    if (!this.citaDrawer) return null;
    return this.pacientesDashboard.find((paciente) => this.idPaciente(paciente) === Number(this.citaDrawer!.pacienteID)) ?? null;
  }

  get destinatarioReprogramacion(): string {
    const paciente = this.pacienteDrawer;
    return this.canalNotificacionReprogramacion === 'Email'
      ? this.texto(paciente?.correoElectronico)
      : this.texto(paciente?.telefono);
  }

  get pacienteHistorialNombre(): string {
    if (!this.pacienteHistorialSeleccionado) return '';
    return this.nombrePaciente(this.pacienteHistorialSeleccionado);
  }

  motivoVisible(cita: Cita): string {
    const motivo = this.texto(cita.motivo).trim();
    if (!motivo) return 'No indicado';

    const legible = motivo
      .replace(/\b(?:e2e|2e2|qa|test|testing|pruebas?|temp|temporal|demo|sample|debug)\b/gi, '')
      .replace(/[_-]+/g, ' ')
      .replace(/\s+/g, ' ')
      .trim()
      .replace(/\bcancelacion\b/i, 'Cancelación');

    if (!legible) return 'Consulta general';
    return legible.charAt(0).toUpperCase() + legible.slice(1);
  }

  /** Las citas antiguas guardaron nombres genéricos; en pantalla se muestra un médico real del catálogo. */
  medicoVisible(cita: Cita | null | undefined): string {
    const guardado = this.texto(cita?.nombreMedico).trim();
    const generico = !guardado || /prueba|test|demo|médico asignado|medico asignado/i.test(guardado);
    if (!generico) return guardado;

    const porMedicoID = this.medicosClinica.find((medico) => medico.id === Number(cita?.medicoID));
    if (porMedicoID) return porMedicoID.nombre;

    const citaID = Number(cita?.citaID ?? 0);
    const indice = Number.isFinite(citaID) && citaID > 0
      ? (citaID - 1) % this.medicosClinica.length
      : 0;
    return this.medicosClinica[indice]?.nombre ?? 'Médico asignado';
  }

  seleccionarModoAgenda(modo: 'calendario' | 'semana' | 'kanban'): void {
    this.modoAgenda = modo;
  }

  cambiarMes(delta: number): void {
    const fecha = new Date(this.anioCalendario, this.mesCalendario + delta, 1);
    this.anioCalendario = fecha.getFullYear();
    this.mesCalendario = fecha.getMonth();
  }

  irHoyCalendario(): void {
    const hoy = new Date();
    this.anioCalendario = hoy.getFullYear();
    this.mesCalendario = hoy.getMonth();
  }

  citasDelDia(fecha: Date): Cita[] {
    const clave = this.claveDia(fecha);
    return this.citasAgendaFiltradas.filter((cita) => this.claveDia(cita.fechaHoraInicio) === clave);
  }

  citasEnHora(fecha: Date, hora: number): Cita[] {
    const clave = this.claveDia(fecha);
    const horaTexto = `${String(hora).padStart(2, '0')}:00`;
    return this.citasAgendaFiltradas.filter((cita) =>
      this.claveDia(cita.fechaHoraInicio) === clave &&
      this.formatearHora(cita.fechaHoraInicio) === horaTexto
    );
  }

  abrirCita(cita: Cita): void {
    const id = Number(cita.citaID ?? 0);
    if (!id) return;

    if (!this.drawerCitaAbierta) this.registrarPaso();

    this.paramGetById = id;
    this.paramPutId = id;
    this.paramDeleteId = id;
    this.paramHistorialPacienteId = Number(cita.pacienteID);
    this.citaSeleccionada = cita;
    this.estadoActualSeleccionado = this.estadoDe(cita);
    this.bodyUpdate = {
      estado: this.estadoActualSeleccionado,
      fechaHoraInicio: cita.fechaHoraInicio,
      fechaHoraFin: cita.fechaHoraFin ?? '',
      motivo: cita.motivo ?? '',
      observaciones: cita.observaciones ?? '',
      registroAtencion: { secuencia: 1, observaciones: '' },
      reprogramacion: {
        nuevaFechaHoraInicio: cita.fechaHoraInicio,
        nuevaFechaHoraFin: cita.fechaHoraFin ?? '',
        motivo: ''
      }
    };
    this.drawerCitaAbierta = true;
    this.cargarRecetaActivaDeCita();
  }

  cerrarDrawer(): void {
    if (!this.drawerCitaAbierta) return;
    this.consumirPasoDrawer('cita');
    this.drawerCitaAbierta = false;
  }

  verHistorialClinico(cita: Cita): void {
    this.registrarPaso();
    this.pacienteHistorialSeleccionado = Number(cita.pacienteID);
    this.paramHistorialPacienteId = this.pacienteHistorialSeleccionado;
    this.drawerCitaAbierta = false;
    this.irAVista('historial');
  }

  ejecutarTransicionAgenda(destino: EstadoCitaEnum): void {
    if (!this.citaDrawer?.citaID) return;
    this.paramPutId = Number(this.citaDrawer.citaID);
    this.estadoActualSeleccionado = this.estadoDe(this.citaDrawer);
    this.bodyUpdate = {
      estado: destino,
      fechaHoraInicio: this.citaDrawer.fechaHoraInicio,
      fechaHoraFin: this.citaDrawer.fechaHoraFin ?? '',
      motivo: this.citaDrawer.motivo ?? '',
      observaciones: this.citaDrawer.observaciones ?? '',
      registroAtencion: { secuencia: Number(this.bodyUpdate.registroAtencion?.secuencia ?? 1), observaciones: '' },
      reprogramacion: { nuevaFechaHoraInicio: this.citaDrawer.fechaHoraInicio, nuevaFechaHoraFin: this.citaDrawer.fechaHoraFin ?? '', motivo: '' }
    };
    if (destino === 'Concluida') this.cargarRecetaActivaDeCita();
    this.ejecutarPutCita();
  }

  reprogramarCitaAgenda(): void {
    if (!this.citaDrawer?.citaID || this.estadoDe(this.citaDrawer) !== 'PendienteResultados') return;
    const reprogramacion = this.bodyUpdate.reprogramacion;
    if (!reprogramacion?.nuevaFechaHoraInicio || !reprogramacion.motivo?.trim()) {
      this.respuestas['putCita'] = {
        status: 400,
        error: 'Indica la nueva fecha y el motivo de la reprogramación.'
      };
      return;
    }

    const cita = this.citaDrawer;
    const citaID = Number(cita.citaID);
    const pacienteID = Number(cita.pacienteID);
    const fechaAnterior = cita.fechaHoraInicio;
    const nuevaFecha = reprogramacion.nuevaFechaHoraInicio;
    const motivo = reprogramacion.motivo.trim();
    const nombrePaciente = this.nombrePaciente(pacienteID);
    const destinatario = this.destinatarioReprogramacion;
    const canal = this.canalNotificacionReprogramacion;

    this.paramPutId = citaID;
    this.estadoActualSeleccionado = 'PendienteResultados';
    this.bodyUpdate.estado = 'PendienteResultados';
    this.mensajeReprogramacion = '';

    this.ejecutarPutCita(() => {
      this.registrarNotificacionReprogramacion(
        citaID,
        pacienteID,
        nombrePaciente,
        destinatario,
        canal,
        fechaAnterior,
        nuevaFecha,
        motivo
      );
    });
  }

  private registrarNotificacionReprogramacion(
    citaID: number,
    pacienteID: number,
    nombrePaciente: string,
    destinatario: string,
    canal: string,
    fechaAnterior: string | undefined,
    nuevaFecha: string,
    motivo: string
  ): void {
    if (!destinatario) {
      this.mensajeReprogramacion =
        'La nueva fecha se guardó. No se registró el aviso: el paciente no tiene destinatario para ese canal.';
      this.cargarDashboard();
      return;
    }

    const cambio = fechaAnterior
      ? ` Antes: ${this.formatearFechaCorta(fechaAnterior)} a las ${this.formatearHora(fechaAnterior)}.`
      : '';
    const mensaje = `Su cita médica #${citaID} fue reprogramada para el ${this.formatearFechaCorta(nuevaFecha)} a las ${this.formatearHora(nuevaFecha)}.${cambio} Motivo: ${motivo}.`;
    const payload: Partial<Notificacion> = {
      citaID,
      pacienteID,
      pacienteNombre: nombrePaciente,
      destinatario,
      canal,
      mensaje,
      fechaProgramadaCita: nuevaFecha,
      estado: 'Pendiente',
      intentos: 0,
      tipoNotificacion: 'ReprogramacionCita'
    };

    this.notificacionesService.create(payload).subscribe({
      next: () => {
        this.mensajeReprogramacion =
          `Registro completado: ${nombrePaciente} · aviso de reprogramación por ${canal} para ${destinatario}.`;
        this.cargarNotificacionesClinicas();
        this.cargarDashboard();
      },
      error: () => {
        this.mensajeReprogramacion =
          'La nueva fecha se guardó, pero no se pudo registrar el aviso.';
        this.cargarDashboard();
      }
    });
  }

  cancelarCitaAgenda(): void {
    if (!this.citaDrawer?.citaID) return;
    this.paramDeleteId = Number(this.citaDrawer.citaID);
    this.ejecutarDeleteCita();
    this.cerrarDrawer();
  }

  verRecetaClinica(): void {
    const id = this.idRecetaActiva;
    if (id) window.open(this.recetasService.getUrlImprimible(id), '_blank');
  }

  imprimirRecetaClinica(): void {
    this.imprimirReceta();
  }

  toggleEndpoint(nombre: string): void {
    this.endpointAbierto = this.endpointAbierto === nombre ? '' : nombre;
  }

  private ejecutar<T>(
    clave: string,
    peticion: Observable<T>,
    estadoOk = 200,
    alTerminar?: (data: T) => void,
    alFallar?: (err: { status?: number; message?: string }) => void
  ): void {
    this.respuestas[clave] = { loading: true };
    peticion.subscribe({
      next: (data) => {
        this.respuestas[clave] = { status: estadoOk, data };
        alTerminar?.(data);
      },
      error: (err: { status?: number; message?: string }) => {
        this.respuestas[clave] = { status: err.status || 500, error: err.message || 'Error inesperado' };
        alFallar?.(err);
      }
    });
  }

  private normalizarEstado(cita: Cita): EstadoCitaEnum {
    if (cita.estado && this.estados.some((e) => e.codigo === cita.estado)) {
      return cita.estado;
    }
    return estadoCitaFromId(cita.estadoCitaID) ?? 'Agendada';
  }

  idPaciente(paciente: { pacienteID?: number; id?: number } | null | undefined): number {
    return Number(paciente?.pacienteID ?? paciente?.id ?? 0);
  }

  // =====================================================================
  // REGLAS DEL FLUJO CLÍNICO
  // =====================================================================

  /** Al salir de Agendada hacia cualquier estado post-consulta se exige RegistroAtencion. */
  get requiereRegistroAtencion(): boolean {
    const destino = this.bodyUpdate.estado;
    return (
      (this.estadoActualSeleccionado === 'Agendada' && destino !== 'Agendada') ||
      destino === 'Concluida'
    );
  }

  /** La reprogramación solo aplica cuando el estado actual y el destino son PendienteResultados. */
  get mostrarReprogramacion(): boolean {
    return (
      this.estadoActualSeleccionado === 'PendienteResultados' &&
      this.bodyUpdate.estado === 'PendienteResultados'
    );
  }

  /** Fuente de verdad: GET /api/RecetasMedicas/cita/{citaId}; null significa 404. */
  get recetaActivaDeCita(): RecetaMedica | null {
    return this.recetaActivaCita;
  }

  get idRecetaActiva(): number | null {
    return idReceta(this.recetaActivaDeCita) ?? this.ultimaRecetaId;
  }

  /** Emitir Receta solo para citas Concluida y cuando el endpoint devolvió 404. */
  get puedeEmitirReceta(): boolean {
    return (
      this.estadoActualSeleccionado === 'Concluida' &&
      this.recetaActivaConsultada &&
      !this.recetaActivaCita
    );
  }

  onEstadoCitaChange(nuevoEstado: EstadoCitaEnum): void {
    this.bodyUpdate.estado = nuevoEstado;
    if (!this.bodyUpdate.registroAtencion) {
      this.bodyUpdate.registroAtencion = { secuencia: 1, observaciones: '' };
    }
    if (!this.bodyUpdate.reprogramacion) {
      this.bodyUpdate.reprogramacion = { nuevaFechaHoraInicio: '', nuevaFechaHoraFin: '', motivo: '' };
    }
    if (nuevoEstado === 'Concluida' && !this.recetaActivaConsultada) {
      this.cargarRecetaActivaDeCita();
    }
  }

  cargarRecetas(): void {
    this.recetasCargando = true;
    this.recetasService
      .getPaged(1, 100, undefined, '', undefined, undefined, true)
      .subscribe({
        next: (data) => {
          const lista = this.extraerData<RecetaMedica>(data);
          this.recetasCita = lista;
          this.recetasClinicas = lista;
          this.recetasCargando = false;
        },
        error: () => {
          this.recetasCita = [];
          this.recetasClinicas = [];
          this.recetasCargando = false;
        }
      });
  }

  /** 404 => null => se habilita "Emitir Receta"; 200 => se muestran Ver/Imprimir. */
  private cargarRecetaActivaDeCita(): void {
    if (!this.paramPutId) return;
    this.recetaActivaConsultada = false;
    this.recetaActivaCita = null;
    this.recetasService.getActivaPorCitaSeguro(this.paramPutId).subscribe({
      next: (data) => {
        this.recetaActivaCita = data;
        this.recetaActivaConsultada = true;
      },
      error: () => {
        this.recetaActivaCita = null;
        this.recetaActivaConsultada = true;
      }
    });
  }

  // =====================================================================
  // CITAS
  // =====================================================================

  ejecutarGetAll(): void {
    this.ejecutar('getAll', this.citasService.getAll(), 200, (data) => {
      this.resultadoGetAll = data;
    });
  }

  ejecutarGetPaged(): void {
    this.ejecutar(
      'getPaged',
      this.citasService.getPaged(
        this.paramsPaged.pageNumber,
        this.paramsPaged.pageSize,
        this.paramsPaged.filter,
        this.paramsPaged.orderBy
      )
    );
  }

  ejecutarGetById(): void {
    if (!this.paramGetById) return;
    this.ejecutar('getById', this.citasService.getById(this.paramGetById), 200, (data) => {
      this.paramPutId = Number(data.citaID ?? this.paramGetById);
      this.citaSeleccionada = data;
      this.estadoActualSeleccionado = this.normalizarEstado(data);
      this.bodyUpdate = {
        estado: this.estadoActualSeleccionado,
        fechaHoraInicio: data.fechaHoraInicio,
        fechaHoraFin: data.fechaHoraFin ?? '',
        motivo: data.motivo,
        observaciones: data.observaciones,
        registroAtencion: { secuencia: 1, observaciones: '' },
        reprogramacion: {
          nuevaFechaHoraInicio: data.fechaHoraInicio,
          nuevaFechaHoraFin: data.fechaHoraFin ?? '',
          motivo: ''
        }
      };
      this.cargarRecetas();
      this.cargarRecetaActivaDeCita();
    });
  }

  ejecutarPostCita(): void {
    this.ejecutar('postCita', this.citasService.create(this.bodyCreate), 201, () => {
      this.ejecutarGetAll();
    });
  }

  ejecutarPutCita(alTerminar?: () => void): void {
    if (!this.paramPutId) return;

    const dto: UpdateCitaDto = {
      estado: this.bodyUpdate.estado,
      fechaHoraInicio: this.bodyUpdate.fechaHoraInicio || undefined,
      fechaHoraFin: this.bodyUpdate.fechaHoraFin || undefined,
      motivo: this.bodyUpdate.motivo || undefined,
      observaciones: this.bodyUpdate.observaciones || undefined
    };

    if (this.requiereRegistroAtencion) {
      const registro = this.bodyUpdate.registroAtencion;
      if (!registro || Number(registro.secuencia) < 1) {
        this.respuestas['putCita'] = {
          status: 400,
          error: 'Se requiere registroAtencion.secuencia >= 1 para salir de Agendada.'
        };
        return;
      }
      dto.registroAtencion = {
        secuencia: Number(registro.secuencia),
        fechaAtencion: registro.fechaAtencion || undefined,
        observaciones: registro.observaciones || undefined
      };
    }

    if (this.mostrarReprogramacion) {
      const reprogramacion = this.bodyUpdate.reprogramacion;
      if (!reprogramacion?.nuevaFechaHoraInicio || !reprogramacion.motivo?.trim()) {
        this.respuestas['putCita'] = {
          status: 400,
          error: 'La reprogramación requiere nuevaFechaHoraInicio y motivo.'
        };
        return;
      }
      dto.reprogramacion = {
        nuevaFechaHoraInicio: reprogramacion.nuevaFechaHoraInicio,
        nuevaFechaHoraFin: reprogramacion.nuevaFechaHoraFin || undefined,
        motivo: reprogramacion.motivo.trim()
      };
    }

    this.ejecutar(
      'putCita',
      this.citasService.updateProceso(this.paramPutId, dto),
      204,
      () => {
        this.respuestas['putCita'] = {
          status: 204,
          data: { mensaje: '204 NoContent - Proceso clínico actualizado exitosamente' }
        };
        this.estadoActualSeleccionado = dto.estado;
        this.ejecutarGetById();
        this.ejecutarGetAll();
        this.cargarDashboard();
        alTerminar?.();
      }
    );
  }

  ejecutarDeleteCita(): void {
    if (!this.paramDeleteId) return;
    this.ejecutar('deleteCita', this.citasService.delete(this.paramDeleteId), 204, () => {
      this.respuestas['deleteCita'] = {
        status: 204,
        data: { mensaje: '204 NoContent - Cita eliminada exitosamente' }
      };
      this.ejecutarGetAll();
      this.cargarDashboard();
    });
  }

  // =====================================================================
  // EMISIÓN Y CONSULTA DE RECETAS DESDE CITAS
  // =====================================================================

  ejecutarEmitirReceta(): void {
    if (!this.puedeEmitirReceta) return;
    const receta: CrearRecetaDto = {
      diagnostico: this.bodyReceta.diagnostico.trim(),
      indicaciones: this.bodyReceta.indicaciones.trim(),
      tratamiento: this.bodyReceta.tratamiento.trim(),
      observaciones: this.bodyReceta.observaciones?.trim() || undefined
    };
    if (!receta.diagnostico || !receta.indicaciones || !receta.tratamiento) {
      this.respuestas['emitirReceta'] = {
        status: 400,
        error: 'diagnostico, indicaciones y tratamiento son obligatorios.'
      };
      return;
    }

    this.ejecutar('emitirReceta', this.recetasService.emitirParaCita(this.paramPutId, receta), 201, (data: RecetaCreadaResponse) => {
      this.ultimaRecetaId = idReceta(data) ?? null;
      this.bodyReceta = { diagnostico: '', indicaciones: '', tratamiento: '', observaciones: '' };
      this.mensajeRecetas = `Receta emitida correctamente para la cita #${this.paramPutId}.`;
      this.recetaEmissionCitaID = null;
      this.cargarRecetas();
      this.cargarRecetaActivaDeCita();
      this.cargarDashboard();
    });
  }

  verRecetaDesdeCita(): void {
    const id = this.idRecetaActiva;
    if (!id) return;
    this.paramGetRecetaId = id;
    this.activeSection = 'recetas';
    this.endpointAbierto = 'getRecetaById';
    this.ejecutarGetRecetaById();
  }

  /** Tarjeta Swagger: GET /api/RecetasMedicas/cita/{citaId} (404 se muestra como respuesta). */
  ejecutarConsultarRecetaActivaCita(): void {
    if (!this.paramPutId) return;
    this.ejecutar(
      'getRecetaActivaCita',
      this.recetasService.getActivaPorCita(this.paramPutId),
      200,
      (data) => {
        this.recetaActivaCita = data;
        this.recetaActivaConsultada = true;
        this.ultimaRecetaId = idReceta(data) ?? this.ultimaRecetaId;
      },
      (err) => {
        if (err.status === 404) {
          this.recetaActivaCita = null;
          this.recetaActivaConsultada = true;
        }
      }
    );
  }

  imprimirReceta(id?: number): void {
    const recetaId = id ?? this.idRecetaActiva;
    if (!recetaId) return;
    const url = this.recetasService.getUrlImprimible(recetaId);
    const ventana = window.open(url, '_blank');
    if (ventana) {
      ventana.onload = () => {
        try {
          ventana.focus();
          ventana.print();
        } catch {
          window.print();
        }
      };
    } else {
      window.print();
    }
  }

  // =====================================================================
  // ESTADOS
  // =====================================================================

  ejecutarGetEstados(): void {
    this.ejecutar('getEstados', this.citasService.getEstados(), 200, (data) => {
      this.resultadoEstados = data;
    });
  }

  ejecutarGetEstadoById(): void {
    if (!this.paramEstadoId) return;
    this.ejecutar('getEstadoById', this.citasService.getEstadoById(this.paramEstadoId));
  }

  // =====================================================================
  // PACIENTES
  // =====================================================================

  ejecutarGetPacientes(): void {
    this.ejecutar('getPacientes', this.pacientesService.getAll(), 200, (data) => {
      this.resultadoPacientes = data ?? [];
    });
  }

  ejecutarGetPacienteById(): void {
    if (!this.paramPacienteId) return;
    this.ejecutar('getPacienteById', this.pacientesService.getById(this.paramPacienteId), 200, (data) => {
      this.paramPutPacienteId = this.idPaciente(data) || this.paramPacienteId;
      this.paramHistorialPacienteId = this.paramPutPacienteId;
      this.bodyPaciente = { ...data };
    });
  }

  ejecutarPostPaciente(): void {
    this.ejecutar('postPaciente', this.pacientesService.create(this.bodyPaciente), 201, () => {
      this.ejecutarGetPacientes();
    });
  }

  ejecutarPutPaciente(): void {
    if (!this.paramPutPacienteId) return;
    this.ejecutar('putPaciente', this.pacientesService.update(this.paramPutPacienteId, this.bodyPaciente), 204, () => {
      this.ejecutarGetPacientes();
    });
  }

  ejecutarDeletePaciente(): void {
    if (!this.paramDeletePacienteId) return;
    this.ejecutar('deletePaciente', this.pacientesService.delete(this.paramDeletePacienteId), 204, () => {
      this.ejecutarGetPacientes();
    });
  }

  ejecutarGetHistorialPaciente(): void {
    if (!this.paramHistorialPacienteId) return;
    this.ejecutar(
      'getHistorialPaciente',
      this.pacientesService.getHistorial(this.paramHistorialPacienteId)
    );
  }

  ejecutarPostEventoHistorial(): void {
    if (!this.paramHistorialPacienteId) return;
    this.ejecutar(
      'postEventoHistorial',
      this.pacientesService.addHistorialEvento(this.paramHistorialPacienteId, this.bodyEventoHistorial),
      201,
      () => this.ejecutarGetHistorialPaciente()
    );
  }

  verHistorial(pacienteId: number): void {
    if (!pacienteId) return;
    this.paramHistorialPacienteId = pacienteId;
    this.activeSection = 'pacientes';
    this.endpointAbierto = 'getHistorialPaciente';
    this.ejecutarGetHistorialPaciente();
  }

  // =====================================================================
  // RECETAS
  // =====================================================================

  ejecutarGetRecetas(): void {
    this.ejecutar('getRecetas', this.recetasService.getAll(), 200, (data) => {
      this.resultadoRecetas = data ?? [];
    });
  }

  ejecutarGetRecetaById(): void {
    if (!this.paramGetRecetaId) return;
    this.ejecutar('getRecetaById', this.recetasService.getById(this.paramGetRecetaId));
  }

  ejecutarHistorialRecetasPaciente(): void {
    if (!this.paramHistorialRecetasPacienteId) return;
    this.ejecutar(
      'historialRecetasPaciente',
      this.recetasService.getHistorialPaciente(this.paramHistorialRecetasPacienteId)
    );
  }

  ejecutarGetRecetasAnuladas(): void {
    this.ejecutar('getRecetasAnuladas', this.recetasService.getAnuladas());
  }

  ejecutarGetRecetasPaged(): void {
    this.ejecutar(
      'getRecetasPaged',
      this.recetasService.getPaged(
        this.paramsRecetasPaged.pageNumber,
        this.paramsRecetasPaged.pageSize,
        this.paramsRecetasPaged.pacienteId,
        this.paramsRecetasPaged.diagnostico,
        this.paramsRecetasPaged.fechaDesde,
        this.paramsRecetasPaged.fechaHasta,
        this.paramsRecetasPaged.incluirAnuladas
      )
    );
  }

  imprimirRecetaConsultada(): void {
    this.imprimirReceta(this.paramGetRecetaId);
  }

  // =====================================================================
  // REPORTES / DASHBOARD
  // =====================================================================

  ejecutarGetDashboard(): void {
    this.ejecutar('getDashboard', this.reportesService.getDashboard(), 200, (data) => {
      this.resultadoDashboard = data;
    });
  }

  ejecutarReporte(tipo: 'pacientes' | 'citas' | 'recetas' | 'notificaciones'): void {
    const clave = `reporte${tipo.charAt(0).toUpperCase()}${tipo.slice(1)}`;
    const filtros = this.filtrosPara(tipo);
    let peticion: Observable<ReporteRow[]>;
    switch (tipo) {
      case 'pacientes':
        peticion = this.reportesService.getPacientes(filtros);
        break;
      case 'citas':
        peticion = this.reportesService.getCitas(filtros);
        break;
      case 'recetas':
        peticion = this.reportesService.getRecetas(filtros);
        break;
      case 'notificaciones':
        peticion = this.reportesService.getNotificaciones(filtros);
        break;
      default:
        return;
    }
    this.ejecutar(clave, peticion);
  }

  private filtrosPara(tipo: string): ReporteFiltros {
    switch (tipo) {
      case 'pacientes':
        return this.filtrosReportePacientes;
      case 'citas':
        return this.filtrosReporteCitas;
      case 'recetas':
        return this.filtrosReporteRecetas;
      case 'notificaciones':
        return this.filtrosReporteNotificaciones;
      default:
        return {};
    }
  }

  // =====================================================================
  // NOTIFICACIONES
  // =====================================================================

  ejecutarGetNotificaciones(): void {
    this.ejecutar('getNotificaciones', this.notificacionesService.getAll(this.paramFiltroEstadoNotif, this.paramFiltroCanalNotif), 200, (data) => {
      this.resultadoNotificaciones = data ?? [];
    });
  }

  ejecutarGetNotificacionById(): void {
    if (!this.paramGetNotifId) return;
    this.ejecutar('getNotificacionById', this.notificacionesService.getById(this.paramGetNotifId));
  }

  ejecutarGetNotificacionesPaged(): void {
    this.ejecutar(
      'getNotificacionesPaged',
      this.notificacionesService.getPaged(
        this.paramsNotifPaged.pageNumber,
        this.paramsNotifPaged.pageSize,
        this.paramsNotifPaged.estado,
        this.paramsNotifPaged.canal
      )
    );
  }

  ejecutarGetStats(): void {
    this.ejecutar('getStats', this.notificacionesService.getStats(), 200, (data) => {
      this.resultadoStats = data;
    });
  }

  ejecutarPostNotificacion(): void {
    this.ejecutar('postNotificacion', this.notificacionesService.create(this.bodyNotificacion), 201, () => {
      this.ejecutarGetNotificaciones();
    });
  }

  ejecutarPutNotificacion(): void {
    if (!this.paramPutNotifId) return;
    this.ejecutar('putNotificacion', this.notificacionesService.update(this.paramPutNotifId, this.bodyNotificacion), 204, () => {
      this.ejecutarGetNotificaciones();
    });
  }

  ejecutarDeleteNotificacion(): void {
    if (!this.paramDeleteNotifId) return;
    this.ejecutar('deleteNotificacion', this.notificacionesService.delete(this.paramDeleteNotifId), 204, () => {
      this.ejecutarGetNotificaciones();
    });
  }

  ejecutarReintentarNotificacion(): void {
    if (!this.paramReintentarNotifId) return;
    this.ejecutar('reintentarNotif', this.notificacionesService.reintentar(this.paramReintentarNotifId), 200, () => {
      this.ejecutarGetNotificaciones();
    });
  }

  // =====================================================================
  // PACIENTES — EXPERIENCIA VISUAL
  // =====================================================================

  get pacientesClinicos(): PacienteCardView[] {
    const citasPorPaciente = new Map<number, Cita[]>();
    const recetasPorPaciente = new Map<number, RecetaMedica[]>();

    this.citasDashboard.forEach((cita) => {
      const id = Number(cita.pacienteID ?? 0);
      if (!id) return;
      const lista = citasPorPaciente.get(id) ?? [];
      lista.push(cita);
      citasPorPaciente.set(id, lista);
    });

    this.recetasDashboard.forEach((receta) => {
      const id = Number(receta.pacienteID ?? receta['pacienteId'] ?? 0);
      if (!id) return;
      const lista = recetasPorPaciente.get(id) ?? [];
      lista.push(receta);
      recetasPorPaciente.set(id, lista);
    });

    return this.pacientesDashboard.map((paciente) => {
      const id = this.idPaciente(paciente);
      const citas = citasPorPaciente.get(id) ?? [];
      const recetas = recetasPorPaciente.get(id) ?? [];
      const ultima = [...citas].sort(
        (a, b) => (this.fechaDe(b.fechaHoraInicio)?.getTime() ?? 0) - (this.fechaDe(a.fechaHoraInicio)?.getTime() ?? 0)
      )[0];
      const estadoCita = ultima ? this.estadoDe(ultima) : null;
      const estado = !ultima
        ? 'Sin citas'
        : estadoCita === 'Concluida'
          ? 'Atendido'
          : estadoCita === 'Cancelada'
            ? 'Cancelada'
            : 'En seguimiento';
      const tono: Tono = !ultima
        ? 'slate'
        : estadoCita === 'Concluida'
          ? 'green'
          : estadoCita === 'Cancelada'
            ? 'slate'
            : estadoCita === 'PendienteResultados'
              ? 'amber'
              : 'cyan';
      const nombre = this.texto(paciente.nombreCompleto) || `Paciente #${id}`;

      return {
        id,
        codigo: this.texto(paciente.codigoPaciente) || `PAC-${String(id).padStart(4, '0')}`,
        nombre,
        documento: this.texto(paciente.documento ?? paciente['documentoIdentidad']) || 'Sin documento',
        telefono: this.texto(paciente.telefono) || 'Sin teléfono',
        correo: this.texto(paciente.correoElectronico) || 'Sin correo',
        edad: this.calcularEdad(paciente.fechaNacimiento),
        ultimaConsulta: ultima ? this.formatearFechaCorta(ultima.fechaHoraInicio) : 'Sin consultas',
        estado,
        tono,
        totalCitas: citas.length,
        totalRecetas: recetas.length,
        inicial: nombre.charAt(0).toUpperCase()
      };
    });
  }

  get pacientesFiltrados(): PacienteCardView[] {
    const busqueda = this.busquedaPacientes.trim().toLowerCase();
    const filtro = this.filtroEstadoPaciente;
    return this.pacientesClinicos.filter((paciente) => {
      if (filtro === 'con-citas' && paciente.totalCitas === 0) return false;
      if (filtro === 'sin-citas' && paciente.totalCitas > 0) return false;
      if (filtro === 'atendidos' && paciente.estado !== 'Atendido') return false;
      if (filtro === 'seguimiento' && paciente.estado !== 'En seguimiento') return false;
      if (!busqueda) return true;
      return [paciente.nombre, paciente.codigo, paciente.documento, paciente.telefono, paciente.correo]
        .join(' ')
        .toLowerCase()
        .includes(busqueda);
    });
  }

  inicialNombre(valor: string | null | undefined): string {
    return this.texto(valor).trim().charAt(0).toUpperCase() || 'P';
  }

  editarPacienteDesdeCard(pacienteID: number): void {
    const paciente = this.pacientesDashboard.find((item) => this.idPaciente(item) === Number(pacienteID));
    this.abrirPaciente(paciente);
  }

  // =====================================================================
  // CREAR CITA: PACIENTE NUEVO/EXISTENTE + NOTIFICACIÓN AUTOMÁTICA
  // =====================================================================

  get pacientesParaNuevaCita(): Paciente[] {
    return [...this.pacientesDashboard].sort((a, b) =>
      this.texto(a.nombreCompleto).localeCompare(this.texto(b.nombreCompleto))
    );
  }

  get medicosPorEspecialidad(): Array<{ id: number; nombre: string; especialidadID: number }> {
    const id = Number(this.nuevaCitaForm.especialidadID);
    const filtrados = this.medicosClinica.filter((medico) => medico.especialidadID === id);
    return filtrados.length ? filtrados : this.medicosClinica.slice(0, 1);
  }

  get destinatarioNuevaCita(): string {
    const canal = this.nuevaCitaForm.canalNotificacion;
    if (this.pacienteCitaForm.modo === 'nuevo') {
      return canal === 'Email'
        ? this.pacienteCitaForm.correoElectronico.trim()
        : this.pacienteCitaForm.telefono.trim();
    }
    const paciente = this.pacientesDashboard.find(
      (item) => this.idPaciente(item) === Number(this.pacienteCitaForm.pacienteID)
    );
    return canal === 'Email' ? this.texto(paciente?.correoElectronico) : this.texto(paciente?.telefono);
  }

  abrirNuevaCita(): void {
    if (!this.drawerNuevaCitaAbierta) this.registrarPaso();

    const hoy = new Date();
    this.pacienteCitaForm = {
      modo: 'existente',
      pacienteID: null,
      nombre: '',
      apellido: '',
      documento: '',
      fechaNacimiento: '',
      telefono: '',
      correoElectronico: ''
    };
    this.nuevaCitaForm = {
      especialidadID: 1,
      medicoID: 1,
      fecha: this.fechaInput(hoy),
      hora: '09:00',
      motivo: '',
      canalNotificacion: 'WhatsApp'
    };
    this.mensajeNuevaCita = '';
    this.resultadoNuevaCita = null;
    this.avisoPrincipal = '';
    this.drawerNuevaCitaAbierta = true;
    this.sidebarAbierta = false;
  }

  cerrarNuevaCita(): void {
    if (!this.drawerNuevaCitaAbierta) return;
    this.consumirPasoDrawer('nueva-cita');
    this.drawerNuevaCitaAbierta = false;
    this.mensajeNuevaCita = '';
    this.resultadoNuevaCita = null;
  }

  cambiarModoPaciente(modo: 'existente' | 'nuevo'): void {
    this.pacienteCitaForm.modo = modo;
    this.mensajeNuevaCita = '';
  }

  onEspecialidadNuevaCitaChange(): void {
    const primera = this.medicosPorEspecialidad[0];
    this.nuevaCitaForm.medicoID = primera?.id ?? 1;
  }

  crearCitaCompleta(): void {
    this.mensajeNuevaCita = '';
    this.resultadoNuevaCita = null;

    const paciente = this.pacienteCitaForm;
    const cita = this.nuevaCitaForm;

    if (!cita.fecha || !cita.hora) {
      this.mensajeNuevaCita = 'Seleccione la fecha y la hora de la cita.';
      return;
    }
    if (!cita.motivo.trim()) {
      this.mensajeNuevaCita = 'Indique el motivo de la cita.';
      return;
    }
    if (paciente.modo === 'existente' && !paciente.pacienteID) {
      this.mensajeNuevaCita = 'Seleccione un paciente existente.';
      return;
    }
    if (paciente.modo === 'nuevo') {
      if (!paciente.nombre.trim() || !paciente.apellido.trim() || !paciente.documento.trim()) {
        this.mensajeNuevaCita = 'Nombre, apellido y documento son obligatorios para el paciente nuevo.';
        return;
      }
      if (!paciente.fechaNacimiento) {
        this.mensajeNuevaCita = 'Indique la fecha de nacimiento del paciente nuevo.';
        return;
      }
    }

    const especialidad = this.especialidadesClinica.find((item) => item.id === Number(cita.especialidadID));
    const medico = this.medicosClinica.find((item) => item.id === Number(cita.medicoID)) ?? this.medicosClinica[0];
    const fechaHoraInicio = `${cita.fecha}T${cita.hora}:00`;
    const inicio = new Date(fechaHoraInicio);
    const fin = new Date(inicio.getTime() + 30 * 60000);
    const fechaHoraFin = `${cita.fecha}T${this.horaInput(fin)}:00`;
    const destinatario = this.destinatarioNuevaCita;
    const nombrePaciente =
      paciente.modo === 'nuevo'
        ? `${paciente.nombre.trim()} ${paciente.apellido.trim()}`.trim()
        : this.nombrePaciente(Number(paciente.pacienteID));

    const continuarConCita = (pacienteID: number, codigoPaciente: string): void => {
      const payloadCita: Partial<Cita> = {
        pacienteID,
        medicoID: Number(medico.id),
        especialidadID: Number(cita.especialidadID),
        consultorioID: 1,
        estadoCitaID: 1,
        fechaHoraInicio,
        fechaHoraFin,
        motivo: cita.motivo.trim(),
        nombreMedico: medico.nombre,
        especialidad: especialidad?.nombre ?? 'Medicina General'
      };

      this.citasService.create(payloadCita).subscribe({
        next: (respuesta) => {
          const citaID = Number((respuesta as unknown as { citaID?: number; CitaID?: number })?.citaID
            ?? (respuesta as unknown as { CitaID?: number })?.CitaID);
          this.registrarNotificacionCitaCreada(
            pacienteID,
            citaID,
            nombrePaciente,
            codigoPaciente,
            fechaHoraInicio,
            destinatario,
            cita.canalNotificacion,
            especialidad?.nombre ?? 'Medicina General'
          );
        },
        error: (err: { error?: { message?: string }; message?: string }) => {
          this.guardandoNuevaCita = false;
          this.mensajeNuevaCita = err?.error?.message || err?.message || 'No fue posible crear la cita.';
        }
      });
    };

    this.guardandoNuevaCita = true;

    if (paciente.modo === 'nuevo') {
      const payloadPaciente: Partial<Paciente> = {
        nombre: paciente.nombre.trim(),
        apellido: paciente.apellido.trim(),
        nombreCompleto: `${paciente.nombre.trim()} ${paciente.apellido.trim()}`.trim(),
        documentoIdentidad: paciente.documento.trim(),
        documento: paciente.documento.trim(),
        codigoPaciente: this.sugerirCodigoPaciente(),
        fechaNacimiento: paciente.fechaNacimiento,
        telefono: paciente.telefono.trim() || undefined,
        correoElectronico: paciente.correoElectronico.trim() || undefined
      };

      this.pacientesService.create(payloadPaciente).subscribe({
        next: (creado) => {
          const datos = creado as Paciente & { pacienteID?: number; id?: number };
          const pacienteID = Number(datos.pacienteID ?? datos.id);
          const codigo = this.texto(datos.codigoPaciente) || this.texto(payloadPaciente.codigoPaciente);
          continuarConCita(pacienteID, codigo);
        },
        error: (err: { error?: { message?: string }; message?: string }) => {
          this.guardandoNuevaCita = false;
          this.mensajeNuevaCita = err?.error?.message || err?.message || 'No fue posible crear el paciente.';
        }
      });
      return;
    }

    const existente = this.pacientesDashboard.find(
      (item) => this.idPaciente(item) === Number(paciente.pacienteID)
    );
    continuarConCita(
      Number(paciente.pacienteID),
      this.texto(existente?.codigoPaciente) || this.sugerirCodigoPaciente()
    );
  }

  private registrarNotificacionCitaCreada(
    pacienteID: number,
    citaID: number,
    nombrePaciente: string,
    codigoPaciente: string,
    fechaHoraInicio: string,
    destinatario: string,
    canal: string,
    especialidad: string
  ): void {
    if (!destinatario) {
      this.finalizarNuevaCita(
        citaID,
        fechaHoraInicio,
        nombrePaciente,
        especialidad,
        canal,
        '',
        false
      );
      return;
    }

    const mensaje = `Su cita fue creada para el ${this.formatearFechaCorta(fechaHoraInicio)} a las ${this.formatearHora(fechaHoraInicio)}. Código de paciente: ${codigoPaciente || '—'}.`;
    const payload: Partial<Notificacion> = {
      citaID,
      pacienteID,
      pacienteNombre: nombrePaciente,
      destinatario,
      canal,
      mensaje,
      fechaProgramadaCita: fechaHoraInicio,
      estado: 'Pendiente',
      intentos: 0
    };

    this.notificacionesService.create(payload).subscribe({
      next: () => this.finalizarNuevaCita(
        citaID,
        fechaHoraInicio,
        nombrePaciente,
        especialidad,
        canal,
        destinatario,
        true
      ),
      error: () => this.finalizarNuevaCita(
        citaID,
        fechaHoraInicio,
        nombrePaciente,
        especialidad,
        canal,
        destinatario,
        false
      )
    });
  }

  private finalizarNuevaCita(
    citaID: number,
    fechaHoraInicio: string,
    nombrePaciente: string,
    especialidad: string,
    canal: string,
    destinatario: string,
    notificacionRegistrada: boolean
  ): void {
    this.guardandoNuevaCita = false;

    const fechaHora = `${this.formatearFechaCorta(fechaHoraInicio)} a las ${this.formatearHora(fechaHoraInicio)}`;
    const detalle = notificacionRegistrada
      ? `Registro completado: ${nombrePaciente} · ${especialidad} · aviso por ${canal} para ${destinatario}.`
      : destinatario
        ? `Registro completado: ${nombrePaciente} · ${especialidad}. No se pudo registrar el aviso por ${canal}.`
        : `Registro completado: ${nombrePaciente} · ${especialidad}. Sin destinatario para el aviso por ${canal}.`;

    this.avisoPrincipal = `Cita #${citaID} creada para ${fechaHora}. ${detalle}`;
    this.cerrarNuevaCita();

    this.cargarDashboard();
    this.cargarNotificacionesClinicas();
    this.ejecutarGetPacientes();
  }

  private fechaInput(fecha: Date): string {
    return `${fecha.getFullYear()}-${String(fecha.getMonth() + 1).padStart(2, '0')}-${String(fecha.getDate()).padStart(2, '0')}`;
  }

  private horaInput(fecha: Date): string {
    return `${String(fecha.getHours()).padStart(2, '0')}:${String(fecha.getMinutes()).padStart(2, '0')}`;
  }

  abrirPaciente(paciente?: Paciente): void {
    if (!this.drawerPacienteAbierta) this.registrarPaso();

    const id = this.idPaciente(paciente ?? null);
    const completo = this.texto(paciente?.nombreCompleto).trim();
    const partes = completo.split(/\s+/).filter(Boolean);
    const nombreGuardado = this.texto(paciente?.['nombre']).trim();
    const apellidoGuardado = this.texto(paciente?.['apellido']).trim();
    const documento = this.texto(paciente?.documento ?? paciente?.['documentoIdentidad']).trim();

    this.pacienteEditando = {
      ...(paciente ?? {}),
      id: id || undefined,
      pacienteID: id || undefined,
      nombre: nombreGuardado || partes.slice(0, -1).join(' ') || completo,
      apellido: apellidoGuardado || (partes.length > 1 ? partes[partes.length - 1] : ''),
      nombreCompleto: completo,
      documento,
      documentoIdentidad: documento,
      codigoPaciente: this.texto(paciente?.codigoPaciente).trim() || this.sugerirCodigoPaciente(),
      fechaNacimiento: this.texto(paciente?.fechaNacimiento).slice(0, 10),
      telefono: this.texto(paciente?.telefono),
      correoElectronico: this.texto(paciente?.correoElectronico)
    };
    this.mensajePaciente = '';
    this.drawerPacienteAbierta = true;
  }

  guardarPacienteClinico(): void {
    const datos = this.pacienteEditando;
    const nombre = this.texto(datos.nombre).trim();
    const apellido = this.texto(datos.apellido).trim();
    const documento = this.texto(datos.documento ?? datos.documentoIdentidad).trim();

    if (!nombre || !apellido || !documento) {
      this.mensajePaciente = 'Nombre, apellido y documento de identidad son obligatorios.';
      return;
    }

    const id = this.idPaciente(datos as Paciente);
    const payload: Partial<Paciente> = {
      nombre,
      apellido,
      nombreCompleto: `${nombre} ${apellido}`.trim(),
      documentoIdentidad: documento,
      documento,
      codigoPaciente: this.texto(datos.codigoPaciente).trim() || this.sugerirCodigoPaciente(),
      fechaNacimiento: this.texto(datos.fechaNacimiento) || '1900-01-01',
      telefono: this.texto(datos.telefono).trim() || undefined,
      correoElectronico: this.texto(datos.correoElectronico).trim() || undefined
    };

    this.guardandoPaciente = true;
    const peticion: Observable<Paciente | void> = id
      ? this.pacientesService.update(id, payload)
      : this.pacientesService.create(payload);

    peticion.subscribe({
      next: () => {
        this.guardandoPaciente = false;
        this.cerrarDrawerPaciente();
        this.mensajePaciente = id ? 'Paciente actualizado correctamente.' : 'Paciente creado correctamente.';
        this.cargarDashboard();
        this.ejecutarGetPacientes();
      },
      error: (err: { error?: { message?: string }; message?: string }) => {
        this.guardandoPaciente = false;
        this.mensajePaciente = err?.error?.message || err?.message || 'No fue posible guardar el paciente.';
      }
    });
  }

  eliminarPacienteClinico(): void {
    const id = this.idPaciente(this.pacienteEditando as Paciente);
    if (!id || !window.confirm('¿Desea eliminar este paciente? Se conservará su historial clínico.')) return;

    this.pacientesService.delete(id).subscribe({
      next: () => {
        this.cerrarDrawerPaciente();
        this.mensajePaciente = 'Paciente eliminado correctamente.';
        this.cargarDashboard();
        this.ejecutarGetPacientes();
      },
      error: () => {
        this.mensajePaciente = 'No fue posible eliminar el paciente.';
      }
    });
  }

  cerrarDrawerPaciente(): void {
    if (!this.drawerPacienteAbierta) return;
    this.consumirPasoDrawer('paciente');
    this.drawerPacienteAbierta = false;
    this.pacienteEditando = {};
  }

  verHistorialPaciente(pacienteID: number): void {
    this.registrarPaso();
    this.pacienteHistorialSeleccionado = Number(pacienteID);
    this.paramHistorialPacienteId = Number(pacienteID);
    this.drawerPacienteAbierta = false;
    this.irAVista('historial');
    this.cargarHistorialClinico();
  }

  verRecetasPaciente(pacienteID: number): void {
    this.registrarPaso();
    this.pacienteRecetasFiltro = Number(pacienteID);
    this.drawerPacienteAbierta = false;
    this.irAVista('recetas');
    this.cargarRecetas();
  }

  filtrarAgendaPorPaciente(pacienteID: number): void {
    this.registrarPaso();
    this.busquedaAgenda = this.nombrePaciente(Number(pacienteID));
    this.filtroMedicoAgenda = '';
    this.drawerPacienteAbierta = false;
    this.irAVista('agenda');
  }

  // =====================================================================
  // HISTORIAL CLÍNICO — TIMELINE
  // =====================================================================

  get pacienteHistorialActual(): Paciente | null {
    if (!this.pacienteHistorialSeleccionado) return null;
    return this.pacientesDashboard.find(
      (paciente) => this.idPaciente(paciente) === Number(this.pacienteHistorialSeleccionado)
    ) ?? null;
  }

  get tiposHistorial(): string[] {
    const valores = this.tiposEventoHistorial.slice();
    this.historialEventos.forEach((evento) => {
      const tipo = this.texto(evento.tipoEvento ?? evento.tipo).trim();
      if (tipo && !valores.includes(tipo)) valores.push(tipo);
    });
    return valores;
  }

  get eventosHistorialVisibles(): TimelineEventView[] {
    const busqueda = this.busquedaHistorial.trim().toLowerCase();
    return this.historialEventos
      .filter((evento) => {
        const tipo = this.texto(evento.tipoEvento ?? evento.tipo);
        if (this.filtroTipoHistorial && tipo !== this.filtroTipoHistorial) return false;
        if (!busqueda) return true;
        return [tipo, this.texto(evento.descripcion), this.texto(evento['diagnostico']), this.texto(evento['tratamiento'])]
          .join(' ')
          .toLowerCase()
          .includes(busqueda);
      })
      .map((evento) => this.mapearEventoHistorial(evento))
      .sort((a, b) => (this.fechaDe(b.fecha)?.getTime() ?? 0) - (this.fechaDe(a.fecha)?.getTime() ?? 0));
  }

  seleccionarPacienteHistorial(pacienteID: number | null): void {
    if (Number(pacienteID ?? 0) !== Number(this.pacienteHistorialSeleccionado ?? 0)) {
      this.registrarPaso();
    }
    this.pacienteHistorialSeleccionado = pacienteID ? Number(pacienteID) : null;
    this.paramHistorialPacienteId = this.pacienteHistorialSeleccionado ?? 0;
    this.busquedaHistorial = '';
    this.filtroTipoHistorial = '';
    this.mensajeHistorial = '';
    if (this.pacienteHistorialSeleccionado) {
      this.cargarHistorialClinico();
    } else {
      this.historialEventos = [];
    }
  }

  cargarHistorialClinico(): void {
    const id = Number(this.pacienteHistorialSeleccionado ?? this.paramHistorialPacienteId ?? 0);
    if (!id) return;
    this.historialCargando = true;
    this.pacientesService.getHistorial(id).subscribe({
      next: (data) => {
        this.historialEventos = this.extraerData<HistorialEvento>(data);
        this.historialCargando = false;
      },
      error: () => {
        this.historialEventos = [];
        this.historialCargando = false;
        this.mensajeHistorial = 'No fue posible cargar el historial clínico.';
      }
    });
  }

  registrarEventoHistorialClinico(): void {
    const id = Number(this.pacienteHistorialSeleccionado ?? 0);
    if (!id) {
      this.mensajeHistorial = 'Seleccione un paciente antes de registrar un evento.';
      return;
    }
    const tipoEvento = this.nuevoEventoHistorial.tipoEvento.trim();
    const descripcion = this.nuevoEventoHistorial.descripcion.trim();
    if (!tipoEvento || !descripcion) {
      this.mensajeHistorial = 'Seleccione el tipo de evento y escriba una descripción.';
      return;
    }

    const evento: Partial<HistorialEvento> = {
      pacienteID: id,
      tipoEvento,
      descripcion,
      eventKey: `manual-${id}-${Date.now()}`,
      fechaEvento: this.nuevoEventoHistorial.fechaEvento || new Date().toISOString().slice(0, 16),
      citaID: this.nuevoEventoHistorial.citaID ?? undefined
    };

    this.pacientesService.addHistorialEvento(id, evento).subscribe({
      next: () => {
        this.nuevoEventoHistorial.descripcion = '';
        this.nuevoEventoHistorial.citaID = null;
        this.mensajeHistorial = 'Evento registrado en el historial clínico.';
        this.cargarHistorialClinico();
      },
      error: () => {
        this.mensajeHistorial = 'No fue posible registrar el evento.';
      }
    });
  }

  abrirCitaDesdeHistorial(citaID?: number): void {
    const id = Number(citaID ?? 0);
    if (!id) return;
    const cita = this.citasDashboard.find((item) => Number(item.citaID ?? 0) === id);
    if (cita) this.abrirCita(cita);
  }

  // =====================================================================
  // RECETAS — VISTA CLÍNICA
  // =====================================================================

  get recetasFiltradas(): RecetaCardView[] {
    const busqueda = this.busquedaRecetas.trim().toLowerCase();
    return this.recetasClinicas
      .filter((receta) => {
        if (!this.incluirAnuladasRecetas && recetaAnulada(receta)) return false;
        if (this.pacienteRecetasFiltro) {
          const id = Number(receta.pacienteID ?? receta['pacienteId'] ?? 0);
          if (id !== Number(this.pacienteRecetasFiltro)) return false;
        }
        if (!busqueda) return true;
        const paciente = this.texto(receta['nombrePaciente']) || this.nombrePaciente(Number(receta.pacienteID ?? 0));
        return [paciente, this.texto(receta.diagnostico), this.texto(receta.tratamiento), this.texto(receta['citaID'])]
          .join(' ')
          .toLowerCase()
          .includes(busqueda);
      })
      .map((receta) => this.mapearReceta(receta))
      .sort((a, b) => (this.fechaDe(b.fecha)?.getTime() ?? 0) - (this.fechaDe(a.fecha)?.getTime() ?? 0));
  }

  get pacientesConRecetas(): Paciente[] {
    const ids = new Set<number>();
    [...this.recetasClinicas, ...this.recetasDashboard].forEach((receta) => {
      const id = Number(receta.pacienteID ?? receta['pacienteId'] ?? 0);
      if (id) ids.add(id);
    });
    return this.pacientesDashboard.filter((paciente) => ids.has(this.idPaciente(paciente)));
  }

  get citasConcluidasDisponibles(): Cita[] {
    const conRecetaVigente = new Set<number>();
    this.recetasClinicas.forEach((receta) => {
      if (!recetaAnulada(receta)) {
        const id = Number(citaIdReceta(receta) ?? 0);
        if (id) conRecetaVigente.add(id);
      }
    });
    return this.citasDashboard.filter((cita) => {
      const id = Number(cita.citaID ?? 0);
      return this.estadoDe(cita) === 'Concluida' && id > 0 && !conRecetaVigente.has(id);
    });
  }

  prepararAnulacionReceta(id: number): void {
    this.recetaAnulandoId = Number(id);
    this.motivoAnulacion = '';
  }

  cancelarAnulacionReceta(): void {
    this.recetaAnulandoId = null;
    this.motivoAnulacion = '';
  }

  anularRecetaClinica(): void {
    const id = Number(this.recetaAnulandoId ?? 0);
    const motivo = this.motivoAnulacion.trim();
    if (!id || !motivo) {
      this.mensajeRecetas = 'Seleccione una receta e indique el motivo de la anulación.';
      return;
    }

    this.recetasService.anular(id, motivo).subscribe({
      next: () => {
        this.mensajeRecetas = `Receta #${id} anulada correctamente.`;
        this.cancelarAnulacionReceta();
        if (idReceta(this.recetaActivaCita) === id) {
          this.recetaActivaCita = null;
          this.recetaActivaConsultada = false;
        }
        this.cargarRecetas();
        this.cargarDashboard();
      },
      error: () => {
        this.mensajeRecetas = 'No fue posible anular la receta.';
      }
    });
  }

  verRecetaCard(receta: RecetaCardView): void {
    if (receta.anulada) return;
    const url = this.recetasService.getUrlImprimible(receta.id);
    window.open(url, '_blank');
  }

  imprimirRecetaCard(receta: RecetaCardView): void {
    if (receta.anulada) return;
    this.imprimirReceta(receta.id);
  }

  emitirRecetaDesdeModulo(): void {
    const citaID = Number(this.recetaEmissionCitaID ?? 0);
    const cita = this.citasConcluidasDisponibles.find((item) => Number(item.citaID ?? 0) === citaID);
    if (!cita) {
      this.mensajeRecetas = 'Seleccione una cita concluida sin receta vigente.';
      return;
    }

    this.paramPutId = citaID;
    this.estadoActualSeleccionado = 'Concluida';
    this.recetaActivaCita = null;
    this.recetaActivaConsultada = true;
    this.mensajeRecetas = '';
    this.ejecutarEmitirReceta();
  }

  // =====================================================================
  // CENTRO DE NOTIFICACIONES
  // =====================================================================

  get notificacionesFiltradas(): NotificacionCardView[] {
    const busqueda = this.busquedaNotificaciones.trim().toLowerCase();
    return this.notificacionesDashboard
      .filter((notificacion) => {
        const tipo = this.texto(notificacion.tipoNotificacion ?? notificacion.tipo);
        if (this.filtroTipoNotificacion && tipo !== this.filtroTipoNotificacion) return false;
        if (this.filtroEstadoNotificacion && notificacion.estado !== this.filtroEstadoNotificacion) return false;
        if (this.filtroCanalNotificacion && notificacion.canal !== this.filtroCanalNotificacion) return false;
        if (!busqueda) return true;
        return [this.texto(notificacion.pacienteNombre), notificacion.destinatario, notificacion.mensaje, tipo, notificacion.estado]
          .join(' ')
          .toLowerCase()
          .includes(busqueda);
      })
      .map((notificacion) => this.mapearNotificacion(notificacion))
      .sort((a, b) => (this.fechaDe(b.fecha)?.getTime() ?? 0) - (this.fechaDe(a.fecha)?.getTime() ?? 0));
  }

  get tiposNotificacionDisponibles(): string[] {
    const valores = [...this.tiposNotificacion];
    this.notificacionesDashboard.forEach((notificacion) => {
      const tipo = this.texto(notificacion.tipoNotificacion ?? notificacion.tipo).trim();
      if (tipo && !valores.includes(tipo)) valores.push(tipo);
    });
    return valores;
  }

  get notificacionesPendientes(): number {
    return this.numero(this.statsDashboard?.pendientes)
      || this.notificacionesDashboard.filter((item) => item.estado === 'Pendiente').length;
  }

  get notificacionesFallidas(): number {
    return this.numero(this.statsDashboard?.fallidas)
      || this.notificacionesDashboard.filter((item) => item.estado === 'Fallido').length;
  }

  cargarNotificacionesClinicas(): void {
    forkJoin({
      lista: this.notificacionesService.getAll().pipe(catchError(() => of([] as Notificacion[]))),
      stats: this.notificacionesService.getStats().pipe(catchError(() => of(null)))
    }).subscribe({
      next: (data) => {
        this.notificacionesDashboard = this.extraerData<Notificacion>(data.lista);
        if (data.stats) this.statsDashboard = data.stats;
      },
      error: () => {
        this.notificacionesDashboard = [];
      }
    });
  }

  reintentarNotificacionClinica(id: number): void {
    this.notificacionReintentandoId = Number(id);
    this.notificacionesService.reintentar(Number(id)).subscribe({
      next: () => {
        this.notificacionReintentandoId = null;
        this.mensajeNotificaciones = `Reintento enviado para la notificación #${id}.`;
        this.cargarNotificacionesClinicas();
        this.cargarDashboard();
      },
      error: () => {
        this.notificacionReintentandoId = null;
        this.mensajeNotificaciones = 'No fue posible reintentar el envío.';
      }
    });
  }

  // =====================================================================
  // REPORTES — RESÚMENES EJECUTIVOS
  // =====================================================================

  get reporteFilas(): ReporteRow[] {
    return this.reportes[this.reporteActivo] ?? [];
  }

  get reporteTitulo(): string {
    const titulos: Record<TipoReporte, string> = {
      pacientes: 'Pacientes registrados',
      citas: 'Citas médicas',
      recetas: 'Recetas emitidas',
      notificaciones: 'Notificaciones enviadas'
    };
    return titulos[this.reporteActivo];
  }

  get reporteTituloDistribucion(): string {
    const titulos: Record<TipoReporte, string> = {
      pacientes: 'Pacientes por inicial',
      citas: 'Citas por estado',
      recetas: 'Recetas por diagnóstico',
      notificaciones: 'Notificaciones por canal'
    };
    return titulos[this.reporteActivo];
  }

  get reporteSubtituloDetalle(): string {
    const subtitulos: Record<TipoReporte, string> = {
      pacientes: 'Una fila por paciente registrado',
      citas: 'Una fila por cita agendada',
      recetas: 'Una fila por receta emitida',
      notificaciones: 'Una fila por notificación registrada'
    };
    return subtitulos[this.reporteActivo];
  }

  get columnasReporte(): ColumnaReporteView[] {
    switch (this.reporteActivo) {
      case 'pacientes':
        return [
          { clave: 'PacienteID', etiqueta: 'ID' },
          { clave: 'NombreCompleto', etiqueta: 'Paciente' },
          { clave: 'DocumentoIdentidad', etiqueta: 'Documento' },
          { clave: 'FechaNacimiento', etiqueta: 'Nacimiento' }
        ];
      case 'citas':
        return [
          { clave: 'CitaID', etiqueta: 'Cita' },
          { clave: 'PacienteID', etiqueta: 'Paciente' },
          { clave: 'Estado', etiqueta: 'Estado' },
          { clave: 'FechaHoraInicio', etiqueta: 'Fecha' },
          { clave: 'NombreMedico', etiqueta: 'Médico' },
          { clave: 'Especialidad', etiqueta: 'Especialidad' }
        ];
      case 'recetas':
        return [
          { clave: 'RecetaMedicaID', etiqueta: 'Receta' },
          { clave: 'CitaID', etiqueta: 'Cita' },
          { clave: 'NombrePaciente', etiqueta: 'Paciente' },
          { clave: 'Diagnostico', etiqueta: 'Diagnóstico' },
          { clave: 'FechaEmision', etiqueta: 'Fecha' },
          { clave: 'EsAnulada', etiqueta: 'Estado' }
        ];
      case 'notificaciones':
        return [
          { clave: 'NotificacionID', etiqueta: 'ID' },
          { clave: 'CitaID', etiqueta: 'Cita' },
          { clave: 'PacienteNombre', etiqueta: 'Paciente' },
          { clave: 'Tipo', etiqueta: 'Tipo' },
          { clave: 'Estado', etiqueta: 'Estado' },
          { clave: 'Canal', etiqueta: 'Canal' },
          { clave: 'FechaRegistro', etiqueta: 'Fecha' }
        ];
    }
  }

  get reporteCategorias(): ReporteCategoriaView[] {
    const campo = this.reporteActivo === 'pacientes'
      ? 'NombreCompleto'
      : this.reporteActivo === 'citas'
        ? 'Estado'
        : this.reporteActivo === 'recetas'
          ? 'Diagnostico'
          : 'Canal';
    const conteo = new Map<string, number>();

    this.reporteFilas.forEach((fila) => {
      const valor = this.valorFila(fila, campo);
      const etiqueta = this.reporteActivo === 'pacientes'
        ? valor.charAt(0).toUpperCase() || '—'
        : this.reporteActivo === 'citas'
          ? this.etiquetaEstadoReporte(valor)
          : valor;
      conteo.set(etiqueta, (conteo.get(etiqueta) ?? 0) + 1);
    });

    const total = this.reporteFilas.length || 1;
    const colores = ['#2563eb', '#0ea5e9', '#16a34a', '#f59e0b', '#7c3aed', '#64748b'];
    return Array.from(conteo.entries())
      .sort((a, b) => b[1] - a[1])
      .slice(0, 6)
      .map(([etiqueta, cantidad], indice) => ({
        etiqueta,
        cantidad,
        porcentaje: Math.round((cantidad / total) * 100),
        color: colores[indice % colores.length]
      }));
  }

  get reporteKpis(): KpiView[] {
    const total = this.reporteFilas.length;
    switch (this.reporteActivo) {
      case 'pacientes':
        return [
          { etiqueta: 'Registros', valor: total, detalle: 'En el periodo', icono: '◎', tono: 'blue' },
          { etiqueta: 'Con documento', valor: this.contarConValor('DocumentoIdentidad'), detalle: 'Identificación válida', icono: '✓', tono: 'green' },
          { etiqueta: 'Con teléfono', valor: this.contarConValor('Telefono'), detalle: 'Datos de contacto', icono: '☎', tono: 'cyan' },
          { etiqueta: 'Con correo', valor: this.contarConValor('CorreoElectronico'), detalle: 'Datos de contacto', icono: '✉', tono: 'violet' }
        ];
      case 'citas':
        return [
          { etiqueta: 'Citas', valor: total, detalle: 'En el periodo', icono: '▦', tono: 'blue' },
          { etiqueta: 'Concluidas', valor: this.contarCampo('Estado', 'Concluida'), detalle: 'Altas clínicas', icono: '✓', tono: 'green' },
          { etiqueta: 'En seguimiento', valor: this.contarCampo('Estado', 'PendienteResultados'), detalle: 'Pendiente de resultados', icono: '◌', tono: 'amber' },
          { etiqueta: 'Canceladas', valor: this.contarCampo('Estado', 'Cancelada'), detalle: 'Registro histórico', icono: '×', tono: 'slate' }
        ];
      case 'recetas':
        return [
          { etiqueta: 'Recetas', valor: total, detalle: 'En el periodo', icono: '℞', tono: 'violet' },
          { etiqueta: 'Vigentes', valor: this.contarCampo('EsAnulada', 'No'), detalle: 'Disponibles para impresión', icono: '✓', tono: 'green' },
          { etiqueta: 'Anuladas', valor: this.contarCampo('EsAnulada', 'Sí'), detalle: 'Anulación lógica', icono: '×', tono: 'slate' },
          { etiqueta: 'Con diagnóstico', valor: this.contarConValor('Diagnostico'), detalle: 'Información clínica', icono: '✚', tono: 'cyan' }
        ];
      case 'notificaciones':
        return [
          { etiqueta: 'Notificaciones', valor: total, detalle: 'En el periodo', icono: '✉', tono: 'violet' },
          { etiqueta: 'Enviadas', valor: this.contarCampo('Estado', 'Enviado'), detalle: 'Despacho correcto', icono: '✓', tono: 'green' },
          { etiqueta: 'Pendientes', valor: this.contarCampo('Estado', 'Pendiente'), detalle: 'En cola de envío', icono: '◌', tono: 'amber' },
          { etiqueta: 'Fallidas', valor: this.contarCampo('Estado', 'Fallido'), detalle: 'Requieren reintento', icono: '×', tono: 'slate' }
        ];
    }
  }

  seleccionarReporte(tipo: TipoReporte): void {
    if (tipo !== this.reporteActivo) this.registrarPaso();
    this.reporteActivo = tipo;
    this.mensajeReportes = '';
    this.cargarReporteActivo();
  }

  cargarReporteActivo(): void {
    this.cargarReporte(this.reporteActivo);
  }

  cargarReporte(tipo: TipoReporte): void {
    this.reportesCargando = true;
    this.mensajeReportes = '';
    const filtros: ReporteFiltros & Record<string, unknown> = {
      pageNumber: 1,
      pageSize: 50,
      FechaDesde: this.reporteDesde ? `${this.reporteDesde}T00:00:00` : undefined,
      FechaHasta: this.reporteHasta ? `${this.reporteHasta}T23:59:59` : undefined,
      estado: this.reporteEstado || undefined,
      canal: this.reporteCanal || undefined
    };

    let peticion: Observable<unknown>;
    switch (tipo) {
      case 'pacientes':
        peticion = this.reportesService.getPacientes(filtros);
        break;
      case 'citas':
        peticion = this.reportesService.getCitas(filtros);
        break;
      case 'recetas':
        peticion = this.reportesService.getRecetas(filtros);
        break;
      case 'notificaciones':
        peticion = this.reportesService.getNotificaciones(filtros);
        break;
    }

    peticion.subscribe({
      next: (data) => {
        this.reportes[tipo] = this.extraerData<ReporteRow>(data);
        this.reportesCargando = false;
      },
      error: () => {
        this.reportes[tipo] = [];
        this.reportesCargando = false;
        this.mensajeReportes = 'No fue posible generar el reporte con los filtros seleccionados.';
      }
    });
  }

  valorFila(fila: ReporteRow, clave: string): string {
    const partes = clave.split('.');
    let valor: unknown = fila;
    partes.forEach((parte) => {
      if (valor === null || valor === undefined || typeof valor !== 'object') return;
      const objeto = valor as Record<string, unknown>;
      const enMinuscula = parte.charAt(0).toLowerCase() + parte.slice(1);
      valor = objeto[parte] ?? objeto[this.capitalizar(parte)] ?? objeto[enMinuscula];
    });

    if (clave === 'PacienteID' && this.reporteActivo === 'citas') {
      const id = Number(valor);
      if (Number.isFinite(id) && id > 0) return this.nombrePaciente(id);
    }

    if (valor === null || valor === undefined || valor === '') return '—';
    if (typeof valor === 'boolean') return valor ? 'Sí' : 'No';
    if (Array.isArray(valor)) return valor.map((item) => this.texto(item)).join(', ');
    if (typeof valor === 'object') {
      const objeto = valor as Record<string, unknown>;
      return this.texto(objeto['nombre'] ?? objeto['nombreEstado'] ?? objeto['descripcion'] ?? objeto['value'] ?? valor);
    }

    const texto = this.texto(valor);
    if (/^\d{4}-\d{2}-\d{2}([T\s]|$)/.test(texto)) return this.formatearFechaCorta(texto);
    return texto;
  }

  exportarReporteCsv(): void {
    const filas = this.reporteFilas;
    if (!filas.length) return;
    const escapar = (valor: string) => `"${valor.replace(/"/g, '""')}"`;
    const cabeceras = this.columnasReporte.map((columna) => escapar(columna.etiqueta)).join(',');
    const cuerpo = filas.map((fila) =>
      this.columnasReporte.map((columna) => escapar(this.valorFila(fila, columna.clave))).join(',')
    );
    const contenido = `\uFEFF${[cabeceras, ...cuerpo].join('\r\n')}`;
    const url = URL.createObjectURL(new Blob([contenido], { type: 'text/csv;charset=utf-8;' }));
    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.download = `reporte-${this.reporteActivo}.csv`;
    enlace.click();
    URL.revokeObjectURL(url);
  }

  // =====================================================================
  // HELPERS DE PRESENTACIÓN (sin tocar contratos de API)
  // =====================================================================

  private calcularEdad(fechaNacimiento: string | null | undefined): number | null {
    const fecha = this.fechaDe(fechaNacimiento);
    if (!fecha) return null;
    const hoy = new Date();
    let edad = hoy.getFullYear() - fecha.getFullYear();
    const mes = hoy.getMonth() - fecha.getMonth();
    if (mes < 0 || (mes === 0 && hoy.getDate() < fecha.getDate())) edad--;
    return edad >= 0 && edad < 130 ? edad : null;
  }

  private sugerirCodigoPaciente(): string {
    return `PAC-${String(this.pacientesDashboard.length + 1).padStart(4, '0')}`;
  }

  private mapearEventoHistorial(evento: HistorialEvento): TimelineEventView {
    const tipo = this.texto(evento.tipoEvento ?? evento.tipo) || 'Evento clínico';
    const normalizado = tipo.toLowerCase();
    const diagnostico = this.texto(evento['diagnostico']);
    const tratamiento = this.texto(evento['tratamiento']);
    const motivo = this.texto(evento['motivoReprogramacion']);
    const fechaAnterior = this.texto(evento['fechaAnterior']);
    const fechaNueva = this.texto(evento['fechaNueva']);
    const extras = [
      diagnostico ? `Diagnóstico: ${diagnostico}` : '',
      tratamiento ? `Tratamiento: ${tratamiento}` : '',
      fechaAnterior && fechaNueva ? `Reprogramado: ${this.formatearFechaCorta(fechaAnterior)} → ${this.formatearFechaCorta(fechaNueva)}` : '',
      motivo ? `Motivo: ${motivo}` : ''
    ].filter(Boolean);
    const descripcion = [this.texto(evento.descripcion ?? evento.observaciones), extras.join(' · ')].filter(Boolean).join(' · ') || 'Sin descripción';

    let icono = '•';
    let tono: Tono = 'slate';
    if (normalizado.includes('receta')) { icono = '℞'; tono = 'violet'; }
    else if (normalizado.includes('reprogram')) { icono = '◷'; tono = 'amber'; }
    else if (normalizado.includes('atencion') || normalizado.includes('consulta')) { icono = '✚'; tono = 'cyan'; }
    else if (normalizado.includes('notificacion')) { icono = '✉'; tono = 'blue'; }
    else if (normalizado.includes('conclui') || normalizado.includes('alta')) { icono = '✓'; tono = 'green'; }
    else if (normalizado.includes('cancel')) { icono = '×'; tono = 'slate'; }

    return {
      id: this.texto(evento['eventKey'] ?? evento.id ?? evento.historialID ?? evento.eventoID ?? `${tipo}-${evento.fechaEvento ?? ''}`),
      fecha: this.texto(evento.fechaEvento ?? evento.fecha ?? evento['fechaRegistro']),
      titulo: tipo,
      descripcion,
      tipo,
      icono,
      tono,
      citaID: Number(evento.citaID ?? 0) || undefined,
      recetaID: Number(evento['recetaMedicaID'] ?? 0) || undefined
    };
  }

  private mapearReceta(receta: RecetaMedica): RecetaCardView {
    const id = Number(idReceta(receta) ?? 0);
    const citaID = Number(citaIdReceta(receta) ?? 0);
    const cita = this.citasDashboard.find((item) => Number(item.citaID ?? 0) === citaID);
    const pacienteID = Number(receta.pacienteID ?? receta['pacienteId'] ?? 0);
    return {
      id,
      citaID,
      paciente: this.texto(receta['nombrePaciente']) || this.nombrePaciente(pacienteID),
      medico: this.medicoVisible(cita),
      especialidad: cita?.especialidad || 'Especialidad no indicada',
      fecha: this.texto(receta.fechaEmision ?? receta.fechaCreacion),
      diagnostico: this.texto(receta.diagnostico) || 'Sin diagnóstico registrado',
      indicaciones: this.texto(receta.indicaciones) || 'Sin indicaciones registradas',
      tratamiento: this.texto(receta.tratamiento) || 'Sin tratamiento registrado',
      observaciones: this.texto(receta.observaciones),
      anulada: recetaAnulada(receta),
      motivoAnulacion: this.texto(receta['motivoAnulacion'] ?? receta['fechaAnulacion'])
    };
  }

  private mapearNotificacion(notificacion: Notificacion): NotificacionCardView {
    const tipo = this.texto(notificacion.tipoNotificacion ?? notificacion.tipo) || 'Recordatorio';
    const estado = this.texto(notificacion.estado) || 'Pendiente';
    const tono: Tono = estado === 'Enviado' ? 'green' : estado === 'Fallido' ? 'slate' : 'amber';
    const icono = estado === 'Enviado' ? '✓' : estado === 'Fallido' ? '!' : '◌';
    return {
      id: Number(notificacion.notificacionID ?? 0),
      tipo,
      titulo: `${tipo} · ${estado}`,
      estado,
      canal: this.texto(notificacion.canal) || 'Sin canal',
      paciente: this.texto(notificacion.pacienteNombre) || this.nombrePaciente(Number(notificacion.pacienteID ?? 0)),
      destinatario: this.texto(notificacion.destinatario),
      fecha: this.texto(notificacion.fechaEnvio ?? notificacion.fechaRegistro),
      mensaje: this.texto(notificacion.mensaje),
      intentos: this.numero(notificacion.intentos),
      tono,
      icono
    };
  }

  private extraerData<T>(valor: unknown): T[] {
    if (Array.isArray(valor)) return valor as T[];
    if (valor && typeof valor === 'object') {
      const envelope = valor as { data?: unknown; items?: unknown };
      if (Array.isArray(envelope.data)) return envelope.data as T[];
      if (Array.isArray(envelope.items)) return envelope.items as T[];
    }
    return [];
  }

  private capitalizar(valor: string): string {
    return valor ? valor.charAt(0).toUpperCase() + valor.slice(1) : valor;
  }

  private etiquetaEstadoReporte(valor: string): string {
    if (!valor || valor === '—') return 'Sin estado';
    const texto = valor.replace('EstadoCitaRead.', '').trim();
    const numerico = Number(texto);
    if (Number.isFinite(numerico) && numerico >= 1 && numerico <= 5) {
      return this.etiquetaEstado(estadoCitaFromId(numerico) ?? 'Agendada');
    }
    return this.etiquetaEstado(texto as EstadoCitaEnum);
  }

  private contarCampo(campo: string, valorEsperado: string): number {
    return this.reporteFilas.filter((fila) => this.valorFila(fila, campo).toLowerCase() === valorEsperado.toLowerCase()).length;
  }

  private contarConValor(campo: string): number {
    return this.reporteFilas.filter((fila) => {
      const valor = this.valorFila(fila, campo);
      return valor !== '—' && valor !== '';
    }).length;
  }

  private numero(valor: unknown): number {
    if (typeof valor === 'number' && Number.isFinite(valor)) return valor;
    if (typeof valor === 'string' && valor.trim()) {
      const numero = Number(valor);
      return Number.isFinite(numero) ? numero : 0;
    }
    return 0;
  }

  private texto(valor: unknown): string {
    return valor === null || valor === undefined ? '' : String(valor);
  }

  private fechaDe(valor: string | Date | null | undefined): Date | null {
    if (!valor) return null;
    const fecha = valor instanceof Date ? new Date(valor.getTime()) : new Date(valor);
    return Number.isNaN(fecha.getTime()) ? null : fecha;
  }

  private esHoy(valor: string | null | undefined): boolean {
    const fecha = this.fechaDe(valor);
    if (!fecha) return false;
    const hoy = new Date();
    return fecha.getFullYear() === hoy.getFullYear()
      && fecha.getMonth() === hoy.getMonth()
      && fecha.getDate() === hoy.getDate();
  }

  private claveDia(valor: string | Date | null | undefined): string {
    const fecha = this.fechaDe(valor);
    if (!fecha) return '';
    return `${fecha.getFullYear()}-${String(fecha.getMonth() + 1).padStart(2, '0')}-${String(fecha.getDate()).padStart(2, '0')}`;
  }

  private claveMes(valor: string | Date | null | undefined): string {
    const fecha = this.fechaDe(valor);
    if (!fecha) return '';
    return `${fecha.getFullYear()}-${String(fecha.getMonth() + 1).padStart(2, '0')}`;
  }

  formatearFechaCorta(valor: string | null | undefined): string {
    const fecha = this.fechaDe(valor);
    if (!fecha) return 'Sin fecha';
    return new Intl.DateTimeFormat('es-ES', { day: '2-digit', month: 'short' }).format(fecha).replace('.', '');
  }

  formatearHora(valor: string | null | undefined): string {
    const fecha = this.fechaDe(valor);
    if (!fecha) return '--:--';
    return new Intl.DateTimeFormat('es-ES', { hour: '2-digit', minute: '2-digit' }).format(fecha);
  }

  estadoDe(cita: Cita): EstadoCitaEnum {
    if (cita.estado && this.estados.some((estado) => estado.codigo === cita.estado)) {
      return cita.estado;
    }
    return estadoCitaFromId(cita.estadoCitaID) ?? 'Agendada';
  }

  etiquetaEstado(estado: EstadoCitaEnum): string {
    return ESTADOS_CITA.find((item) => item.codigo === estado)?.nombre ?? estado;
  }

  nombrePaciente(pacienteID: number): string {
    const paciente = this.pacientesDashboard.find((item) => this.idPaciente(item) === Number(pacienteID));
    return paciente?.nombreCompleto || `Paciente #${pacienteID}`;
  }

  private construirTendencia(cantidad: number, modo: 'dia' | 'mes'): TendenciaView[] {
    const base = new Date();
    const claves: Array<{ clave: string; etiqueta: string }> = [];

    if (modo === 'dia') {
      for (let i = cantidad - 1; i >= 0; i--) {
        const fecha = new Date(base.getFullYear(), base.getMonth(), base.getDate() - i);
        claves.push({
          clave: this.claveDia(fecha),
          etiqueta: new Intl.DateTimeFormat('es-ES', { weekday: 'short' }).format(fecha).replace('.', '')
        });
      }
    } else {
      for (let i = cantidad - 1; i >= 0; i--) {
        const fecha = new Date(base.getFullYear(), base.getMonth() - i, 1);
        claves.push({
          clave: this.claveMes(fecha),
          etiqueta: new Intl.DateTimeFormat('es-ES', { month: 'short' }).format(fecha).replace('.', '')
        });
      }
    }

    const datos = new Map<string, { citas: number; recetas: number }>();
    claves.forEach((item) => datos.set(item.clave, { citas: 0, recetas: 0 }));

    this.citasDashboard.forEach((cita) => {
      const clave = modo === 'dia' ? this.claveDia(cita.fechaHoraInicio) : this.claveMes(cita.fechaHoraInicio);
      const item = datos.get(clave);
      if (item) item.citas++;
    });

    this.recetasDashboard.forEach((receta) => {
      const fecha = receta.fechaEmision || receta.fechaCreacion;
      const clave = modo === 'dia' ? this.claveDia(fecha) : this.claveMes(fecha);
      const item = datos.get(clave);
      if (item) item.recetas++;
    });

    const maximo = Math.max(1, ...claves.map((item) => {
      const valor = datos.get(item.clave)!;
      return valor.citas + valor.recetas;
    }));

    return claves.map((item) => {
      const valor = datos.get(item.clave)!;
      const total = valor.citas + valor.recetas;
      return {
        etiqueta: item.etiqueta,
        citas: valor.citas,
        recetas: valor.recetas,
        total,
        altura: Math.round((total / maximo) * 100)
      };
    });
  }
}

export { AppComponent as App };
