using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using ApplicationCore.Extensions;
using Infrastructure.Identity.Data;

namespace ApplicationCore.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        private readonly ICompanyService _companyService;

        public string UserId => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public ApplicationUserService(UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IMailService mailService,
            ICompanyService companyService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
            _companyService = companyService;
        }
        public async Task CreatePersonelAsync(AppUserDTO appUserDTO)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var password = PasswordGenerator.GeneratePassword();

            var personel = _mapper.Map<AppUserDTO, ApplicationUser>(appUserDTO);
            personel.Picture = appUserDTO.Picutre!;
            personel.Gender = appUserDTO.Gender;
            personel.Department = appUserDTO.Department;
            personel.Email = appUserDTO.Email;
            personel.CompanyId = user.CompanyId;
            personel.UserName = appUserDTO.Email;
            var result = await _userManager.CreateAsync(personel, password);

            if (!result.Succeeded)
                return;

            await _userManager.AddToRoleAsync(personel, "Personel");

            _mailService.SendEmailAsync(personel, password);
        }

        public async Task UpdateApplicationUserAsync(AppUserDTO appUserDTO)
        {
            var user = await _userManager.FindByEmailAsync(appUserDTO.Email);
            var roles = await _userManager.GetRolesAsync(user);

            // giriş yapan kişi çekiyoruz
            var loginUser = await _userManager.FindByIdAsync(UserId);

            // giren kişinin rollerini çekiyoruz
            var loginUserRoles = await _userManager.GetRolesAsync(loginUser);

            if ((loginUserRoles.Any(x => x == "Personel") && roles.Any(x => x == "Personel")) ||
                (loginUserRoles.Any(x => x == "Yonetici") && roles.Any(x => x == "Yonetici")) ||
                (loginUserRoles.Any(x => x == "Admin") && roles.Any(x => x == "Admin")))
            {
                user.Address = appUserDTO.Address;
                user.PhoneNumber = appUserDTO.PhoneNumber;
                user.Picture = appUserDTO.Picutre!;
                user.AdvanceSpent = appUserDTO.AdvanceSpent;
                user.FirstAdvanceDate = appUserDTO.FirstAdvanceDate;

                await _userManager.UpdateAsync(user);
            }

            else
            {
                if (appUserDTO.Name != null && appUserDTO.Surname != null && (user.Name != appUserDTO.Name || user.Surname != appUserDTO.Surname))
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
                    user.Email = yeniMail;
                    user.UserName = yeniMail;
                }

                var company = new Company();

                if (_userManager.GetRolesAsync(loginUser).Result.Any(x => x == "Yonetici"))
                    company = await _companyService.GetCompanyByIdAsync(user.CompanyId);

                else
                    company = await _companyService.GetCompanyByIdAsync(appUserDTO.CompanyDTO!.Id);

                // kime gideceği belli değil "user" lazım
                user.Name = appUserDTO.Name!;
                user.MiddleName = appUserDTO.MiddleName;
                user.Surname = appUserDTO.Surname!;
                user.SecondSurname = appUserDTO.SecondSurname;
                user.BirthDate = appUserDTO.BirthDate;
                user.PlaceOfBirth = appUserDTO.PlaceOfBirth;
                user.IdentityNumber = appUserDTO.IdentityNumber;
                user.PhoneNumber = appUserDTO.PhoneNumber;
                user.Picture = appUserDTO.Picutre!;
                user.HireDate = appUserDTO.HireDate;
                user.ReleaseDate = appUserDTO.ReleaseDate;
                user.Department = appUserDTO.Department;
                user.Salary = appUserDTO.Salary;
                user.Gender = appUserDTO.Gender;
                user.Address = appUserDTO.Address;
                user.Job = appUserDTO.Job;
                user.Company = company;
                user.CompanyId = company.Id;
                // Kontrol et isim değişirse mail değişmeli!

                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<List<ApplicationUser>> GetAllPersonelAsync()
        {
            var personelUsers = await _userManager.GetUsersInRoleAsync("Personel");

            return personelUsers.ToList();
        }

        public async Task<List<ApplicationUser>> GetAllYoneticiAsync()
        {
            var yoneticiUsers = await _userManager.GetUsersInRoleAsync("Yonetici");

            return yoneticiUsers.ToList();
        }

        public async Task CreateYoneticiAsync(AppUserDTO appUserDTO)
        {
            var company = await _companyService.GetCompanyByIdAsync(appUserDTO.CompanyDTO!.Id);

            var password = PasswordGenerator.GeneratePassword();

            var yonetici = _mapper.Map<AppUserDTO, ApplicationUser>(appUserDTO);
            yonetici.Picture = appUserDTO.Picutre!;
            yonetici.Gender = appUserDTO.Gender;
            yonetici.Department = appUserDTO.Department;
            yonetici.Email = appUserDTO.Email;
            yonetici.CompanyId = appUserDTO.CompanyDTOId;
            yonetici.Company = company;
            yonetici.UserName = appUserDTO.Email;
            var result = await _userManager.CreateAsync(yonetici, password);

            if (!result.Succeeded)
                return;

            await _userManager.AddToRoleAsync(yonetici, "Yonetici");

            _mailService.SendEmailAsync(yonetici, password);
        }
    }
}
