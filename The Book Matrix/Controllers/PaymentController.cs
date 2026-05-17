using BLL.Services;
using DAL.EF.Tables;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace The_Book_Matrix.Controllers
{
    public class PaymentController : Controller
    {
        CartService cartService;
        PaymentRepository paymentRepository;
        IConfiguration configuration;

        public PaymentController(
            CartService cartService,
            PaymentRepository paymentRepository,
            IConfiguration configuration)
        {
            this.cartService = cartService;
            this.paymentRepository = paymentRepository;
            this.configuration = configuration;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayNow()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var roleName = HttpContext.Session.GetString("RoleName");
         

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (roleName == "Admin")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            var cart = cartService.GetCartByUserId(userId.Value);

            if (cart == null || cart.Items.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            foreach (var item in cart.Items)
            {
                var book = paymentRepository.GetBookById(item.BookId);

                if (book == null)
                {
                    TempData["ErrorMessage"] = item.Title + " is no longer available.";
                    return RedirectToAction("Index", "Cart");
                }

                if (book.IsActive == false)
                {
                    TempData["ErrorMessage"] = item.Title + " is currently unavailable.";
                    return RedirectToAction("Index", "Cart");
                }

                if (book.Quantity < item.Quantity)
                {
                    TempData["ErrorMessage"] = item.Title + " has only " + book.Quantity + " item(s) available.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            string tranId = "TBM" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string baseUrl = Request.Scheme + "://" + Request.Host;

            CustomerOrder order = new CustomerOrder();

            order.UserId = userId.Value;
            order.CartId = cart.CartId;
            order.TranId = tranId;
            order.TotalAmount = cart.GrandTotal;
            order.Currency = "BDT";
            order.OrderStatus = "Pending";
            order.CreatedAt = DateTime.Now;

            paymentRepository.AddOrder(order);

            List<CustomerOrderItem> orderItems = new List<CustomerOrderItem>();

            foreach (var item in cart.Items)
            {
                CustomerOrderItem orderItem = new CustomerOrderItem();

                orderItem.CustomerOrderId = order.CustomerOrderId;
                orderItem.BookId = item.BookId;
                orderItem.BookTitle = item.Title;
                orderItem.UnitPrice = item.UnitPrice;
                orderItem.Quantity = item.Quantity;
                orderItem.LineTotal = item.LineTotal;

                orderItems.Add(orderItem);
            }

            paymentRepository.AddOrderItems(orderItems);

            var postData = new Dictionary<string, string>
            {
                { "store_id", configuration["SslCommerz:StoreId"] },
                { "store_passwd", configuration["SslCommerz:StorePassword"] },
                { "total_amount", cart.GrandTotal.ToString("0.00") },
                { "currency", "BDT" },
                { "tran_id", tranId },

                { "success_url", baseUrl + "/Payment/Success" },
                { "fail_url", baseUrl + "/Payment/Fail" },
                { "cancel_url", baseUrl + "/Payment/Cancel" },
                { "ipn_url", baseUrl + "/Payment/Ipn" },

                { "cus_name", "Book Matrix Customer" },
                { "cus_email", "customer@example.com" },
                { "cus_add1", "Dhaka" },
                { "cus_city", "Dhaka" },
                { "cus_country", "Bangladesh" },
                { "cus_phone", "01711111111" },

                { "shipping_method", "NO" },
                { "num_of_item", cart.Items.Count.ToString() },
                { "product_name", "Books" },
                { "product_category", "Books" },
                { "product_profile", "general" },

                { "value_a", userId.Value.ToString() },
                { "value_b", cart.CartId.ToString() }
            };

            HttpClient client = new HttpClient();

            var response = client.PostAsync(
                configuration["SslCommerz:SessionApiUrl"],
                new FormUrlEncodedContent(postData)
            ).Result;

            string json = response.Content.ReadAsStringAsync().Result;

            var result = JsonSerializer.Deserialize<SslInitResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result != null &&
                result.status == "SUCCESS" &&
                !string.IsNullOrEmpty(result.GatewayPageURL))
            {
                order.SslczSessionKey = result.sessionkey;
                paymentRepository.UpdateOrder(order);

                return Redirect(result.GatewayPageURL);
            }

            order.OrderStatus = "InitiationFailed";
            order.FailedReason = result != null ? result.failedreason : "SSLCommerz connection failed.";

            paymentRepository.UpdateOrder(order);

            TempData["ErrorMessage"] = order.FailedReason;
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Success()
        {
            string message;
            bool isSuccess = ValidateAndSavePayment(Request.Form, out message);

            ViewBag.IsSuccess = isSuccess;
            ViewBag.Message = message;
            ViewBag.TranId = Request.Form["tran_id"].ToString();

            return View();
        }

        [HttpPost]
        public IActionResult Fail()
        {
            string tranId = Request.Form["tran_id"].ToString();

            MarkOrderFailedOrCancelled(tranId, "Failed", "Payment failed.");

            ViewBag.Message = "Payment failed.";
            ViewBag.TranId = tranId;

            return View();
        }

        [HttpPost]
        public IActionResult Cancel()
        {
            string tranId = Request.Form["tran_id"].ToString();

            MarkOrderFailedOrCancelled(tranId, "Cancelled", "Payment cancelled.");

            ViewBag.Message = "Payment cancelled.";
            ViewBag.TranId = tranId;

            return View();
        }

        [HttpPost]
        public IActionResult Ipn()
        {
            string message;
            bool isSuccess = ValidateAndSavePayment(Request.Form, out message);

            if (isSuccess)
            {
                return Ok("IPN_VALID");
            }

            return Ok("IPN_FAILED");
        }

        private bool ValidateAndSavePayment(IFormCollection form, out string message)
        {
            message = "";

            string tranId = form["tran_id"].ToString();
            string valId = form["val_id"].ToString();

            if (string.IsNullOrEmpty(tranId) || string.IsNullOrEmpty(valId))
            {
                message = "Invalid payment response.";
                return false;
            }

            var order = paymentRepository.GetOrderByTranId(tranId);

            if (order == null)
            {
                message = "Order not found.";
                return false;
            }
            if (order.OrderStatus == "Paid")
            {
                message = "Payment already processed.";
                return true;
            }

            string validationUrl = configuration["SslCommerz:ValidationApiUrl"]
                + "?val_id=" + Uri.EscapeDataString(valId)
                + "&store_id=" + Uri.EscapeDataString(configuration["SslCommerz:StoreId"])
                + "&store_passwd=" + Uri.EscapeDataString(configuration["SslCommerz:StorePassword"])
                + "&v=1&format=json";

            HttpClient client = new HttpClient();
            string json = client.GetStringAsync(validationUrl).Result;

            var validation = JsonSerializer.Deserialize<SslValidationResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (validation == null)
            {
                message = "Payment validation failed.";
                return false;
            }

            decimal paidAmount = 0;

            decimal.TryParse(
                validation.amount,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out paidAmount
            );

            bool validStatus =
                validation.status == "VALID" ||
                validation.status == "VALIDATED";

            bool tranMatched = validation.tran_id == order.TranId;
            bool amountMatched = paidAmount == order.TotalAmount;

            bool currencyMatched =
                validation.currency_type == "BDT" ||
                validation.currency == "BDT";

            bool riskyPayment = validation.risk_level == "1";

            if (validStatus && tranMatched && amountMatched && currencyMatched && !riskyPayment)
            {
                string stockMessage;
                bool stockUpdated = paymentRepository.ReduceBookStockForOrder(
                    order.CustomerOrderId,
                    out stockMessage
                );

                if (stockUpdated == false)
                {
                    order.OrderStatus = "StockProblem";
                    order.SslStatus = validation.status;
                    order.ValidationId = validation.val_id;
                    order.BankTranId = validation.bank_tran_id;
                    order.CardType = validation.card_type;
                    order.RiskLevel = validation.risk_level;
                    order.FailedReason = stockMessage;

                    paymentRepository.UpdateOrder(order);

                    SavePaymentIfNotExists(order, validation, json, "StockProblem");

                    message = stockMessage;
                    return false;
                }

                order.OrderStatus = "Paid";
                order.SslStatus = validation.status;
                order.ValidationId = validation.val_id;
                order.BankTranId = validation.bank_tran_id;
                order.CardType = validation.card_type;
                order.RiskLevel = validation.risk_level;
                order.PaidAt = DateTime.Now;

                paymentRepository.UpdateOrder(order);
                paymentRepository.MarkCartAsPaid(order.CartId);

                SavePaymentIfNotExists(order, validation, json, "Paid");

                message = "Payment successful.";
                return true;
            }

            order.OrderStatus = "ValidationFailed";
            order.SslStatus = validation.status;
            order.ValidationId = validation.val_id;
            order.BankTranId = validation.bank_tran_id;
            order.CardType = validation.card_type;
            order.RiskLevel = validation.risk_level;
            order.FailedReason = "Payment validation failed.";

            paymentRepository.UpdateOrder(order);

            SavePaymentIfNotExists(order, validation, json, "ValidationFailed");

            message = "Payment validation failed.";
            return false;
        }

        private void SavePaymentIfNotExists(
            CustomerOrder order,
            SslValidationResponse validation,
            string gatewayResponse,
            string paymentStatus)
        {
            if (paymentRepository.PaymentExists(order.TranId))
            {
                return;
            }

            Payment payment = new Payment();

            payment.CustomerOrderId = order.CustomerOrderId;
            payment.TranId = order.TranId;
            payment.Amount = order.TotalAmount;
            payment.Currency = "BDT";
            payment.PaymentStatus = paymentStatus;
            payment.ValId = validation.val_id;
            payment.BankTranId = validation.bank_tran_id;
            payment.CardType = validation.card_type;
            payment.RiskLevel = validation.risk_level;
            payment.RiskTitle = validation.risk_title;
            payment.GatewayResponse = gatewayResponse;
            payment.CreatedAt = DateTime.Now;

            paymentRepository.AddPayment(payment);
        }

        private void MarkOrderFailedOrCancelled(string tranId, string status, string reason)
        {
            if (string.IsNullOrEmpty(tranId))
            {
                return;
            }

            var order = paymentRepository.GetOrderByTranId(tranId);

            if (order != null)
            {
                order.OrderStatus = status;
                order.FailedReason = reason;

                paymentRepository.UpdateOrder(order);
            }
        }
    }

    public class SslInitResponse
    {
        public string status { get; set; }
        public string failedreason { get; set; }
        public string GatewayPageURL { get; set; }
        public string sessionkey { get; set; }
    }

    public class SslValidationResponse
    {
        public string status { get; set; }
        public string tran_id { get; set; }
        public string val_id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string currency_type { get; set; }
        public string bank_tran_id { get; set; }
        public string card_type { get; set; }
        public string risk_level { get; set; }
        public string risk_title { get; set; }
    }
}