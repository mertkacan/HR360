using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Web.Areas.Identity.Data;
using Web.Models;

namespace Web.Extensions
{
    public static class MappingExtensions
    {
        public static AppUserDTO ToAppUserDTO(this ApplicationUser appUser)
        {
            if (appUser == null)
            {
                return new AppUserDTO();
            }

            return new AppUserDTO()
            {
                Id = appUser.Id,
                PhoneNumber = appUser.PhoneNumber,
                Name = appUser.Name,
                MiddleName = appUser.MiddleName,
                Surname = appUser.Surname,
                SecondSurname = appUser.SecondSurname,
                BirthDate = appUser.BirthDate,
                PlaceOfBirth = appUser.PlaceOfBirth,
                IdentityNumber = appUser.IdentityNumber,
                HireDate = appUser.HireDate,
                ReleaseDate = appUser.ReleaseDate,
                Job = appUser.Job,
                Address = appUser.Address,
                Salary = appUser.Salary,
                Picutre = appUser.Picture,
                Email = appUser.Email,
                Gender = appUser.Gender,
                Department = appUser.Department,
                CompanyDTO = appUser.Company.ToAppCompanyDTO(),
                AdvanceSpent = appUser.AdvanceSpent,
                FirstAdvanceDate = appUser.FirstAdvanceDate,
            };
        }

        public async static Task ConvertEmailAsync(this AppUserDTO appUserDTO)
        {
            var name = appUserDTO.Name.Trim().ToLower();
            var surname = appUserDTO.Surname.Trim().ToLower();

            Dictionary<char, char> degisimTablosu = new Dictionary<char, char>()
            {
                {'ç', 'c'},
                {'ğ', 'g'},
                {'ı', 'i'},
                {'ö', 'o'},
                {'ş', 's'},
                {'ü', 'u'}
             };

            string orjinalMail = name + "." + surname + "@bilgeadamboost.com";
            string yeniMail = "";

            foreach (char harf in orjinalMail)
            {
                if (degisimTablosu.ContainsKey(harf))
                    yeniMail += degisimTablosu[harf];

                else
                    yeniMail += harf;
            }

            appUserDTO.Email = yeniMail;
        }

        public async static Task UserCompanyIncludeAsync(this ApplicationUser applicationUser, ICompanyService companyService)
        {
            var specCompany = new CompanyFilterSpecification(applicationUser);

            var company = await companyService.FirstOrDefaultAsync(specCompany);

            applicationUser.Company = company!;
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (member == null)
                return enumValue.ToString();

            var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute == null)
                return enumValue.ToString();

            return displayAttribute.GetName()!;
        }

        public async static Task<List<Expense>> ExpensesFilterAsync(this ApplicationUser applicationUser, IRepository<Expense> repository)
        {
            var specExpenses = new ExpensesFilterSpecification(applicationUser.Id);
            var expensesList = await repository.ListAsync(specExpenses);

            return expensesList;
        }

        public async static Task<List<Advance>> AdvancesFilterAsync(this ApplicationUser appUser, IAdvanceService advanceService)
        {
            var specAdvanceUserInclude = new AdvancesFilterSpecification(appUser);
            var advances = await advanceService.GetAdvancesByUserIdAsync(specAdvanceUserInclude);

            return advances;
        }

        public static AppExpenseDTO ToAppExpenseDto(this Expense expense)
        {
            if (expense == null) return new AppExpenseDTO();

            return new AppExpenseDTO()
            {
                ExpenseAmount = expense.ExpenseAmount,
                CreationDate = expense.CreationDate,
                Approval = expense.Approval,
                Currency = expense.Currency,
                TypeOfExpenses = expense.TypeOfExpenses,
                Description = expense.Description,
                ApprovalDate = expense.ApprovalDate,
                ManagerDescription = expense.ManagerDescription,
                AppUserDTOId = expense.ApplicationUserID,
                PictureName = expense.Invoce,
                AppUserDTO = expense.ApplicationUser.ToAppUserDTO(),
                Id = expense.Id
            };

        }

        // bakılacak
        public static AppCompanyDTO ToAppCompanyDTO(this Company company)
        {
            if (company == null) return new AppCompanyDTO();

            return new AppCompanyDTO()
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress,
                CompanyMail = company.CompanyMail,
                CompanyPhoneNumber = company.CompanyPhoneNumber,
                Logo = company.Logo,
                CompanyEstablishmentDate = company.CompanyEstablishmentDate,
                CompanyStatus = company.CompanyStatus,
                ContractFinishDate = company.ContractFinishDate,
                ContractStartDate = company.ContractStartDate,
                IsActive = company.IsActive,
                MersisNumarasi = company.MersisNumarasi,
                VergiDairesi = company.VergiDairesi,
                VergiNumarasi = company.VergiNumarasi
            };
        }

        // bakılacak
        public static AppAdvanceDTO ToAppAdvanceDTO(this Advance advance)
        {
            if (advance == null) return new AppAdvanceDTO();

            return new AppAdvanceDTO()
            {
                Id = advance.Id,
                AdvanceAmount = advance.AdvanceAmount,
                Currency = advance.Currency,
                CreationDate = advance.CreationDate,
                Approval = advance.Approval,
                ApprovalDate = advance.ApprovalDate,
                Description = advance.Description,
                TypeOfAdvance = advance.TypeOfAdvance,
                AppUserDTOId = advance.ApplicationUserID,
                AppUserDTO = advance.ApplicationUser.ToAppUserDTO(),
                ManagerDescription = advance.ManagerDescription,
            };
        }

        // açıklama 1/3 : foto adı default ise kendi adını döndürür, değilse siler ve "default" string döndürür
        public static string PictureDefaultCheckAndDeleteReturnPicName(this string picName, IWebHostEnvironment _env)
        {
            if (picName != "man_default.png" &&
                picName != "woman_default.png" &&
                picName != "company_default.png" &&
                picName != null)
            {
                File.Delete(Path.Combine(_env.WebRootPath, "images", picName)); //güncellerken önceki resmi silme işlemi
                return "default";
            }

            return picName;
        }

        // açıklama 2/3 : dosya null ise "default" string dönderir, dosya varsa guid isim verip uzantıyla birlikte images içine kaydeder
        public async static Task<string> SaveFileReturnPicNameAsync(this IFormFile formFile, IWebHostEnvironment _env)
        {
            if (formFile == null) return "default";

            string fileExtension = Path.GetExtension(formFile.FileName);

            if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jpeg")
                return "default";

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);

            var filePath = Path.Combine(_env.WebRootPath, "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return fileName;
        }

        public static void DeleteFile(this string fileName, IWebHostEnvironment _env)
        {
            var filePath = Path.Combine(_env.WebRootPath, "dosyalar", fileName);

            if (File.Exists(filePath))
                File.Delete(Path.Combine(_env.WebRootPath, "dosyalar", fileName));
        }

        public static void AdvanceLimitCheck(this ApplicationUser applicationUser)
        {
            TimeSpan? timeSpan = DateTime.Now - applicationUser.FirstAdvanceDate;
            if (timeSpan?.TotalDays > 365)
            {
                applicationUser.AdvanceSpent = 0;
            }
        }
    }
}
