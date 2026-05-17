using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace The_Book_Matrix.Controllers
{
    public class BookController : Controller
    {

        BookService bookService;

        public BookController(BookService bookService)
        {
            this.bookService = bookService;
        }

        public IActionResult Index()
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

            var books = bookService.GetActiveBooks();

            return View(books);
        }
    }
}
