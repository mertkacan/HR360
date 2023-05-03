using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace Infrastructure.Data.Config
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.Name)
             .HasMaxLength(30);

            builder.Property(x => x.Surname)
               .HasMaxLength(30);

            builder.Property(x => x.BirthDate)
               .IsRequired();

            builder.Property(x => x.IdentityNumber)
               .HasMaxLength(11)
               .IsRequired();

            builder.Property(x => x.Salary)
               .HasPrecision(18, 2);

            builder.HasOne(x => x.Company)
                .WithMany(x => x.ApplicationUsers)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CompanyId).IsRequired(false);
        }
    }
}
