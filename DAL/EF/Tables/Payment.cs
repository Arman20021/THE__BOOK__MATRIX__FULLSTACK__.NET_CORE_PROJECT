using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int CustomerOrderId { get; set; }

    public string TranId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string? ValId { get; set; }

    public string? BankTranId { get; set; }

    public string? CardType { get; set; }

    public string? RiskLevel { get; set; }

    public string? RiskTitle { get; set; }

    public string? GatewayResponse { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CustomerOrder CustomerOrder { get; set; } = null!;
}
