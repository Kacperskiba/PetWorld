using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetWorld.Application.Interfaces;

namespace PetWorld.Infrastructure.Data;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly PetWorldDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(PetWorldDbContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var retryCount = 0;
        var maxRetries = 10;

        while (retryCount < maxRetries)
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                await ProductSeeder.SeedAsync(_context);
                _logger.LogInformation("Database initialized successfully");
                return;
            }
            catch (Exception ex) when (retryCount < maxRetries - 1)
            {
                retryCount++;
                _logger.LogWarning("Database connection failed (attempt {RetryCount}/{MaxRetries}): {Message}",
                    retryCount, maxRetries, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
