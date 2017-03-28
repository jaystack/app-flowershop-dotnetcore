using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using App.Flowershop.ViewModels;
using Microsoft.AspNetCore.Cors;
using System.Net;
using Microsoft.AspNetCore.Http;
using SystemEndpoints;

namespace App.Flowershop.Controllers
{
    public class HomeController : Controller
    {
        private const string fs_cart = "fs_cart";
        private readonly IOptions<Config> config;
        private IStore store;

        public HomeController(IOptions<Config> optionsAccessor)
        {
            config = optionsAccessor;
            store = new Store(config.Value.hosts);
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.ItemsContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Items), "/checkout").Result;
            vm.SummaryContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Cart), "/cart/summary").Result;

            return View("Index", vm);
        }

        [HttpGet("/category/{catName}")]
        public IActionResult Category(string catName)
        {
            var vm = new IndexViewModel();

            vm.ItemsContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Items), "/category/" + catName).Result;
            vm.SummaryContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Cart), "/cart/summary").Result;

            return View("Index", vm);
        }

        [HttpGet("/checkout")]
        [HttpGet("cart/checkout")]
        public IActionResult Checkout()
        {
            var vm = new IndexViewModel();

            vm.ItemsContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Items), "/checkout").Result;
            vm.CheckoutContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Cart), "/cart/checkout").Result;

            return View("Index", vm);
        }

        [HttpPost("/checkout")]
        [HttpPost("cart/checkout")]
        public IActionResult Checkout(string customerName, string customerAddress)
        {
            var vm = new IndexViewModel();

            vm.ItemsContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Items), "/checkout").Result;
            var result = post("http://" + store.GetServiceAddress(config.Value.Apps.Cart), "/cart/checkout", new Dictionary<string, string>()
            {
                { "customerName", customerName },
                { "customerAddress", customerAddress },
                { fs_cart, HttpContext.Session.GetString(fs_cart) }
            });

            Console.WriteLine(result.StatusCode);
            Console.WriteLine(HttpStatusCode.Created);

            if (result.StatusCode == HttpStatusCode.Created)
                vm.SummaryContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Cart), "/cart/summary").Result;

            return View("Index", vm);
        }

        [HttpGet("/cart/add/{id}")]
        public async Task<IActionResult> Add(string id, string ret)
        {
            var vm = new IndexViewModel();

            await getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Cart), "/cart/add/" + id);

            if (!String.IsNullOrEmpty(ret) && ret.StartsWith("/category/"))
                return Redirect(ret);
            else
            {
                vm.ItemsContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Items), "/checkout").Result;
                vm.SummaryContent = getResponseAsync("http://" + store.GetServiceAddress(config.Value.Apps.Cart), "/cart/summary").Result;

                return View("Index", vm);
            }
        }

        private Task<string> getResponseAsync(string baseUrl, string url)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            if (HttpContext.Session.Keys.Contains(fs_cart))
            {
                cookies.Add(new Uri(baseUrl), new Cookie(fs_cart, HttpContext.Session.GetString(fs_cart)));
            }

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    IEnumerable<Cookie> responseCookies = cookies.GetCookies(client.BaseAddress).Cast<Cookie>();

                    var fscart = responseCookies.FirstOrDefault(p => p.Name == fs_cart);

                    if (fscart != null)
                    {
                        HttpContext.Session.SetString(fscart.Name, fscart.Value);
                    }

                    return response.Content.ReadAsStringAsync();
                }
                catch
                {
                    var resp = new StringContent("Service not available: " + baseUrl);
                    return resp.ReadAsStringAsync();
                }
            }
        }

        private HttpResponseMessage post(string baseUrl, string url, Dictionary<string, string> values)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            if (HttpContext.Session.Keys.Contains(fs_cart))
            {
                cookies.Add(new Uri(baseUrl), new Cookie(fs_cart, HttpContext.Session.GetString(fs_cart)));
            }

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(baseUrl);
                var content = new FormUrlEncodedContent(values.ToArray());
                var result = client.PostAsync(url, content).Result;

                IEnumerable<Cookie> responseCookies = cookies.GetCookies(client.BaseAddress).Cast<Cookie>();

                var fscart = responseCookies.FirstOrDefault(p => p.Name == fs_cart);

                if (fscart != null)
                {
                    HttpContext.Session.SetString(fscart.Name, fscart.Value);
                }

                return result;
            }
        }
    }
}
