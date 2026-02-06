// Formatea una fecha al formato YYYY-MM-DD
export const formatDate = (date: Date | null): string | null => {
  if (!date) return null;

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
};

// Formatea un texto al formato YYYY-MM-DD
export const formatDateForApi = (date: string | null): string | null => {
  if (!date) return null;

  if (typeof date === 'string' && date.match(/^\d{4}-\d{2}-\d{2}/)) {
    return date.split('T')[0];
  }

  return date;
};