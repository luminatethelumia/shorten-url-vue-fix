using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using URLshorten.Data;
using URLshorten.Models;

namespace ApiController.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                
                // Remove existing DbContext registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<URLshortenContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }                

                // Add the in-memory database provider for testing.
                services.AddDbContext<URLshortenContext>(options =>
                    options.UseInMemoryDatabase("URLTestDB"));

                // Build the service provider and initialize the database.
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<URLshortenContext>();
                
                // ensure in-memory db is created
                db.Database.EnsureCreated();

                db.UrlShortenModel.Add(new UrlShortenModel
                {
                    Url = "https://www.youtube.com/watch?v=zaRM0iIhJvs",
                    ShortUrl = "https://localhost:3000/api/abc123",
                    QRImage = new byte[] {
                            0x42, 0x4D, 0x66, 0x11, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x28, 0x00,
                            0x00, 0x00, 0xE4, 0x02, 0x00, 0x00, 0xE4, 0x02, 0x00, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                        },
                    Code = "abc123",
                    CreatedAt = DateTime.Now,
                });

                db.SaveChanges();
            });
        }
    }

}
