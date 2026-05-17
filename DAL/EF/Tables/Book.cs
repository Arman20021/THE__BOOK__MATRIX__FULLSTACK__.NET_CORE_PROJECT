using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class Book
{
    public int BookId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string AuthorName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string ImagePath { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;
}
