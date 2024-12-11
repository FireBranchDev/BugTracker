using BackendClassLib.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendAPI.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    IConfiguration? _configuration;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        string projectDirectory = Directory.GetCurrentDirectory();

        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(c => c.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(dbContextDescriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_configuration?.GetConnectionString("DefaultConnection"));
            });
        });

        builder.UseEnvironment("Development");
    }
}
