/**
 * Configuración de desarrollo. angular.json sustituye environment.ts por este
 * archivo cuando se compila con la configuración "development".
 */
export const environment = {
  production: false,
  api: {
    citas: 'http://localhost:5000/api',
    notificaciones: 'http://localhost:5100/api',
    pacientes: 'http://localhost:5015/api',
    recetas: 'http://localhost:5200/api',
    reportes: 'http://localhost:5300/api'
  }
};
