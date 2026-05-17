using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }

        public int BookId { get; set; }

        public string Title { get; set; } = "";

        public string? ImagePath { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}
