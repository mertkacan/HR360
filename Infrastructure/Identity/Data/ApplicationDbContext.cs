
using Infrastructure.Data.SeedData;
using Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection;
using ApplicationCore.Entities;

namespace Web.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Advance> Advances => Set<Advance>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<IdentityUserRole<string>>(userRole =>
        {
            userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
        });


        builder.Entity<IdentityUserLogin<string>>().HasNoKey();
        builder.Entity<IdentityUserToken<string>>().HasNoKey();
    }
}
