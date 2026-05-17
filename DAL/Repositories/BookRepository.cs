using DAL.EF;
using DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class BookRepository
    {
        BookShopDBContext db;

        public BookRepository(BookShopDBContext db)
        {
            this.db = db;
        }

        public List<Book> GetAllBooks()
        {
            return db.Books.ToList();
        }

        public List<Book> GetActiveBooks()
        {
            return db.Books
                .Where(b => b.IsActive == true)
                .ToList();
        }

        public Book? GetBookById(int id)
        {
            return db.Books.FirstOrDefault(b => b.BookId == id);
        }

        public void AddBook(Book book)
        {
            db.Books.Add(book);
            db.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            if (book.IsActive == false)
            {
                var cartItems = db.CartItems
                    .Where(ci => ci.BookId == book.BookId)
                    .ToList();

                if (cartItems.Count > 0)
                {
                    db.CartItems.RemoveRange(cartItems);
                }
            }

            db.Books.Update(book);
            db.SaveChanges();
        }

        public void DeleteBook(int id)
        {
            var book = db.Books.FirstOrDefault(b => b.BookId == id);

            if (book != null)
            {
             
                var cartItems = db.CartItems
                    .Where(ci => ci.BookId == id)
                    .ToList();

                if (cartItems.Count > 0)
                {
                    db.CartItems.RemoveRange(cartItems);
                }

            
                book.IsActive = false;
            

                db.Books.Update(book);
                db.SaveChanges();
            }
        }

        public string GetCategoryName(int categoryId)
        {
            var category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return "";
            }

            return category.CategoryName;
        }


        //for book seraching
        public List<Book> SearchBooks(
    string searchText,
    int? categoryId,
    decimal? minPrice,
    decimal? maxPrice,
    string sortBy)
        {
            var query = db.Books
                .Include(b => b.Category)
                .Where(b => b.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                query = query.Where(b =>
                    b.Title.Contains(searchText) ||
                    b.AuthorName.Contains(searchText) ||
                    b.Description.Contains(searchText) ||
                    b.Category.CategoryName.Contains(searchText)
                );
            }

            if (categoryId != null && categoryId > 0)
            {
                query = query.Where(b => b.CategoryId == categoryId);
            }

            if (minPrice != null)
            {
                query = query.Where(b => b.Price >= minPrice);
            }

            if (maxPrice != null)
            {
                query = query.Where(b => b.Price <= maxPrice);
            }

            if (sortBy == "name_asc")
            {
                query = query.OrderBy(b => b.Title);
            }
            else if (sortBy == "name_desc")
            {
                query = query.OrderByDescending(b => b.Title);
            }
            else if (sortBy == "price_asc")
            {
                query = query.OrderBy(b => b.Price);
            }
            else if (sortBy == "price_desc")
            {
                query = query.OrderByDescending(b => b.Price);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookId);
            }

            return query.ToList();
        }
    }
}
