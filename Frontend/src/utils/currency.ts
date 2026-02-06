//Formatea la moneda al formato , .
export const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('es-BO', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
    useGrouping: true
  }).format(value).replace(/\./g, ',');
};