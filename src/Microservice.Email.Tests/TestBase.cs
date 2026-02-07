using Microservice.Email.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Microservice.Email.Tests;

/// <summary>
/// Base class for tests that provides common test utilities.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected EmailDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<EmailDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new EmailDbContext(options);
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
