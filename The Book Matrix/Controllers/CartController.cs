using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace The_Book_Matrix.Controllers
{
    public class CartController : Controller
    {
        CartService cartService;

        public CartController(CartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int bookId, int quantity)
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

            var result = cartService.AddToCart(userId.Value, bookId, quantity);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index", "Home");
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

            var cart = cartService.GetCartByUserId(userId.Value);

            return View(cart);
        }

        public IActionResult Remove(int id)
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

            cartService.RemoveItem(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
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

            var result = cartService.UpdateQuantity(cartItemId, quantity);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
