using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class CustomerOrderItem
{
    public int CustomerOrderItems { get; set; }

    public int CustomerOrderId { get; set; }

    public int BookId { get; set; }

    public string BookTitle { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }

    public virtual CustomerOrder CustomerOrder { get; set; } = null!;
}
