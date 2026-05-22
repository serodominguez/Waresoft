namespace Infrastructure.Persistences.ReadModels.Permission
{
    public record PermissionReadModel
    {
        public int Id { get; init; }
        public int IdRole { get; init; }
        public bool Status { get; init; }
        public int IdModule { get; init; }
        public string? ModuleName { get; init; }
        public bool StatusModule {  get; init; }
        public int IdAction { get; init; }
        public string? ActionName { get; init; }
        public bool StatusAction { get; init; }
    }
}
