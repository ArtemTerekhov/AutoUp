using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoUp.Models;
using AutoUp.ViewModels;

namespace AutoUp.Controllers
{
    public class PaymentsController : Controller
    {

        private AutoUpContext db;
        //private const string API_URL = "https://api-sandbox.coingate.com/v2/";
        private const string API_URL = "https://api.coingate.com/v2/";
        public PaymentsController(AutoUpContext context)
        {
            db = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentsViewModel model)
        {
            ViewBag.Message = "";

            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

                CultureInfo culture = new CultureInfo("en-US");

                var baseAmount = float.Parse(model.Amount, culture);

                Order newOrder = new Order
                {
                    UserId = user.UserId,
                    PriceAmount = baseAmount,
                    OrderDate = DateTime.Now,
                    Status = "New"
                };

                db.Orders.Add(newOrder);

                await db.SaveChangesAsync();

                var orderId = newOrder.OrderId;

                var parameters = await db.Parameters.FirstOrDefaultAsync();
                var authToken = parameters.AuthToken;
                var payCurrency = parameters.ReceiveCurrency;

                var form = new List<KeyValuePair<string, string>>();

                form.Add(new KeyValuePair<string, string>("order_id", orderId.ToString()));
                form.Add(new KeyValuePair<string, string>("price_amount", model.Amount));
                form.Add(new KeyValuePair<string, string>("price_currency", parameters.PriceCurrency));
                form.Add(new KeyValuePair<string, string>("receive_currency", payCurrency));
                form.Add(new KeyValuePair<string, string>("title", parameters.Title));
                form.Add(new KeyValuePair<string, string>("description", parameters.Description));
                form.Add(new KeyValuePair<string, string>("callback_url", parameters.CallbackUrl));
                form.Add(new KeyValuePair<string, string>("cancel_url", parameters.CancelUrl));
                form.Add(new KeyValuePair<string, string>("success_url", parameters.SuccessUrl));
                form.Add(new KeyValuePair<string, string>("token", parameters.Token));

                var request = new HttpRequestMessage(HttpMethod.Post, API_URL + "orders") { Content = new FormUrlEncodedContent(form) };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("Accept", "text/html, application/json, text/javascript, application/xhtml+xml, application/xml;q=0.9, */*;q=0.8");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");
                request.Headers.Add("Authorization", "Token " + authToken);

                HttpClient httpClient = new HttpClient();

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<CreateResponse>(responseData);

                    return RedirectToAction("Checkout", new
                    {
                        coingateOrderId = result.id,
                        orderId = result.order_id,
                        authToken = authToken,
                        payCurrency = payCurrency,
                        lightningNetwork = result.lightning_network
                    });
                }
            }

            ViewBag.Message = "Произошла ошибка";

            return View("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout(int coingateOrderId, string orderId, string payCurrency,
            bool lightningNetwork, string authToken)
        {
            var form = new List<KeyValuePair<string, string>>();

            form.Add(new KeyValuePair<string, string>("id", coingateOrderId.ToString()));
            form.Add(new KeyValuePair<string, string>("pay_currency", payCurrency));
            form.Add(new KeyValuePair<string, string>("lightning_network", lightningNetwork.ToString().ToLower()));

            var request = new HttpRequestMessage(HttpMethod.Post, API_URL + "orders/" + coingateOrderId.ToString()
                + "/checkout")
            { Content = new FormUrlEncodedContent(form) };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Headers.Add("Accept", "text/html, application/json, text/javascript, application/xhtml+xml, application/xml;q=0.9, */*;q=0.8");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");
            request.Headers.Add("Authorization", "Token " + authToken);

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<CheckoutResponse>(responseData);

                Order order = await db.Orders.Where(o => o.OrderId == Convert.ToInt32(orderId)).FirstOrDefaultAsync();

                order.Status = "Pending";

                db.Orders.Update(order);

                await db.SaveChangesAsync();

                ViewBag.Message = "Отправьте биткойны на адрес " + result.payment_address
                    + ". Сумма к оплате - " + result.pay_amount; // result.receive_amount

                return View();
            }

            ViewBag.Message = "Произошла ошибка";

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CoingateCallback(string id, string order_id, string status, string price_amount,
            string price_currency, string receive_currency, string receive_amount,
            string pay_amount, string pay_currency, string underpaid_amount,
            string overpaid_amount, string is_refundable, string created_at, string token)
        //    public async Task<IActionResult> CoingateCallback(string responseData)
        {
            /*
              status = "paid";
              receive_amount = "0.001101";
              order_id = "1";
            */

            //   var result = JsonSerializer.Deserialize<CallbackResponse>(responseData);

            //   if (result.status.ToLower() == "paid")

            Order order = db.Orders.Where(o => o.OrderId == Convert.ToInt32(order_id)).FirstOrDefault();

            var userId = order.UserId;

            if (status.ToLower() == "paid")
            {
                var link = "https://api.cryptonator.com/api/ticker/btc-eur";
                var request = new HttpRequestMessage(HttpMethod.Get, link);
                request.Headers.Add("Accept", "text/html, application/xhtml+xml, application/xml;q=0.9, */*;q=0.8");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();

                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var cryptoData = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonSerializer.Deserialize<Cryptonizer>(cryptoData);

                    CultureInfo culture = new CultureInfo("en-US");

                    var incomeInEuro = Math.Round(float.Parse(responseModel.ticker.price, culture)
                        * float.Parse(pay_amount, culture), 2);

                    User user = await db.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();

                    var balance = user.Balance;
                    var newBalance = balance + Convert.ToDecimal(incomeInEuro, culture);

                    user.Balance = newBalance;

                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                }
            }

            order.Status = status;

            db.Orders.Update(order);

            db.SaveChanges();

            return Content("Ok");
        }

        [HttpGet]
        public IActionResult Done()
        {
            return View();
        }
    }
}
