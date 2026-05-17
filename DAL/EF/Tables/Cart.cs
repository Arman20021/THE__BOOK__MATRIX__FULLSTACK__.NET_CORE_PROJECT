using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public string CartStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();

    public virtual User User { get; set; } = null!;
}
