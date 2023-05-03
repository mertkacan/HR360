using Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Diagnostics;
using Web.Areas.Identity.Data;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Yonetici"))
                return RedirectToAction("Index", "Yonetici");

            else if (User.IsInRole("Personel"))
                return RedirectToAction("Index", "Personel");

            else if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");

            return View();
        }

        [HttpPost]
        public async Task<string> Index(LoginViewModel loginModel)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("loginError", "Email veya şifre hatalı!");

                return "";
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, loginModel.Password);

            if (!user.EmailConfirmed && checkPassword)
                return "PersonelUpdate";

            var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, true, lockoutOnFailure: false);
            var rol = await _userManager.GetRolesAsync(user!);

            if (result.Succeeded)
            {
                if (rol.Any(x => x == "Yonetici"))
                    return "Yonetici";

                else if (rol.Any(x => x == "Personel"))
                    return "Personel";

                else if (rol.Any(x => x == "Admin"))
                    return "Admin";
            }

            else
                return "";

            ModelState.AddModelError("loginError", "Email veya şifre hatalı!");

            return "";
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [Route("AccountUpdate/{email}")]
        public async Task<IActionResult> Update(string email)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
            UpdateAccountViewModel updateAccountViewModel = new UpdateAccountViewModel
            {
                Email = email,
                NameSurname = user!.Name + " " + user.Surname
            };

            return View(updateAccountViewModel);
        }

        [HttpPost]
        [Route("AccountUpdate/{email}")]
        public async Task<IActionResult> Update(UpdateAccountViewModel updateAccountViewModel)
        {
            if (ModelState.IsValid)
            {
                // model geçerliyse işlemler yapılır
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == updateAccountViewModel.Email);
                var passwordCheck = await _userManager.CheckPasswordAsync(user!, updateAccountViewModel.CurrentPassword.Trim());

                if (!passwordCheck)
                {
                    // ModelState kullanılarak hata eklenir
                    ModelState.AddModelError("pwerror", "Şifrenizi yanlış girdiniz");
                }

                var result = await _userManager.ChangePasswordAsync(user!, updateAccountViewModel.CurrentPassword.Trim(), updateAccountViewModel.Password.Trim());

                if (result.Succeeded)
                {
                    user!.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Logout");
                }
            }

            // ModelState kullanılarak hatalar gösterilir
            ModelState.AddModelError("pwformat", "Şifreniz büyük harf,rakam ve özel karakter içermelidir");
            return View(updateAccountViewModel);
        }
    }
}
