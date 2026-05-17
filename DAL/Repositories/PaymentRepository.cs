using DAL.EF;
using DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class PaymentRepository
    {
        BookShopDBContext db;

        public PaymentRepository(BookShopDBContext db)
        {
            this.db = db;
        }

        
        public void AddOrder(CustomerOrder order)
        {
            db.CustomerOrders.Add(order);
            db.SaveChanges();
        }

      
        public void AddOrderItems(List<CustomerOrderItem> items)
        {
            db.CustomerOrderItems.AddRange(items);
            db.SaveChanges();
        }

       
        public CustomerOrder? GetOrderByTranId(string tranId)
        {
            return db.CustomerOrders.FirstOrDefault(o => o.TranId == tranId);
        }

       
        public void UpdateOrder(CustomerOrder order)
        {
            db.CustomerOrders.Update(order);
            db.SaveChanges();
        }

     
        public bool PaymentExists(string tranId)
        {
            return db.Payments.Any(p => p.TranId == tranId);
        }

        // 6. Save SSLCommerz payment response
        public void AddPayment(Payment payment)
        {
            db.Payments.Add(payment);
            db.SaveChanges();
        }

 
        public void MarkCartAsPaid(int cartId)
        {
            var cart = db.Carts.FirstOrDefault(c => c.CartId == cartId);

            if (cart != null)
            {
                cart.CartStatus = "Paid";
                db.Carts.Update(cart);
                db.SaveChanges();
            }
        }

       
        public List<CustomerOrder> GetAllOrders()
        {
            return db.CustomerOrders.ToList();
        }

        // 9. For Admin Dashboard: get only paid orders
        public List<CustomerOrder> GetPaidOrders()
        {
            return db.CustomerOrders
                .Where(o => o.OrderStatus == "Paid")
                .ToList();
        }

 
        public List<CustomerOrderItem> GetItemsOfPaidOrders()
        {
            var paidOrderIds = db.CustomerOrders
                .Where(o => o.OrderStatus == "Paid")
                .Select(o => o.CustomerOrderId)
                .ToList();

            return db.CustomerOrderItems
                .Where(i => paidOrderIds.Contains(i.CustomerOrderId))
                .ToList();
        }


        public Book? GetBookById(int bookId)
        {
            return db.Books.FirstOrDefault(b => b.BookId == bookId);
        }

        public bool ReduceBookStockForOrder(int customerOrderId, out string message)
        {
            message = "";

            var orderItems = db.CustomerOrderItems
                .Where(i => i.CustomerOrderId == customerOrderId)
                .ToList();

            if (orderItems.Count == 0)
            {
                message = "No order items found.";
                return false;
            }

            // First check stock availability
            foreach (var item in orderItems)
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == item.BookId);

                if (book == null)
                {
                    message = item.BookTitle + " book not found.";
                    return false;
                }

                if (book.Quantity < item.Quantity)
                {
                    message = item.BookTitle + " does not have enough stock.";
                    return false;
                }
            }

            // If all books have enough stock, then reduce stock
            foreach (var item in orderItems)
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == item.BookId);

                if (book != null)
                {
                    book.Quantity = book.Quantity - item.Quantity;

                    // Optional: if stock becomes 0, hide from user side
                    if (book.Quantity == 0)
                    {
                        book.IsActive = false;
                    }

                    db.Books.Update(book);
                }
            }

            db.SaveChanges();

            return true;
        }
    }
}
