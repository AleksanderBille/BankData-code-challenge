using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BankDataAPI.Models;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove EF Core-related registrations for AccountContext
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AccountContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                (d.ServiceType.Namespace != null && d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore"))
            ).ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            // Define InMemory database for AccountContext
            services.AddDbContext<AccountContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Auth override
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, options => { });
        });
    }
}