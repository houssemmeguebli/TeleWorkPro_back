using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TTProject.Infrastructure.Data;

public class TTProjectContext : IdentityDbContext<IdentityUser>
{
    public TTProjectContext(DbContextOptions<TTProjectContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }
}
