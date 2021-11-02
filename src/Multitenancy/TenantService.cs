using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Multitenancy
{
    public class TenantService : ITenantService
    {
        private readonly ILogger<TenantService> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _configuration;

        public TenantService(
            ILogger<TenantService> logger,
            IHttpContextAccessor httpContext,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpContext = httpContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets the current tenant from the host.
        /// </summary>
        /// <returns>The tenant.</returns>
        public Tenant GetCurrentTenant()
        {
            Tenant tenant;
            var host = _httpContext.HttpContext.Request.Host;
            var tenants = GetTenantList();

            tenant = tenants.SingleOrDefault(t => t.Hostname == host.Value);
            if (tenant == null)
            {
                _logger.LogCritical("Could not find tenant from host: {host}", host);
                throw new ArgumentException($"Could not find tenant from host: {host}");
            }
            return tenant;
        }

        /// <summary>
        /// Gets a list of tenants in configuration.
        /// </summary>
        /// <returns>The list of tenants.</returns>
        public List<Tenant> GetTenantList()
        {
            var tenants = new List<Tenant>();

            _configuration.GetSection("Tenants").Bind(tenants);

            return tenants;
        }
    }
}