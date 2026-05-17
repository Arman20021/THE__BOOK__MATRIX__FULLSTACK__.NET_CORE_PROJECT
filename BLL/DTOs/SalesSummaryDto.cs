using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class SalesSummaryDto
    {
        public decimal TotalSales { get; set; }
        public decimal TodaysSales { get; set; }
        public decimal MonthlySales { get; set; }

        public int TotalPaidOrders { get; set; }
        public int PendingOrders { get; set; }
        public int FailedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int TotalBooksSold { get; set; }

        public List<MostSoldBookDto> MostSoldBooks { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; }

        public SalesSummaryDto()
        {
            MostSoldBooks = new List<MostSoldBookDto>();
            RecentOrders = new List<RecentOrderDto>();
        }
    }

    public class MostSoldBookDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class RecentOrderDto
    {
        public int CustomerOrderId { get; set; }
        public string TranId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
