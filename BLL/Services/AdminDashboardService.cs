using BLL.DTOs;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AdminDashboardService
    {
        PaymentRepository paymentRepository;

        public AdminDashboardService(PaymentRepository paymentRepository)
        {
            this.paymentRepository = paymentRepository;
        }

        public SalesSummaryDto GetSalesSummary()
        {
            var allOrders = paymentRepository.GetAllOrders();
            var paidOrders = paymentRepository.GetPaidOrders();
            var paidItems = paymentRepository.GetItemsOfPaidOrders();

            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            DateTime monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime nextMonthStart = monthStart.AddMonths(1);

            SalesSummaryDto summary = new SalesSummaryDto();

            summary.TotalSales = paidOrders.Sum(o => o.TotalAmount);

            summary.TodaysSales = paidOrders
                .Where(o => o.PaidAt != null &&
                            o.PaidAt.Value >= today &&
                            o.PaidAt.Value < tomorrow)
                .Sum(o => o.TotalAmount);

            summary.MonthlySales = paidOrders
                .Where(o => o.PaidAt != null &&
                            o.PaidAt.Value >= monthStart &&
                            o.PaidAt.Value < nextMonthStart)
                .Sum(o => o.TotalAmount);

            summary.TotalPaidOrders = paidOrders.Count;

            summary.PendingOrders = allOrders
                .Count(o => o.OrderStatus == "Pending");

            summary.FailedOrders = allOrders
                .Count(o => o.OrderStatus == "Failed" ||
                            o.OrderStatus == "ValidationFailed" ||
                            o.OrderStatus == "InitiationFailed" ||
                            o.OrderStatus == "StockProblem");

            summary.CancelledOrders = allOrders
                .Count(o => o.OrderStatus == "Cancelled");

            summary.TotalBooksSold = paidItems.Sum(i => i.Quantity);

            summary.MostSoldBooks = paidItems
                .GroupBy(i => new { i.BookId, i.BookTitle })
                .Select(g => new MostSoldBookDto
                {
                    BookId = g.Key.BookId,
                    BookTitle = g.Key.BookTitle,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalAmount = g.Sum(x => x.LineTotal)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToList();

            summary.RecentOrders = allOrders
                .OrderByDescending(o => o.CustomerOrderId)
                .Take(10)
                .Select(o => new RecentOrderDto
                {
                    CustomerOrderId = o.CustomerOrderId,
                    TranId = o.TranId,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    CreatedAt = o.CreatedAt,
                    PaidAt = o.PaidAt
                })
                .ToList();

            return summary;
        }
    }
}
