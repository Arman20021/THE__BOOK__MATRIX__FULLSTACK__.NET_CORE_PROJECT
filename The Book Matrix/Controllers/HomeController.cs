using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using The_Book_Matrix.Models;

namespace The_Book_Matrix.Controllers
{

    public class HomeController : Controller
    {
        BookService bookService;
        CategoryService categoryService;

        public HomeController(BookService bookService, CategoryService categoryService)
        {
            this.bookService = bookService;
            this.categoryService = categoryService;
        }

        public IActionResult Index(
            string searchText,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string sortBy)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var roleName = HttpContext.Session.GetString("RoleName");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (roleName == "Admin")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            if (categoryId == 0)
            {
                categoryId = null;
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            ViewBag.RoleName = roleName;

            ViewBag.Categories = categoryService.GetActiveCategories();

            ViewBag.SearchText = searchText;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;

            var books = bookService.SearchBooks(
                searchText,
                categoryId,
                minPrice,
                maxPrice,
                sortBy
            );

            return View(books);
        }
    }
}
