using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Web.Areas.Identity.Data;
using Web.Extensions;
using Web.Models;

namespace Web.Areas.Personel.Controllers
{
    [Area("Personel")]
    [Authorize(Roles = ("Personel"))]

    public class PersonelController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExpenseService _expenseService;
        private readonly IAdvanceService _advanceService;
        private readonly ICompanyService _companyService;

        public string UserId => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public PersonelController(UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IApplicationUserService applicationUserService,
            IHttpContextAccessor httpContextAccessor,
            IExpenseService expenseService,
            IAdvanceService advanceService,
            ICompanyService companyService)
        {
            _userManager = userManager;
            _env = env;
            _applicationUserService = applicationUserService;
            _httpContextAccessor = httpContextAccessor;
            _expenseService = expenseService;
            _advanceService = advanceService;
            _companyService = companyService;
        }
        [Route("Personel"), HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            user.AdvanceLimitCheck();

            var appUserDTO = user.ToAppUserDTO();
            var personelIndexViewModel = new PersonelViewModel()
            {
                AppUserDTO = appUserDTO,
            };

            return View(personelIndexViewModel);
        }

        [Route("Personel/Tumbilgi")]
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

            appUserDTO.Gender = user.Gender;
            appUserDTO.PhoneNumber = "+90 " + appUserDTO.PhoneNumber;

            // 1/3 açıklama metot içinde
            var picName = appUserDTO.Picutre!.PictureDefaultCheckAndDeleteReturnPicName(_env);

            // 2/3 açıklama metot içinde
            picName = await appUserDTO.Resim!.SaveFileReturnPicNameAsync(_env);

            // açıklama 3/3 : eğer picName "default" geldiyse cinsiyete göre resim ataması yapılır.
            if (picName == "default")
                appUserDTO.Picutre = appUserDTO.Gender == Gender.Erkek ?
                    "man_default.png" : "woman_default.png";
            // açıklama 3/3 : eğer picName "default" gelmediyse oluşan resim adı property'e atanır.
            else
                appUserDTO.Picutre = picName;

            await _applicationUserService.UpdateApplicationUserAsync(appUserDTO);

            TempData["sweetAlertMessage"] = "Bilgileriniz başarıyla güncellendi.";
            TempData["sweetAlertType"] = "success";

