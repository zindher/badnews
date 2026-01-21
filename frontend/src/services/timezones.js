/**
 * Configuración de zonas horarias de México
 * Mapeo directo: Estado → Zona Horaria
 */

export const MEXICO_TIMEZONES = {
  // Zona Centro (UTC-6)
  'CENTRO': {
    name: 'Centro (UTC-6)',
    offset: -6,
    states: ['Aguascalientes', 'Ciudad de México', 'Hidalgo', 'Morelos', 'Puebla', 'Querétaro', 'San Luis Potosí', 'Tamaulipas', 'Tlaxcala', 'Veracruz']
  },
  // Zona Montaña (UTC-7)
  'MONTANA': {
    name: 'Montaña (UTC-7)',
    offset: -7,
    states: ['Chihuahua Oeste', 'Durango']
  },
  // Zona Pacífico (UTC-7)
  'PACIFICO': {
    name: 'Pacífico (UTC-7)',
    offset: -7,
    states: ['Nayarit', 'Sinaloa', 'Jalisco', 'Michoacán', 'Guanajuato', 'Zacatecas', 'Coahuila Oeste', 'Chihuahua Este']
  },
  // Zona Noroeste (UTC-8)
  'NOROESTE': {
    name: 'Noroeste (UTC-8)',
    offset: -8,
    states: ['Baja California', 'Sonora']
  },
  // Zona Quintana Roo (UTC-5)
  'QUINTANA_ROO': {
    name: 'Quintana Roo (UTC-5)',
    offset: -5,
    states: ['Quintana Roo', 'Yucatán', 'Campeche', 'Tabasco']
  }
};

/**
 * Mapeo directo: Estado → Zona Horaria
 */
export const STATES_TO_TIMEZONE = {
  // Centro (UTC-6)
  'Aguascalientes': 'CENTRO',
  'Ciudad de México': 'CENTRO',
  'Hidalgo': 'CENTRO',
  'Morelos': 'CENTRO',
  'Puebla': 'CENTRO',
  'Querétaro': 'CENTRO',
  'San Luis Potosí': 'CENTRO',
  'Tamaulipas': 'CENTRO',
  'Tlaxcala': 'CENTRO',
  'Veracruz': 'CENTRO',
  'Oaxaca': 'CENTRO',
  'Nuevo León': 'CENTRO',

  // Montaña (UTC-7)
  'Chihuahua Oeste': 'MONTANA',
  'Durango': 'MONTANA',

  // Pacífico (UTC-7)
  'Nayarit': 'PACIFICO',
  'Sinaloa': 'PACIFICO',
  'Jalisco': 'PACIFICO',
  'Michoacán': 'PACIFICO',
  'Guanajuato': 'PACIFICO',
  'Zacatecas': 'PACIFICO',
  'Coahuila Oeste': 'PACIFICO',
  'Chihuahua Este': 'PACIFICO',
  'Guerrero': 'PACIFICO',
  'Colima': 'PACIFICO',

  // Noroeste (UTC-8)
  'Baja California': 'NOROESTE',
  'Sonora': 'NOROESTE',

  // Quintana Roo (UTC-5)
  'Quintana Roo': 'QUINTANA_ROO',
  'Yucatán': 'QUINTANA_ROO',
  'Campeche': 'QUINTANA_ROO',
  'Tabasco': 'QUINTANA_ROO'
};

/**
 * Lista de todos los estados de México (para el selector)
 */
export const MEXICAN_STATES = [
  'Aguascalientes',
  'Baja California',
  'Baja California Sur',
  'Campeche',
  'Chiapas',
  'Chihuahua',
  'Ciudad de México',
  'Coahuila',
  'Colima',
  'Durango',
  'Estado de México',
  'Guanajuato',
  'Guerrero',
  'Hidalgo',
  'Jalisco',
  'Michoacán',
  'Morelos',
  'Nayarit',
  'Nuevo León',
  'Oaxaca',
  'Puebla',
  'Querétaro',
  'Quintana Roo',
  'San Luis Potosí',
  'Sinaloa',
  'Sonora',
  'Tabasco',
  'Tamaulipas',
  'Tlaxcala',
  'Veracruz',
  'Yucatán',
  'Zacatecas'
];

/**
 * Obtiene la zona horaria basada en el estado
 * @param {string} state - Nombre del estado
 * @returns {string} - Clave de la zona horaria (CENTRO, MONTANA, PACIFICO, etc.)
 */
export function getTimezoneByState(state) {
  if (!state) return 'CENTRO'; // Default a Centro
  return STATES_TO_TIMEZONE[state] || 'CENTRO';
}

/**
 * Calcula la diferencia horaria entre Aguascalientes y la zona destino
 * @param {string} timezone - Clave de la zona horaria
 * @returns {number} - Diferencia en horas
 */
export function getTimezoneOffset(timezone) {
  const tz = MEXICO_TIMEZONES[timezone];
  if (!tz) return 0;
  
  // Aguascalientes está en UTC-6 (CENTRO)
  const aguascalientesOffset = -6;
  return tz.offset - aguascalientesOffset;
}

/**
 * Convierte una hora local (Aguascalientes) a hora en otra zona
 * @param {string} timeString - Hora en formato "HH:MM" (hora de Aguascalientes)
 * @param {string} timezone - Clave de la zona horaria destino
 * @returns {string} - Hora convertida en formato "HH:MM"
 */
export function convertTimeToTimezone(timeString, timezone) {
  const [hours, minutes] = timeString.split(':').map(Number);
  const offset = getTimezoneOffset(timezone);
  
  let newHours = hours + offset;
  let newDay = 0;
  
  // Manejar días anteriores/siguientes
  while (newHours < 0) {
    newHours += 24;
    newDay = -1;
  }
  while (newHours >= 24) {
    newHours -= 24;
    newDay = 1;
  }
  
  return {
    time: `${String(newHours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}`,
    day: newDay,
    dayLabel: newDay === -1 ? '(día anterior)' : newDay === 1 ? '(día siguiente)' : ''
  };
}

/**
 * Valida si un horario es válido (no después de las 21:00)
 * @param {string} timeString - Hora en formato "HH:MM"
 * @returns {boolean}
 */
export function isValidCallTime(timeString) {
  const [hours, minutes] = timeString.split(':').map(Number);
  const totalMinutes = hours * 60 + minutes;
  const maxTime = 21 * 60; // 21:00 (9 PM)
  
  return totalMinutes <= maxTime;
}

/**
 * Obtiene información de la zona horaria
 * @param {string} timezone - Clave de la zona horaria
 * @returns {object} - Información de la zona horaria
 */
export function getTimezoneInfo(timezone) {
  return MEXICO_TIMEZONES[timezone] || MEXICO_TIMEZONES['CENTRO'];
}
