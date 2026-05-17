using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace The_Book_Matrix.Controllers
{
    public class AdminBookController : Controller
    {
        BookService bookService;
        CategoryService categoryService;

        public AdminBookController(BookService bookService, CategoryService categoryService)
        {
            this.bookService = bookService;
            this.categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var roleName = HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var books = bookService.GetAllBooks();

            return View(books);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var roleName = HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Categories = categoryService.GetActiveCategories();

            return View(new BookDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookDto dto)
        {
            var roleName = HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = categoryService.GetActiveCategories();
                return View(dto);
            }

            bookService.AddBook(dto);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var roleName = HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var book = bookService.GetBookById(id);

            if (book == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Categories = categoryService.GetActiveCategories();

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookDto dto)
        {
            var roleName = HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = categoryService.GetActiveCategories();
                return View(dto);
            }

            bookService.UpdateBook(dto);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var roleName = HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            bookService.DeleteBook(id);

            return RedirectToAction("Index");
        }
    }
}
