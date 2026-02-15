namespace Utilities.Static
{
    public enum States
    {
        Inactivo = 0,
        Activo = 1
    }

    public enum Movements
    {
        Cancelado = 0,
        Completado = 1
    }

    public enum Transfers
    {
        Cancelado = 0,
        Enviado = 1,
        Recibido = 2,
        Pendiente = 3
    }

    public enum Availability
    {
        Disponible = 1,
        No_Disponible = 2,
        Descontinuado = 3,
    }
}
