using System.Collections.Generic;

namespace Multitenancy
{
    public interface ITenantService
    {
        Tenant GetCurrentTenant();

        List<Tenant> GetTenantList();
    }
}