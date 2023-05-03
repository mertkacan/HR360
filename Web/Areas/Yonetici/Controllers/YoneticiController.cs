using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Identity.Data;
using Web.Extensions;
using Web.Models;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ApplicationCore.Specifications;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity.Data;
using ApplicationCore.Enums;
using ApplicationCore.Services;
using Newtonsoft.Json;

namespace Web.Controllers
{
    [Area("Yonetici")]
    [Authorize(Roles = ("Yonetici"))]
    public class YoneticiController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICompanyService _companyService;
        private readonly IExpenseService _expenseService;
        private readonly IAdvanceService _advanceService;

        public string UserId => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public YoneticiController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IApplicationUserService applicationUserService,
            IHttpContextAccessor httpContextAccessor,
            ICompanyService companyService,
            IExpenseService expenseService,
            IAdvanceService advanceService)
        {

            _userManager = userManager;
            _env = env;
            _applicationUserService = applicationUserService;
            _httpContextAccessor = httpContextAccessor;
            _companyService = companyService;
            _expenseService = expenseService;
            _advanceService = advanceService;
        }

        [Route("Yonetici"), HttpGet]
        public async Task<IActionResult> Index()
        {
            // Kullanıcının ApplicationUser nesnesini UserManager üzerinden alıyoruz
            var user = await _userManager.FindByIdAsync(UserId);
            var appUserDTO = user.ToAppUserDTO();

            var yoneticiIndexViewModel = new YoneticiViewModel() { AppUserDTO = appUserDTO };

            return View(yoneticiIndexViewModel);
        }

        [Route("Yonetici/Tumbilgi")]
        public async Task<IActionResult> TumBilgi()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            await user.UserCompanyIncludeAsync(_companyService);
            var appUserDTO = user.ToAppUserDTO();

            return View(appUserDTO);
        }

        public async Task<IActionResult> BilgileriDuzenle()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var appUserDTO = user.ToAppUserDTO();

            return View(appUserDTO);
        }

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

            return RedirectToAction("Index", "Yonetici", new { email = appUserDTO.Email });
        }

        [Route("Yonetici/PersonelEkle")]
        public IActionResult PersonelEkle()
        {
            return View();
        }

        [HttpPost]
        [Route("Yonetici/PersonelEkle")]
        public async Task<IActionResult> PersonelEkle(YoneticiViewModel yoneticiViewModel)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            await user.UserCompanyIncludeAsync(_companyService);
            yoneticiViewModel.AppUserDTO.CompanyDTO = user.Company!.ToAppCompanyDTO();
            
            // bu koşul işten çıkış tarihi boş ise 2099 yılına ayarlıyor. çünkü boş kalırsa default 0001 yılına ayarlanıyor ve işe giriş tarihinden önce olamıyor
            if (yoneticiViewModel.AppUserDTO.ReleaseDate == null)
                yoneticiViewModel.AppUserDTO.ReleaseDate = new DateTime(2099, 01, 01);

            // bu koşul işe giriş tarihi işten çıkış tarihinden önce olup olamayacağını kontrol ediyor
            if (yoneticiViewModel.AppUserDTO.HireDate > yoneticiViewModel.AppUserDTO.ReleaseDate)
            {
                ModelState.AddModelError("girisTarih", "İşe giriş tarihi çıkış tarihinden önce olamaz");
                return View();
            }

            // bu koşul işten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olup olamayağını kontrol ediyor
            if (yoneticiViewModel.AppUserDTO.ReleaseDate.Value.Year != 2099)
            {
                TimeSpan? difference = yoneticiViewModel.AppUserDTO.ReleaseDate - yoneticiViewModel.AppUserDTO.HireDate;
                if (difference.HasValue && difference.Value.TotalDays > 30)
                {
                    ModelState.AddModelError("cikisTarih", "İşten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olabilir");
                    return View();
                }
            }

            if (!ModelState.IsValid)
                return View();

            //+90 ile ekleme yapıyoruz. bu metod extension da yapılabilir
            yoneticiViewModel.AppUserDTO.PhoneNumber = "+90 " + yoneticiViewModel.AppUserDTO.PhoneNumber;

            var picName = yoneticiViewModel.AppUserDTO.Picutre!.PictureDefaultCheckAndDeleteReturnPicName(_env);

            picName = await yoneticiViewModel.AppUserDTO.Resim!.SaveFileReturnPicNameAsync(_env);

            if (picName == "default")
                yoneticiViewModel.AppUserDTO.Picutre = yoneticiViewModel.AppUserDTO.Gender == Gender.Erkek ?
                    "man_default.png" : "woman_default.png";

            else
                yoneticiViewModel.AppUserDTO.Picutre = picName;

            await yoneticiViewModel.AppUserDTO.ConvertEmailAsync();

            await _applicationUserService.CreatePersonelAsync(yoneticiViewModel.AppUserDTO);

            TempData["sweetAlertMessage"] = $"{yoneticiViewModel.AppUserDTO.Name} adlı personel başarıyla eklendi. Personel giriş bilgileri e-mail'inize gönderilmiştir";
            TempData["sweetAlertType"] = "success";

            return View(yoneticiViewModel);
        }


        [Route("Yonetici/PersonelListele")]
        public async Task<IActionResult> PersonelListele()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            //  şirket ismini getirmek için include ediyoruz
            await user.UserCompanyIncludeAsync(_companyService);


            var appUserDTO = user.ToAppUserDTO();

            var yoneticiIndexViewModel = new YoneticiViewModel();

            var usersListe = await _applicationUserService.GetAllPersonelAsync();

            var userCompanyListe = usersListe.Where(x => x.CompanyId == user.CompanyId).ToList();

            var usersDTOListe = userCompanyListe.Select(x => x.ToAppUserDTO()).ToList();

            yoneticiIndexViewModel.AppUsersDTOListe = usersDTOListe;
            yoneticiIndexViewModel.AppUserDTO = appUserDTO;

            return View(yoneticiIndexViewModel);
        }


        [Route("Yonetici/PersonelDetay/{personelEmail}")]
        public async Task<IActionResult> PersonelDetay(string personelEmail)
        {
            var user = await _userManager.FindByEmailAsync(personelEmail);

            await user.UserCompanyIncludeAsync(_companyService);


            var appUserDTO = user.ToAppUserDTO();

            var yoneticiViewModel = new YoneticiViewModel() { AppUserDTO = appUserDTO };

            return View(yoneticiViewModel);
        }


        [Route("Yonetici/PersonelBilgiDuzenle/{personelEmail}")]
        public async Task<IActionResult> PersonelBilgiDuzenle(string personelEmail)
        {
            var user = await _userManager.FindByEmailAsync(personelEmail);
            var appUserDTO = user.ToAppUserDTO();
            return View(appUserDTO);
        }

        [Route("Yonetici/PersonelBilgiDuzenle/{personelEmail}")]
        [HttpPost]
        public async Task<IActionResult> PersonelBilgiDuzenle(AppUserDTO appUserDTO)
        {

            //+90 ile ekleme yapıyoruz
            appUserDTO.PhoneNumber = "+90 " + appUserDTO.PhoneNumber;

            // bu koşul işten çıkış tarihi boş ise 2099 yılına ayarlıyor. çünkü boş kalırsa default 0001 yılına ayarlanıyor ve işe giriş tarihinden önce olamıyor
            if (appUserDTO.ReleaseDate == null)
                appUserDTO.ReleaseDate = new DateTime(2099, 01, 01);


            // bu koşul işe giriş tarihi işten çıkış tarihinden önce olup olamayacağını kontrol ediyor
            if (appUserDTO.HireDate > appUserDTO.ReleaseDate)
            {
                ModelState.AddModelError("girisTarih", "İşe giriş tarihi çıkış tarihinden önce olamaz");
                return View(appUserDTO);
            }

            // bu koşul işten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olup olamayağını kontrol ediyor
            if (appUserDTO.ReleaseDate.Value.Year != 2099)
            {
                TimeSpan? difference = appUserDTO.ReleaseDate - appUserDTO.HireDate;
                if (difference.HasValue && difference.Value.TotalDays > 30)
                {
                    ModelState.AddModelError("cikisTarih", "İşten çıkış tarihi giriş tarihinden en fazla 30 gün sonra olabilir");
                    return View(appUserDTO);
                }
            }

            if (!ModelState.IsValid)
                return View(appUserDTO);


            //await appUserDTO.ConvertEmailAsync();

            var picName = appUserDTO.Picutre!.PictureDefaultCheckAndDeleteReturnPicName(_env);

            picName = await appUserDTO.Resim!.SaveFileReturnPicNameAsync(_env);

            if (picName == "default")
                appUserDTO.Picutre = appUserDTO.Gender == Gender.Erkek ?
                    "man_default.png" : "woman_default.png";

            else
                appUserDTO.Picutre = picName;

            await _applicationUserService.UpdateApplicationUserAsync(appUserDTO);

            TempData["sweetAlertMessage"] = "Bilgileriniz başarıyla güncellendi.";
            TempData["sweetAlertType"] = "success";

            return View(appUserDTO);
        }

        [Route("Yonetici/PersonelTalep")]

        public async Task<IActionResult> PersonelTalep()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            //  şirket ismini getirmek için include ediyoruz
            await user.UserCompanyIncludeAsync(_companyService);


            // kullanıcıyı appUserDTO'ya çeviriyoruz
            var appUserDTO = user.ToAppUserDTO();

            // yoneticiViewModel oluşturuyoruz çünkü view'a birden fazla model gönderemiyoruz
            var YoneticiViewModel = new YoneticiViewModel();

            // personel listesini include ile çekiyoruz çünkü personelin giderlerini de çekmek istiyoruz
            _userManager.Users.Include(x => x.Expenses)
                .ToList();

            // boş bir liste oluşturuyoruz. içine personelin giderlerini ekleyeceğiz
            var appExpenseDTOIncludeList = new List<AppExpenseDTO>();

            // spec içidne include ediyoruz. çünkü giderlerin içinde personel bilgileri de var
            var specExpenseUserInclude = new ExpenseUserIncludeSpecification(user.CompanyId);

            // expenselere appUser include edilmiş liste
            var expenseListe = await _expenseService.GetAllExpensesAsync(specExpenseUserInclude);

            // expense listesini expenseDTO listeisne çevirdik
            appExpenseDTOIncludeList = expenseListe!.Select(x => x.ToAppExpenseDto())
                .ToList();

            YoneticiViewModel.AppUserDTO = appUserDTO;
            YoneticiViewModel.AppExpensesDTOListe = appExpenseDTOIncludeList.OrderByDescending(x => x.CreationDate)
                .ToList();

            return View(YoneticiViewModel);
        }

        public async Task<IActionResult> HarcamaOnay(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);

            expense!.Approval = Approval.Onaylandi;
            expense.ApprovalDate = DateTime.Now;

            await _expenseService.UpdateExpenseAsync(expense);


            TempData["sweetAlertMessage"] = $"{expense.TypeOfExpenses} için {expense.ExpenseAmount} {expense.Currency} tutarındaki harcama isteği başarıyla onaylandı";
            TempData["sweetAlertType"] = "success";

            return RedirectToAction("PersonelTalep");
        }

        [Route("Yonetici/HarcamaRed/{id?}/{mesaj?}")]
        public async Task<string> HarcamaRed(int id, string mesaj)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);

            expense!.Approval = Approval.Reddedildi;
            expense.ApprovalDate = DateTime.Now;
            expense.Invoce.DeleteFile(_env);
            expense.ManagerDescription = mesaj;

            await _expenseService.UpdateExpenseAsync(expense);

            TempData["sweetAlertMessage"] = $"{expense.TypeOfExpenses} için {expense.ExpenseAmount} {expense.Currency} tutarındaki harcama isteği başarıyla reddedildi";
            TempData["sweetAlertType"] = "success";

            return "Reddedildi";
        }

        // dosya gösterme
        [Route("Yonetici/ShowFile/{file}")]
        public IActionResult ShowFile(string file)
        {
            var fileName = "dosyalar/" + file;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var contentType = GetContentType(fileName);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,

                // açıklama : inline sayesinde dosya indirme işlemi yapılmaz. tarayıcıda açılır
                Inline = true
            };


            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(fileStream, contentType);
        }

        // dosya türüne göre içerik türünü döndürür
        private string GetContentType(string fileName)
        {
            string contentType;
            switch (Path.GetExtension(fileName).ToLowerInvariant())
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".jpeg":
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            return contentType;
        }

        [Route("Yonetici/PersonelAvansListele")]
        public async Task<IActionResult> PersonelAvansListele()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            //  şirket ismini getirmek için include ediyoruz
            await user.UserCompanyIncludeAsync(_companyService);


            // kullanıcıyı appUserDTO'ya çeviriyoruz
            var appUserDTO = user.ToAppUserDTO();

            // yoneticiViewModel oluşturuyoruz çünkü view'a birden fazla model gönderemiyoruz
            var YoneticiViewModel = new YoneticiViewModel();

            // personel listesini include ile çekiyoruz çünkü personelin giderlerini de çekmek istiyoruz
            _userManager.Users.Include(x => x.Advances)
                .ToList();

            // boş bir liste oluşturuyoruz. içine personelin giderlerini ekleyeceğiz
            var appAdvanceDTOIncludeList = new List<AppAdvanceDTO>();

            // spec içidne include ediyoruz. çünkü giderlerin içinde personel bilgileri de var
            var specAdvanceUserInclude = new AdvanceUserIncludeSpecification(user.CompanyId);

            // expenselere appUser include edilmiş liste
            var advanceListe = await _advanceService.GetAdvancesByUserIdAsync(specAdvanceUserInclude);

            // expense listesini expenseDTO listeisne çevirdik
            appAdvanceDTOIncludeList = advanceListe!.Select(x => x.ToAppAdvanceDTO())
                .ToList();

            YoneticiViewModel.AppUserDTO = appUserDTO;
            YoneticiViewModel.AppAdvanceDTOListe = appAdvanceDTOIncludeList.OrderByDescending(x => x.CreationDate)
                .ToList();

            return View(YoneticiViewModel);
        }

        public async Task<IActionResult> PersonelAvansOnay(int id)
        {
            var advance = await _advanceService.GetAdvanceByIdAsync(id);

            advance!.Approval = Approval.Onaylandi;
            advance.ApprovalDate = DateTime.Now;

            await _advanceService.UpdateAdvanceAsync(advance);


            TempData["sweetAlertMessage"] = $"{advance.TypeOfAdvance} için {advance.AdvanceAmount} {advance.Currency} tutarındaki avans isteği başarıyla onaylandı";
            TempData["sweetAlertType"] = "success";

            return RedirectToAction("PersonelAvansListele");
        }

        [Route("Yonetici/PersonelAvansRed/{id?}/{mesaj?}")]
        public async Task<string> PersonelAvansRed(int id, string mesaj)
        {
            var advance = await _advanceService.GetAdvanceByIdAsync(id);
            var user = await _userManager.FindByIdAsync(advance.ApplicationUserID);

            advance!.Approval = Approval.Reddedildi;
            advance.ApprovalDate = DateTime.Now;
            advance.ManagerDescription = mesaj;

            user.AdvanceSpent -= (decimal)advance.AdvanceAmount!;
            await _applicationUserService.UpdateApplicationUserAsync(user.ToAppUserDTO());

            await _advanceService.UpdateAdvanceAsync(advance);

            TempData["sweetAlertMessage"] = $"{advance.TypeOfAdvance} için {advance.AdvanceAmount} {advance.Currency} tutarındaki avans isteği başarıyla reddedildi";
            TempData["sweetAlertType"] = "success";

            return "Reddedildi";
        }
    }
}
