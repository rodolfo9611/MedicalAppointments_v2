/**
 * URLs centralizadas de todos los microservicios.
 * Ningún componente o servicio debe hardcodear una URL directamente.
 */
export const environment = {
  production: true,
  api: {
    citas: 'http://localhost:5000/api',
    notificaciones: 'http://localhost:5100/api',
    pacientes: 'http://localhost:5015/api',
    recetas: 'http://localhost:5200/api',
    reportes: 'http://localhost:5300/api'
  }
};
