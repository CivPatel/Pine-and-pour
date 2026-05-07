using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PinePour.Api.Data;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=PinePourDesignTime;User Id=sa;Password=Password123!;TrustServerCertificate=True");

        return new DataContext(optionsBuilder.Options);
    }
}
