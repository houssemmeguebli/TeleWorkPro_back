using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TTProject.Core.Entities;

namespace TTProject.Infrastructure.Data
{
    public class TTProjectContextOld : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public TTProjectContextOld(DbContextOptions<TTProjectContextOld> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<ProjectManager> ProjectManagers { get; set; }
        public DbSet<TTRequest> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TTProjectContextOld).Assembly);
            base.OnModelCreating(modelBuilder);

                    modelBuilder.Entity<TTRequest>()
               .HasOne(r => r.Employee)
               .WithMany(e => e.Requests)
               .HasForeignKey(r => r.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict);

                    modelBuilder.Entity<TTRequest>()
                        .HasOne(r => r.ProjectManager)
                        .WithMany(pm => pm.Requests)
                        .HasForeignKey(r => r.ProjectManagerId)
                        .OnDelete(DeleteBehavior.Restrict);
                }
    }
}