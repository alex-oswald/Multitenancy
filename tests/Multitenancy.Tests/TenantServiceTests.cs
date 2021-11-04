using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace OswaldTechnologies.Multitenancy.Tests
{
    public class TenantServiceTests
    {
        private readonly ILogger<TenantService> _logger;
        private readonly IHttpContextAccessor _httpContext;

        private readonly string _json = @"
            {
              ""Tenants"": [
                {
                  ""Name"": ""Localhost Tenant"",
                  ""Hostname"": ""localhost:44331"",
                  ""ConnectionString"": ""conn1""
                },
                {
                  ""Name"": ""Test Tenant"",
                  ""Hostname"": ""test.example.com"",
                  ""ConnectionString"": ""conn2""
                }
              ]
            }";

        private readonly Dictionary<string, string> _collection =
            new()
            {
                { "Tenants:0:Name", "Localhost Tenant" },
                { "Tenants:0:Hostname", "localhost:44331" },
                { "Tenants:0:ConnectionString", "conn1" },
                { "Tenants:1:Name", "Test Tenant" },
                { "Tenants:1:Hostname", "test.example.com" },
                { "Tenants:1:ConnectionString", "conn2" },
            };

        public TenantServiceTests()
        {
            _logger = new Mock<ILogger<TenantService>>().Object;
            _httpContext = new Mock<IHttpContextAccessor>().Object;
        }

        [Fact]
        public void Get_tenants_from_json_file()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(_json));

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var tenantService = new TenantService(_logger, _httpContext, config);

            var tenants = tenantService.GetTenantList();

            Assert.NotNull(tenants);
            Assert.Equal(2, tenants.Count);
            Assert.Equal("Localhost Tenant", tenants[0].Name);
            Assert.Equal("conn2", tenants[1].ConnectionString);
        }

        [Fact]
        public void Get_tenants_from_collection()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(_collection)
                .Build();

            var tenantService = new TenantService(_logger, _httpContext, config);

            var tenants = tenantService.GetTenantList();

            Assert.NotNull(tenants);
            Assert.Equal(2, tenants.Count);
            Assert.Equal("Localhost Tenant", tenants[0].Name);
            Assert.Equal("conn2", tenants[1].ConnectionString);
        }

        [Theory]
        [InlineData("localhost:44331", "Localhost Tenant")]
        [InlineData("test.example.com", "Test Tenant")]
        public void Get_current_tenants_name(string hostString, string tenantName)
        {
            var host = new HostString(hostString);

            var httpContext = new Mock<IHttpContextAccessor>();
            httpContext.SetupGet(x => x.HttpContext.Request.Host)
                .Returns(host);

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(_collection)
                .Build();

            var tenantService = new TenantService(_logger, httpContext.Object, config);

            var currentTenant = tenantService.GetCurrentTenant();

            Assert.Equal(tenantName, currentTenant.Name);
        }
    }
}