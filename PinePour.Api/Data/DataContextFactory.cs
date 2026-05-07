using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PinePour.Api.Data;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var environmentName =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var provider = configuration["Database:Provider"] ?? "Postgres";
        var connectionString = configuration.GetConnectionString("DataContext");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing database connection string. Set ConnectionStrings__DataContext before running EF tooling.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase)
            || provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseNpgsql(PostgresConnectionStringNormalizer.Normalize(connectionString));
        }
        else if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                 || provider.Equals("Sql", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported Database:Provider '{provider}'.");
        }

        return new DataContext(optionsBuilder.Options);
    }
}