            return RedirectToAction("Index", "Personel", new { email = appUserDTO.Email });
        }

        [Route("Personel/HarcamaTalepIsteme")]
        public async Task<IActionResult> HarcamaTalepIsteme()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            await user.UserCompanyIncludeAsync(_companyService);
            var appUserDTO = user.ToAppUserDTO();
            var personelViewModel = new PersonelViewModel();
            personelViewModel.AppUserDTO = appUserDTO;

            return View(personelViewModel);
        }

        [Route("Personel/HarcamaTalepIsteme")]
        [HttpPost]
        public async Task<IActionResult> HarcamaTalepIsteme(PersonelViewModel personelViewModel)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            await user.UserCompanyIncludeAsync(_companyService);
            var appUserDTO = user.ToAppUserDTO();
            personelViewModel.AppUserDTO = appUserDTO;

            if (personelViewModel.AppExpenseDTO!.Invoce != null)
            {
                string[] allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
                string fileExtension = Path.GetExtension(personelViewModel.AppExpenseDTO.Invoce.FileName);

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    ModelState.AddModelError("Fatura", "Dosya türü desteklenmiyor!");
            }

            if (ModelState.IsValid)
            {
                personelViewModel.AppExpenseDTO.PictureName = await personelViewModel.AppExpenseDTO.Invoce!.SaveFileReturnPicNameAsync(_env);

                await _expenseService.CreateExpenseAsync(personelViewModel.AppExpenseDTO);

                TempData["sweetAlertMessage"] = "Harcama talebiniz yöneticinize başarıyla iletilmiştir.";
                TempData["sweetAlertType"] = "success";

                return View(personelViewModel);
            }

            return View(personelViewModel);
        }

        [Route("Personel/HarcamaTalepListe")]
        public async Task<IActionResult> HarcamaTalepListe()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            await user.UserCompanyIncludeAsync(_companyService);

            var appUserDTO = user.ToAppUserDTO();
            var personelViewModel = new PersonelViewModel();

            var specExpensesFilter = new ExpensesFilterSpecification(user.Id);
            var expenses = await _expenseService.GetAllExpensesAsync(specExpensesFilter);

            var expensesDto = expenses.Select(x => x.ToAppExpenseDto());

            personelViewModel.AppUserDTO = appUserDTO;
            personelViewModel.AppExpenseDTOListe = expensesDto
                .OrderByDescending(x => x.CreationDate)
                .ToList();

            return View(personelViewModel);
        }

        [HttpGet]
        [Route("Personel/Delete/{id}")]
        public async Task<string> Delete(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);

            // faturayı sil
            expense!.Invoce.DeleteFile(_env);

            // öğeyi veritabanından silin
            await _expenseService.DeleteExpenseByIdAsync(expense!.Id);


            // sweetalert için
            TempData["sweetMessage"] = "Harcama isteği başarıyla iptal edildi";
            TempData["sweetType"] = "success";

            return "success";
        }


        // dosya gösterme
        [Route("Personel/ShowFile/{file}")]
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

        [Route("Personel/AvansTalepEt")]
        public async Task<IActionResult> AvansTalepEt()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            await user.UserCompanyIncludeAsync(_companyService);

            var appUserDTO = user.ToAppUserDTO();

            var personelViewModel = new PersonelViewModel();
            personelViewModel.AppUserDTO = appUserDTO;

            return View(personelViewModel);
        }

        [Route("Personel/AvansTalepEt")]
        [HttpPost]
        public async Task<IActionResult> AvansTalepEt(PersonelViewModel personelViewModel)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            await user.UserCompanyIncludeAsync(_companyService);

            _userManager.Users.Include(x => x.Advances)
                .ToList();

            var appUserDTO = user.ToAppUserDTO();


            if (!ModelState.IsValid)
            {
                personelViewModel.AppUserDTO = appUserDTO;
                return View(personelViewModel);
            }


            if (user.Advances.Count == 0 && user.FirstAdvanceDate == null)
                appUserDTO.FirstAdvanceDate = DateTime.Now;


            switch (personelViewModel.AppAdvanceDTO!.Currency)
            {
                case Currency.Dolar:
                    personelViewModel.AppAdvanceDTO.AdvanceAmount = personelViewModel.AppAdvanceDTO.AdvanceAmount * 19.5m;
                    break;

                case Currency.Euro:
                    personelViewModel.AppAdvanceDTO.AdvanceAmount = personelViewModel.AppAdvanceDTO.AdvanceAmount * 21.5m;
                    break;

                default:
                    break;
            }

            appUserDTO.AdvanceSpent += (decimal)personelViewModel.AppAdvanceDTO!.AdvanceAmount!;

            personelViewModel.AppUserDTO = appUserDTO;

            var kurumsalSayisi = user.Advances.Count(x => x.TypeOfAdvance!.Value == TypeOfAdvance.Kurumsal &&
                 x.Approval! == Approval.OnayBekliyor);

            var bireyselSayisi = user.Advances.Count(x => x.TypeOfAdvance!.Value == TypeOfAdvance.Bireysel &&
                 x.Approval! == Approval.OnayBekliyor);

            if (personelViewModel.AppAdvanceDTO.TypeOfAdvance == TypeOfAdvance.Kurumsal)
                kurumsalSayisi++;

            else
                bireyselSayisi++;

            if (kurumsalSayisi > 1)
            {
                ModelState.AddModelError("turError", "Kurumsal avans isteğiniz zaten mevcut");
                return View(personelViewModel);
            }

            if (bireyselSayisi > 1)
            {
                ModelState.AddModelError("turError", "Bireysel avans isteğiniz zaten mevcut");
                return View(personelViewModel);
            }

            if ((user.Salary * 3) < appUserDTO.AdvanceSpent)
            {
                ModelState.AddModelError("avansError", "Talep edebileceğiniz maksimum avans tutarını aştınız");

                return View(personelViewModel);
            }

            await _applicationUserService.UpdateApplicationUserAsync(appUserDTO);

            if (ModelState.IsValid)
            {
                await _advanceService.CreateAdvanceAsync(personelViewModel.AppAdvanceDTO!);

                TempData["sweetAlertMessage"] = "Avans talebiniz yöneticinize başarıyla iletilmiştir.";
                TempData["sweetAlertType"] = "success";

                return View(personelViewModel);
            }

            return View(personelViewModel);
        }

        [Route("Personel/AvansListele")]
        public async Task<IActionResult> AvansListele()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            await user.UserCompanyIncludeAsync(_companyService);

            var appUserDTO = user.ToAppUserDTO();

            var personelViewModel = new PersonelViewModel();

            var advances = await user.AdvancesFilterAsync(_advanceService);

            var advanceDTOList = advances.Select(x => x.ToAppAdvanceDTO());

            personelViewModel.AppUserDTO = appUserDTO;
            personelViewModel.AppAdvanceDTOListe = advanceDTOList
                .OrderByDescending(x => x.CreationDate)
                .ToList();

            return View(personelViewModel);
        }

        [HttpGet]
        [Route("Personel/AvansIptal/{id}")]
        public async Task<string> AvansIptal(int id)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var advance = await _advanceService.GetAdvanceByIdAsync(id);
            user.AdvanceSpent -= (decimal)advance.AdvanceAmount!;
            await _applicationUserService.UpdateApplicationUserAsync(user.ToAppUserDTO());

            // öğeyi veritabanından silin
            await _advanceService.DeleteAdvanceByIdAsync(id);

            // sweetalert için
            TempData["sweetMessage"] = "Avans isteği başarıyla iptal edildi";
            TempData["sweetType"] = "success";

            return "success";
        }
    }
}
