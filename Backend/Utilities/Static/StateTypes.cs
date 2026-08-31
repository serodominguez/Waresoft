namespace Utilities.Static
{
    public enum Movements
    {
        Cancelado = 0,
        Completado = 1
    }

    public enum Periods
    {
        Cancelado = 0,
        Abierto = 1,
        Cerrado = 2
    }

    public enum Replenishment
    {
        Disponible = 1,
        No_Disponible = 2,
        Descontinuado = 3,
    }

    public enum States
    {
        Inactivo = 0,
        Activo = 1
    }

    public enum Transfers
    {
        Cancelado = 0,
        Enviado = 1,
        Recibido = 2,
        Pendiente = 3
    }
}
