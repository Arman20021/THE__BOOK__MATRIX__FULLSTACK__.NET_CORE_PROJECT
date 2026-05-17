using DAL.EF;
using DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CartRepository
    {

        BookShopDBContext db;

        public CartRepository(BookShopDBContext db)
        {
            this.db = db;
        }

        public Cart? GetActiveCartByUserId(int userId)
        {
            return db.Carts.FirstOrDefault(c =>
                c.UserId == userId &&
                c.CartStatus == "Active");
        }

        public void AddCart(Cart cart)
        {
            db.Carts.Add(cart);
            db.SaveChanges();
        }

        public CartItem? GetCartItem(int cartId, int bookId)
        {
            return db.CartItems.FirstOrDefault(ci =>
                ci.CartId == cartId &&
                ci.BookId == bookId);
        }

        public CartItem? GetCartItemById(int cartItemId)
        {
            return db.CartItems.FirstOrDefault(ci =>
                ci.CartItemId == cartItemId);
        }

        public void AddCartItem(CartItem item)
        {
            db.CartItems.Add(item);
            db.SaveChanges();
        }

        public void UpdateCartItem(CartItem item)
        {
            db.CartItems.Update(item);
            db.SaveChanges();
        }

        public List<CartItem> GetCartItemsByCartId(int cartId)
        {
            return db.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToList();
        }

        public void RemoveCartItem(int cartItemId)
        {
            var item = db.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }
        }

        public Book? GetBookById(int bookId)
        {
            return db.Books.FirstOrDefault(b => b.BookId == bookId);
        }
    }
}
