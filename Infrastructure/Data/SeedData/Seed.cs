using ApplicationCore.Enums;
using Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace Infrastructure.Data.SeedData
{
    public class Seed
    {
        public static async Task SeedAsync(ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            await db.Database.MigrateAsync();

            // eğer rol veya kullanıcı varsa return ile çık
            if (await roleManager.Roles.AnyAsync() || await userManager.Users.AnyAsync()) return;

            // rolleri oluşturduk
            await roleManager.CreateAsync(new IdentityRole("Yonetici"));
            await roleManager.CreateAsync(new IdentityRole("Personel"));
            await roleManager.CreateAsync(new IdentityRole("Admin"));

            // company oluştur
            List<Company> companies = new List<Company>()
            {
                new Company() {
                    CompanyName = "UBISOFT",
                    CompanyPhoneNumber = "+90 (521) 512-51-51",
                    CompanyAddress = "Deniz Sk. 25/3 Etimesgut/ANKARA" ,
                    CompanyMail = "ubisoft@info.com",
                    CompanyStatus = CompanyStatus.As,
                    CompanyEstablishmentDate = new DateTime(1991, 1, 12),
                    ContractStartDate= new DateTime(2010, 1, 12),
                    VergiDairesi = VergiDairesi.Ulus,
                    VergiNumarasi = "0274865721",
                    MersisNumarasi = "0027486572100015"
                },
                new Company() {
                    CompanyName = "Microsoft",
                    CompanyPhoneNumber = "+90 (555) 513-21-55",
                    CompanyAddress = "Kırlangıç Mahallesi No.3 Çankaya/ANKARA" ,
                    CompanyMail = "microsoft@info.com",
                    CompanyStatus = CompanyStatus.Ltd,
                    CompanyEstablishmentDate = new DateTime(1975, 1, 12),
                    ContractStartDate= new DateTime(2018, 1, 12),
                    VergiDairesi = VergiDairesi.Taksim,
                    VergiNumarasi = "0274865721",
                    MersisNumarasi = "0027486572100015"
                },
            };
            await db.AddRangeAsync(companies);
            await db.SaveChangesAsync();


            // kişileri oluştur
            List<ApplicationUser> users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    UserName = "kullanici1@bilgeadamboost.com",
                    Email = "kullanici1@bilgeadamboost.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+90-(542)-342-42-42",
                    Address = "Ankara Eryaman Göksu Mah. 88. sk. Atatürk cad.",
                    Surname = "Keskinoğlu",
                    MiddleName = "Hasan",
                    Name = "Burak",
                    BirthDate = new DateTime(1991, 1, 12),
                    PlaceOfBirth = "Ankara",
                    IdentityNumber = "12345678912",
                    Job = Job.YazilimMuhendisi,
                    Picture = "man_default.png",
                    HireDate = new DateTime(2021, 1, 12),
                    ReleaseDate = new DateTime(2023, 1, 12),
                    Department = Department.IT,
                    Gender = Gender.Erkek,
                    Company = companies[0],
                    Salary = 15000
                },
                new ApplicationUser()
                {
                    UserName = "kullanici2@bilgeadamboost.com",
                    Email = "kullanici2@bilgeadamboost.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+90-(556)-231-13-13",
                    Address = "Bursa İnegöl Atatürk Mah. No: 52/21 Çiçek Apt. ",
                    Surname = "Çelik",
                    MiddleName = "Yıldız",
                    Name = "Derya",
                    BirthDate = new DateTime(1998, 3, 5),
                    PlaceOfBirth = "Bursa",
                    IdentityNumber = "19435325482",
                    Job =Job.Asci,
                    Picture = "woman_default.png",
                    HireDate = new DateTime(2021, 1, 12),
                    ReleaseDate = new DateTime(2025, 1, 12),
                    Department = Department.Engineer,
                    Gender = Gender.Kadin,
                    Company = companies[1],
                    Salary = 24000
                },
                new ApplicationUser()
                {
                    UserName = "admin@bilgeadam.com",
                    Email = "admin@bilgeadam.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+90-(555)-555-55-55",
                    Address = "Bursa İnegöl Atatürk Mah. No: 52/21 Çiçek Apt. ",
                    Surname = "Admin",
                    MiddleName = "Admin",
                    Name = "Admin",
                    BirthDate = new DateTime(1998, 3, 5),
                    PlaceOfBirth = "Admin",
                    IdentityNumber = "19435325482",
                    Job =Job.Admin,
                    Picture = "man_default.png",
                    Gender = Gender.Erkek
                }
            };


            await userManager.CreateAsync(users[0], "Asd123.");
            await userManager.CreateAsync(users[1], "Asd123.");
            await userManager.CreateAsync(users[2], "Asd123.");

            // rolü kullanıcıya ata
            var user1 = await userManager.FindByEmailAsync(users[0].Email);
            var user2 = await userManager.FindByEmailAsync(users[1].Email);
            var user3 = await userManager.FindByEmailAsync(users[2].Email);

            await userManager.AddToRoleAsync(user1, "Yonetici");
            await userManager.AddToRoleAsync(user2, "Yonetici");
            await userManager.AddToRoleAsync(user3, "Admin");
        }
    }
}
