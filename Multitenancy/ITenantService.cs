using System.Collections.Generic;

namespace OswaldTechnologies.Multitenancy
{
    public interface ITenantService
    {
        Tenant GetCurrentTenant();

        List<Tenant> GetTenantList();
    }
}