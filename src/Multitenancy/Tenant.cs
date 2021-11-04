namespace OswaldTechnologies.Multitenancy
{
    public record Tenant
    {
        public string Name { get; init; } = "";

        public string Hostname { get; init; } = "";

        public string ConnectionString { get; init; } = "";
    }
}