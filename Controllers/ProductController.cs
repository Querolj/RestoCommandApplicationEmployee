using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using RestoCommand.Models;
using RestoCommandApplicationEmployee.Utils;
using System.Net.Http.Headers;

namespace RestoCommandApplicationEmployee.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _client;

        public ProductController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(ApiUrl.BaseUrl);
        }

        public IActionResult Index()
        {
            List<Product>? products = new List<Product>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "product/getall").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(data);
            }
            return View(products);
        }

        public string Welcome()
        {
            return "This is the Welcome action method...";

        }

        public IActionResult Details(string? id)
        {
            return GetProductView(id);
        }

        public IActionResult Edit(string? id)
        {
            return GetProductView(id);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string? id)
        {
            return GetProductView(id);
        }

        public IActionResult Create(string? id)
        {
            Product product = new Product();
            product.Id = Guid.NewGuid().ToString();
            return View(product);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("Id,Name,Description,Price,Type,ImageUrl")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound("Product with id " + id + " not found");
            }

            HttpResponseMessage response = null;
            if (ModelState.IsValid)
            {
                Dictionary<string, string?> query = new Dictionary<string, string?>
                {
                    { "Id", product.Id },
                    { "Name", product.Name },
                    { "Description", product.Description },
                    { "Price", product.Price.ToString() },
                    { "Type", product.Type.ToString() },
                    { "ImageUrl", product.ImageUrl }
                };

                string? requestUri = QueryHelpers.AddQueryString(_client.BaseAddress + "product/edit/" + id, query);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, requestUri);
                response = _client.SendAsync(request).Result;
            }
            else
            {
                return BadRequest(ModelState);
            }

            if (response == null)
            {
                return StatusCode(500, "Response is null");
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                int statusCode = (int)response.StatusCode;
                return StatusCode(statusCode, response.ReasonPhrase);
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Description,Price,Type,ImageUrl")] Product product)
        {
            HttpResponseMessage response = null;
            if (ModelState.IsValid)
            {
                Dictionary<string, string?> query = new Dictionary<string, string?>
                {
                    { "Id", product.Id },
                    { "Name", product.Name },
                    { "Description", product.Description },
                    { "Price", product.Price.ToString() },
                    { "Type", product.Type.ToString() },
                    { "ImageUrl", product.ImageUrl }
                };

                string? requestUri = QueryHelpers.AddQueryString(_client.BaseAddress + "product/add", query);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                response = _client.SendAsync(request).Result;
            }
            else
            {
                return BadRequest(ModelState);
            }

            if (response == null)
            {
                return StatusCode(500, "Response is null");
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                int statusCode = (int)response.StatusCode;
                return StatusCode(statusCode, response.ReasonPhrase);
            }
        }

        private IActionResult GetProductView(string? id)
        {
            if (id == null)
            {
                return NotFound("Product with id " + id + " not found");
            }

            Product? product;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "product/get/" + id).Result;

            if (response == null)
            {
                return StatusCode(500, "Response is null");
            }

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(data);
            }
            else
            {
                int statusCode = (int)response.StatusCode;
                return StatusCode(statusCode, response.ReasonPhrase);
            }

            if (product == null)
            {
                return NotFound(nameof(product) + " with id " + id + " is null");
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "product/delete/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                int statusCode = (int)response.StatusCode;
                //return RedirectToAction(nameof(Index));

                return StatusCode(statusCode, response.ReasonPhrase);
            }
        }
    }
}
