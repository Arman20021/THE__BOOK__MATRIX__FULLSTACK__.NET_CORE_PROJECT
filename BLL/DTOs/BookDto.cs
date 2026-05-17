using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace BLL.DTOs
{
    public class BookDto
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = "";

        [Required(ErrorMessage = "Book title is required")]
        public string Title { get; set; } = "";

        public string? AuthorName { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        public string? ImagePath { get; set; }

        public bool IsActive { get; set; }
    }
}
