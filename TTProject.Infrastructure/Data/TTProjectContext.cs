using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TTProject.Core.Entities;

namespace TTProject.Infrastructure.Data
{
    public class TTProjectContext : IdentityDbContext

    {

        public TTProjectContext(DbContextOptions<TTProjectContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ProjectManager> ProjectManagers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TTRequest> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TTProjectContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
