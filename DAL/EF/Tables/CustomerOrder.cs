using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class CustomerOrder
{
    public int CustomerOrderId { get; set; }

    public int UserId { get; set; }

    public int CartId { get; set; }

    public string TranId { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string? SslStatus { get; set; }

    public string? SslczSessionKey { get; set; }

    public string? ValidationId { get; set; }

    public string? BankTranId { get; set; }

    public string? CardType { get; set; }

    public string? RiskLevel { get; set; }

    public string? FailedReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ICollection<CustomerOrderItem> CustomerOrderItems { get; set; } = new List<CustomerOrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
