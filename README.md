# ASP.NET Core Multitenancy

[![Nuget](https://img.shields.io/nuget/v/OswaldTechnologies.Multitenancy)](https://www.nuget.org/packages/OswaldTechnologies.Multitenancy/)
![Nuget](https://img.shields.io/nuget/dt/OswaldTechnologies.Multitenancy)


This library lets you add multitenancy to an ASP.Core application. You add its services to the DI container.
It adds a `ITenantService` that provides two methods. `GetCurrentTenant` fetches the current tenant configuration
based on the current hostname. `GetTenantList` returns a list of all tenants in configuration. You are able to use
this service in other classes to change your database connection string.

## How to use

Install the library.

Add the tenant configuration to `appsettings.json`.

```json
{
    "Tenants": [
        {
            "Name": "Localhost Tenant",
            "Hostname": "localhost:44331",
            "ConnectionString": "conn1"
        },
        {
            "Name": "Test Tenant",
            "Hostname": "test.example.com",
            "ConnectionString": "conn2"
        }
    ]
}
```

Add the Multitenancy services to the dependency injection container.

```csharp
using Multitenancy;

public void ConfigureServices(IServiceCollection services)
{
    services.AddMultitenancy();
}
```

Inject into `DbContext` to specify the database connection string for the current tenant.


```csharp
public class ExampleContext : IdentityDbContext<ApplicationUser>
{
    private readonly ITenantService _tenantService;

    public ExampleContext(
        DbContextOptions<ExampleContext> options,
        ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            if (_tenantService == null)
            {
                throw new ArgumentNullException(nameof(_tenantService));
            }
            optionsBuilder.UseSqlServer(_tenantService.GetCurrentTenant().ConnectionString);
        }
    }
}
```
