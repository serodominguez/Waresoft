namespace Application.Security
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : Attribute
    {
        public string Module { get; }
        public string Action { get; }

        public RequirePermissionAttribute(string module, string action)
        {
            Module = module;
            Action = action;
        }
    }
}
