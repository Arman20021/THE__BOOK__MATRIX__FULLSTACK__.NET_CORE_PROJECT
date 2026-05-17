using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class BookService
    {
            BookRepository bookRepository;

            public BookService(BookRepository bookRepository)
            {
                this.bookRepository = bookRepository;
            }

            public List<BookDto> GetAllBooks()
            {
                var books = bookRepository.GetAllBooks();

                List<BookDto> list = new List<BookDto>();

                foreach (var b in books)
                {
                    BookDto dto = new BookDto();

                    dto.BookId = b.BookId;
                    dto.CategoryId = b.CategoryId;
                    dto.CategoryName = bookRepository.GetCategoryName(b.CategoryId);
                    dto.Title = b.Title;
                    dto.AuthorName = b.AuthorName;
                    dto.Description = b.Description;
                    dto.Price = b.Price;
                    dto.Quantity = b.Quantity;
                    dto.ImagePath = b.ImagePath;
                    dto.IsActive = b.IsActive;

                    list.Add(dto);
                }

                return list;
            }

            public List<BookDto> GetActiveBooks()
            {
                var books = bookRepository.GetActiveBooks();

                List<BookDto> list = new List<BookDto>();

                foreach (var b in books)
                {
                    BookDto dto = new BookDto();

                    dto.BookId = b.BookId;
                    dto.CategoryId = b.CategoryId;
                    dto.CategoryName = bookRepository.GetCategoryName(b.CategoryId);
                    dto.Title = b.Title;
                    dto.AuthorName = b.AuthorName;
                    dto.Description = b.Description;
                    dto.Price = b.Price;
                    dto.Quantity = b.Quantity;
                    dto.ImagePath = b.ImagePath;
                    dto.IsActive = b.IsActive;

                    list.Add(dto);
                }

                return list;
            }

            public BookDto? GetBookById(int id)
            {
                var b = bookRepository.GetBookById(id);

                if (b == null)
                {
                    return null;
                }

                BookDto dto = new BookDto();

                dto.BookId = b.BookId;
                dto.CategoryId = b.CategoryId;
                dto.CategoryName = bookRepository.GetCategoryName(b.CategoryId);
                dto.Title = b.Title;
                dto.AuthorName = b.AuthorName;
                dto.Description = b.Description;
                dto.Price = b.Price;
                dto.Quantity = b.Quantity;
                dto.ImagePath = b.ImagePath;
                dto.IsActive = b.IsActive;

                return dto;
            }

            public void AddBook(BookDto dto)
            {
                Book book = new Book();

                book.CategoryId = dto.CategoryId;
                book.Title = dto.Title;
                book.AuthorName = dto.AuthorName;
                book.Description = dto.Description;
                book.Price = dto.Price;
                book.Quantity = dto.Quantity;
                book.ImagePath = dto.ImagePath;
                book.IsActive = true;


                bookRepository.AddBook(book);
            }

            public void UpdateBook(BookDto dto)
            {
                var book = bookRepository.GetBookById(dto.BookId);

                if (book != null)
                {
                    book.CategoryId = dto.CategoryId;
                    book.Title = dto.Title;
                    book.AuthorName = dto.AuthorName;
                    book.Description = dto.Description;
                    book.Price = dto.Price;
                    book.Quantity = dto.Quantity;
                    book.ImagePath = dto.ImagePath;
                    book.IsActive = dto.IsActive;


                    bookRepository.UpdateBook(book);
                }
            }

            public void DeleteBook(int id)
            {
                bookRepository.DeleteBook(id);
            }

        public List<BookDto> SearchBooks(
      string searchText,
     int? categoryId,
     decimal? minPrice,
     decimal? maxPrice,
      string sortBy)
        {
            var books = bookRepository.SearchBooks(
                searchText,
                categoryId,
                minPrice,
                maxPrice,
                sortBy
            );

            List<BookDto> bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                BookDto dto = new BookDto();

                dto.BookId = book.BookId;
                dto.CategoryId = book.CategoryId;
                dto.Title = book.Title;
                dto.AuthorName = book.AuthorName;
                dto.Description = book.Description;
                dto.Price = book.Price;
                dto.Quantity = book.Quantity;
                dto.ImagePath = book.ImagePath;
                dto.IsActive = book.IsActive;

                if (book.Category != null)
                {
                    dto.CategoryName = book.Category.CategoryName;
                }

                bookDtos.Add(dto);
            }

            return bookDtos;
        }


    }
}
