using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace The_Book_Matrix.Controllers
{
    public class AuthController : Controller
    {
        AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = authService.Register(dto);

            if (result.IsSuccess == false)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Invalid verification link.";
                return View();
            }

            var result = authService.VerifyEmail(token);

            if (result.IsSuccess == true)
            {
                ViewBag.SuccessMessage = result.Message;
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = authService.Login(dto);

            if (result.IsSuccess == false)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetString("FullName", result.FullName);
            HttpContext.Session.SetString("Email", result.Email);
            HttpContext.Session.SetString("RoleName", result.RoleName);

            if (result.RoleName == "Admin")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
