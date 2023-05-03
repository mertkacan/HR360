using ApplicationCore.DTO;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Areas.Identity.Data;
using Web.Extensions;
using Web.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICompanyService _companyService;

        public string UserId => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public AdminController(UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IApplicationUserService applicationUserService,
            IHttpContextAccessor httpContextAccessor,
            ICompanyService companyService)
        {
            _userManager = userManager;
            _env = env;
            _applicationUserService = applicationUserService;
            _httpContextAccessor = httpContextAccessor;
            _companyService = companyService;
        }

        [Route("Admin"), HttpGet]
        public async Task<IActionResult> Index()
        {
            // Kullanıcının ApplicationUser nesnesini UserManager üzerinden alıyoruz
            var user = await _userManager.FindByIdAsync(UserId);
            var appUserDTO = user.ToAppUserDTO();

            var adminViewModel = new AdminViewModel() { AppUserDTO = appUserDTO };

            return View(adminViewModel);
        }

        [Route("Admin/Tumbilgi")]
        public async Task<IActionResult> TumBilgi()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var appUserDTO = user.ToAppUserDTO();

            var adminViewModel = new AdminViewModel() { AppUserDTO = appUserDTO };

            return View(adminViewModel);
        }


        [Route("Admin/BilgileriDuzenle")]
        public async Task<IActionResult> BilgileriDuzenle()
        {

            var user = await _userManager.FindByIdAsync(UserId);
            var appUserDTO = user.ToAppUserDTO();


            return View(appUserDTO);
        }

        [Route("Admin/BilgileriDuzenle")]
        [HttpPost]
        public async Task<IActionResult> BilgileriDuzenle(AppUserDTO appUserDTO)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (ModelState.GetFieldValidationState("Resim") == ModelValidationState.Invalid ||
                ModelState.GetFieldValidationState("Address") == ModelValidationState.Invalid ||
                ModelState.GetFieldValidationState("PhoneNumber") == ModelValidationState.Invalid)
            {
                appUserDTO = user.ToAppUserDTO();

                return View(appUserDTO);
            }
            // ??
            appUserDTO.Gender = user.Gender;

            //+90 ile ekleme yapıyoruz. bu metod extension da yapılabilir
            appUserDTO.PhoneNumber = "+90 " + appUserDTO.PhoneNumber;

            var picName = appUserDTO.Picutre!.PictureDefaultCheckAndDeleteReturnPicName(_env);

            picName = await appUserDTO.Resim!.SaveFileReturnPicNameAsync(_env);

            if (picName == "default")
                appUserDTO.Picutre = appUserDTO.Gender == Gender.Erkek ?
                    "man_default.png" : "woman_default.png";

            else
                appUserDTO.Picutre = picName;

            await _applicationUserService.UpdateApplicationUserAsync(appUserDTO);

            // sweetalert için
            TempData["sweetAlertMessage"] = "Bilgileriniz başarıyla güncellendi.";
            TempData["sweetAlertType"] = "success";

            return RedirectToAction("Index", "Admin");
        }

        [Route("Admin/YoneticiEkle")]
        public async Task<IActionResult> YoneticiEkle()
        {
            var adminViewModel = new AdminViewModel();

            var companyList = await _companyService.GetAllCompanyAsync();
            adminViewModel.AppCompanyDTOListe = companyList.Select(x => x.ToAppCompanyDTO()).ToList();
            return View(adminViewModel);
        }

        [Route("Admin/YoneticiEkle")]
        [HttpPost]
        public async Task<IActionResult> YoneticiEkle(AdminViewModel adminViewModel)
        {
            var companyList = await _companyService.GetAllCompanyAsync();

            if (!ModelState.IsValid)
            {
                if (adminViewModel.AppUserDTO!.CompanyDTO!.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    ModelState.AddModelError("company", "Şirket alanı zorunludur!");

                adminViewModel.AppCompanyDTOListe = companyList.Select(x => x.ToAppCompanyDTO()).ToList();
                return View(adminViewModel);
            }

            // bu koşul işten çıkış tarihi boş ise 2099 yılına ayarlıyor. çünkü boş kalırsa default 0001 yılına ayarlanıyor ve işe giriş tarihinden önce olamıyor
            if (adminViewModel.AppUserDTO!.ReleaseDate == null)
                adminViewModel.AppUserDTO.ReleaseDate = new DateTime(2099, 01, 01);


            // bu koşul işe giriş tarihi işten çıkış tarihinden önce olup olamayacağını kontrol ediyor
            if (adminViewModel.AppUserDTO.HireDate > adminViewModel.AppUserDTO.ReleaseDate)
            {
                ModelState.AddModelError("girisTarih", "İşe giriş tarihi çıkış tarihinden önce olamaz");
                return View();
            }

            // bu koşul işten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olup olamayağını kontrol ediyor
            if (adminViewModel.AppUserDTO.ReleaseDate.Value.Year != 2099)
            {
                TimeSpan? difference = adminViewModel.AppUserDTO.ReleaseDate - adminViewModel.AppUserDTO.HireDate;
                if (difference.HasValue && difference.Value.TotalDays > 30)
                {
                    ModelState.AddModelError("cikisTarih", "İşten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olabilir");
                    return View();
                }
            }

            if (!ModelState.IsValid)
                return View();

            //+90 ile ekleme yapıyoruz. bu metod extension da yapılabilir
            adminViewModel.AppUserDTO.PhoneNumber = "+90 " + adminViewModel.AppUserDTO.PhoneNumber;

            var picName = adminViewModel.AppUserDTO.Picutre!.PictureDefaultCheckAndDeleteReturnPicName(_env);

            picName = await adminViewModel.AppUserDTO.Resim!.SaveFileReturnPicNameAsync(_env);

            if (picName == "default")
                adminViewModel.AppUserDTO.Picutre = adminViewModel.AppUserDTO.Gender == Gender.Erkek ?
                    "man_default.png" : "woman_default.png";

            else
                adminViewModel.AppUserDTO.Picutre = picName;

            await adminViewModel.AppUserDTO.ConvertEmailAsync();

            var company = await _companyService.GetCompanyByIdAsync(adminViewModel.AppCompanyDTOId);

            adminViewModel.AppUserDTO.CompanyDTO = company.ToAppCompanyDTO();

            await _applicationUserService.CreateYoneticiAsync(adminViewModel.AppUserDTO);

            TempData["sweetAlertMessage"] = $"{adminViewModel.AppUserDTO.Name} adlı personel başarıyla eklendi. Personel giriş bilgileri e-mail'inize gönderilmiştir";
            TempData["sweetAlertType"] = "success";


            adminViewModel.AppCompanyDTOListe = companyList.Select(x => x.ToAppCompanyDTO()).ToList();

            return View(adminViewModel);
        }


        [Route("Admin/YoneticiListele")]
        public async Task<IActionResult> YoneticiListele()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            _userManager.Users.Include(x => x.Company)
             .ToList();

            var appUserDTO = user.ToAppUserDTO();

            var adminViewModel = new AdminViewModel();

            var usersListe = await _applicationUserService.GetAllYoneticiAsync();

            var usersDTOListe = usersListe.Select(x => x.ToAppUserDTO()).ToList();

            adminViewModel.AppUsersDTOListe = usersDTOListe.OrderBy(x => x.CompanyDTO!.CompanyName)
                .ThenBy(x => x.Name)
                .ToList();

            adminViewModel.AppUserDTO = appUserDTO;

            return View(adminViewModel);
        }

        [Route("Admin/YoneticiGuncelle/{yoneticiEmail}")]
        public async Task<IActionResult> YoneticiGuncelle(string yoneticiEmail)
        {
            var user = await _userManager.FindByEmailAsync(yoneticiEmail);
            var appUserDTO = user.ToAppUserDTO();

            var adminViewModel = new AdminViewModel() { AppUserDTO = appUserDTO };

            var companies = await _companyService.GetAllCompanyAsync();

            var appCompanyDTOList = companies.Select(x => x.ToAppCompanyDTO()).ToList();

            adminViewModel.AppCompanyDTOListe = appCompanyDTOList;
            return View(adminViewModel);
        }

        [Route("Admin/YoneticiGuncelle/{yoneticiEmail}")]
        [HttpPost]
        public async Task<IActionResult> YoneticiGuncelle(AdminViewModel adminViewModel)
        {
            var user = await _userManager.FindByIdAsync(adminViewModel.AppUserDTO!.Id);

            //+90 ile ekleme yapıyoruz
            adminViewModel.AppUserDTO!.PhoneNumber = "+90 " + adminViewModel.AppUserDTO.PhoneNumber;

            var companies = await _companyService.GetAllCompanyAsync();
            var company = await _companyService.GetCompanyByIdAsync(adminViewModel.AppCompanyDTOId);
            var appCompanyDTOList = companies.Select(x => x.ToAppCompanyDTO()).ToList();

            adminViewModel.AppCompanyDTO = company.ToAppCompanyDTO();
            adminViewModel.AppCompanyDTOListe = appCompanyDTOList;
            // bu koşul işten çıkış tarihi boş ise 2099 yılına ayarlıyor. çünkü boş kalırsa default 0001 yılına ayarlanıyor ve işe giriş tarihinden önce olamıyor
            if (adminViewModel.AppUserDTO.ReleaseDate == null)
                adminViewModel.AppUserDTO.ReleaseDate = new DateTime(2099, 01, 01);


            // bu koşul işe giriş tarihi işten çıkış tarihinden önce olup olamayacağını kontrol ediyor
            if (adminViewModel.AppUserDTO.HireDate > adminViewModel.AppUserDTO.ReleaseDate)
            {
                ModelState.AddModelError("girisTarih", "İşe giriş tarihi çıkış tarihinden önce olamaz");
                return View(adminViewModel);
            }

            // bu koşul işten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olup olamayağını kontrol ediyor
            if (adminViewModel.AppUserDTO.ReleaseDate.Value.Year != 2099)
            {
                TimeSpan? difference = adminViewModel.AppUserDTO.ReleaseDate - adminViewModel.AppUserDTO.HireDate;
                if (difference.HasValue && difference.Value.TotalDays > 30)
                {
                    ModelState.AddModelError("cikisTarih", "İşten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olabilir");
                    return View(adminViewModel);
                }
            }

            if (!ModelState.IsValid)
            {
                if (adminViewModel.AppCompanyDTO!.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    ModelState.AddModelError("company", "Şirket alanı zorunludur!");

                return View(adminViewModel);
            }


            var picName = adminViewModel.AppUserDTO.Picutre!.PictureDefaultCheckAndDeleteReturnPicName(_env);

            picName = await adminViewModel.AppUserDTO.Resim!.SaveFileReturnPicNameAsync(_env);

            if (picName == "default")
                adminViewModel.AppUserDTO.Picutre = adminViewModel.AppUserDTO.Gender == Gender.Erkek ?
                    "man_default.png" : "woman_default.png";

            else
                adminViewModel.AppUserDTO.Picutre = picName;

            adminViewModel.AppUserDTO.CompanyDTOId = company.Id;
            adminViewModel.AppUserDTO.CompanyDTO = company.ToAppCompanyDTO();

            await _applicationUserService.UpdateApplicationUserAsync(adminViewModel.AppUserDTO);

            TempData["sweetAlertMessage"] = "Bilgileriniz başarıyla güncellendi.";
            TempData["sweetAlertType"] = "success";

            return View(adminViewModel);
        }

        [Route("Admin/YoneticiTumBilgi/{yoneticiEmail}")]
        public async Task<IActionResult> YoneticiTumBilgi(string yoneticiEmail)
        {
            var user = await _userManager.FindByEmailAsync(yoneticiEmail);

            await user.UserCompanyIncludeAsync(_companyService);

            var appUserDTO = user.ToAppUserDTO();

            var adminViewModel = new AdminViewModel() { AppUserDTO = appUserDTO };

            return View(adminViewModel);
        }

        [Route("Admin/SirketEkle")]
        public IActionResult SirketEkle()
        {
            return View();
        }

        [Route("Admin/SirketEkle")]
        [HttpPost]
        public async Task<IActionResult> SirketEkle(AdminViewModel adminViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var picName = await adminViewModel.AppCompanyDTO!.LogoFile!.SaveFileReturnPicNameAsync(_env);
            adminViewModel.AppCompanyDTO.Logo = picName == "default" ? "company_default.png" : picName;

            if (adminViewModel.AppCompanyDTO!.MersisNumarasi!.Substring(1, 10) != adminViewModel.AppCompanyDTO.VergiNumarasi ||
                adminViewModel.AppCompanyDTO!.MersisNumarasi![0] != '0' ||
                adminViewModel.AppCompanyDTO!.MersisNumarasi!.Substring(11, 5) != "00015")
            {
                ModelState.AddModelError("mersisNo", "Geçersiz Mersis No girişi");
                return View();
            }

            // ?
            if (adminViewModel.AppCompanyDTO!.ContractFinishDate == null)
                adminViewModel.AppCompanyDTO.ContractFinishDate = new DateTime(2099, 01, 01);


            // bu koşul işe giriş tarihi işten çıkış tarihinden önce olup olamayacağını kontrol ediyor
            if (adminViewModel.AppCompanyDTO.ContractStartDate > adminViewModel.AppCompanyDTO.ContractFinishDate)
            {
                ModelState.AddModelError("baslangicTarih", "Sözleşme bitiş tarihi başlangıç tarihinden önce olamaz");
                return View();
            }

            // bu koşul işten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olup olamayağını kontrol ediyor
            if (adminViewModel.AppCompanyDTO.ContractFinishDate.Value.Year != 2099)
            {
                TimeSpan? difference = adminViewModel.AppCompanyDTO.ContractFinishDate - adminViewModel.AppCompanyDTO.ContractStartDate;
                if (difference.HasValue && difference.Value.TotalDays > 30)
                {
                    ModelState.AddModelError("bitisTarih", "Sözleşme bitiş tarihi başlangıç tarihinden en fazla 30 gün sonra olabilir");
                    return View();
                }
            }

            //+90 ile ekleme yapıyoruz. bu metod extension da yapılabilir
            adminViewModel.AppCompanyDTO.CompanyPhoneNumber = "+90 " + adminViewModel.AppCompanyDTO.CompanyPhoneNumber;

            //await adminViewModel.AppCompanyDTO.TakePictureNameAsync(_env);

            //await adminViewModel.AppUserDTO.ConvertEmailAsync();

            await _companyService.CreateCompanyAsync(adminViewModel.AppCompanyDTO);

            TempData["sweetAlertMessage"] = $"{adminViewModel.AppCompanyDTO.CompanyName} adlı şirket başarıyla eklendi.";
            TempData["sweetAlertType"] = "success";

            return View(adminViewModel);
        }

        [Route("Admin/SirketListele/")]
        public async Task<IActionResult> SirketListele()
        {

            var adminViewModel = new AdminViewModel();

            var companies = await _companyService.GetAllCompanyAsync();

            var appCompanyDTOList = companies.Select(x => x.ToAppCompanyDTO()).ToList();

            adminViewModel.AppCompanyDTOListe = appCompanyDTOList.OrderBy(x => x.CompanyName)
                                                    .ToList();

            return View(adminViewModel);
        }

        [Route("Admin/SirketDetay/{sirketEmail}")]
        public async Task<IActionResult> SirketDetay(string sirketEmail)
        {
            var company = await _companyService.GetCompanyByEmailAsync(sirketEmail);
            var adminViewModel = new AdminViewModel() { AppCompanyDTO = company.ToAppCompanyDTO() };

            return View(adminViewModel);
        }

        [Route("Admin/SirketGuncelle/{sirketEmail}")]
        public async Task<IActionResult> SirketGuncelle(string sirketEmail)
        {
            var company = await _companyService.GetCompanyByEmailAsync(sirketEmail);
            var adminViewModel = new AdminViewModel() { AppCompanyDTO = company.ToAppCompanyDTO() };

            return View(adminViewModel);
        }

        [Route("Admin/SirketGuncelle/{sirketEmail}")]
        [HttpPost]
        public async Task<IActionResult> SirketGuncelle(AdminViewModel adminViewModel)
        {
            adminViewModel.AppCompanyDTO!.CompanyPhoneNumber = "+90 " + adminViewModel.AppCompanyDTO!.CompanyPhoneNumber;

            if (adminViewModel.AppCompanyDTO.ContractFinishDate == null)
                adminViewModel.AppCompanyDTO.ContractFinishDate = new DateTime(2099, 01, 01);


            if (adminViewModel.AppCompanyDTO.ContractStartDate > adminViewModel.AppCompanyDTO.ContractFinishDate)
            {
                ModelState.AddModelError("girisTarih", "Sözleşme başlangıç tarihi bitiş tarihinden önce olamaz");
                return View(adminViewModel);
            }

            if (adminViewModel.AppCompanyDTO.ContractFinishDate.Value.Year != 2099)
            {
                TimeSpan? difference = adminViewModel.AppCompanyDTO.ContractFinishDate - adminViewModel.AppCompanyDTO.ContractStartDate;

                if (difference.HasValue && difference.Value.TotalDays > 30)
                {
                    ModelState.AddModelError("cikisTarih", "Sözleşme bitiş tarihi başlangıç tarihinden en fazla 30 gün sonra olabilir");
                    return View(adminViewModel);
                }
            }

            if (!ModelState.IsValid)
                return View(adminViewModel);

            var picName = adminViewModel.AppCompanyDTO.Logo!.PictureDefaultCheckAndDeleteReturnPicName(_env);

            picName = await adminViewModel.AppCompanyDTO.LogoFile!.SaveFileReturnPicNameAsync(_env);

            if (picName == "default")
                adminViewModel.AppCompanyDTO.Logo = "company_default.png";

            else
                adminViewModel.AppCompanyDTO.Logo = picName;

            await _companyService.UpdateCompanyAsync(adminViewModel.AppCompanyDTO);

            TempData["sweetAlertMessage"] = "Bilgileriniz başarıyla güncellendi.";
            TempData["sweetAlertType"] = "success";

            return View(adminViewModel);
        }
    }
}
