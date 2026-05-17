using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace The_Book_Matrix.Controllers
{
    public class AdminDashboardController : Controller
    {
        AdminDashboardService adminDashboardService;

        public AdminDashboardController(AdminDashboardService adminDashboardService)
        {
            this.adminDashboardService = adminDashboardService;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var roleName = HttpContext.Session.GetString("RoleName");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (roleName != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var salesSummary = adminDashboardService.GetSalesSummary();

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            return View(salesSummary);
        }
    }
}
