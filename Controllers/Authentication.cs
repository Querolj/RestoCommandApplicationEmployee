using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestoCommand.Models.Authentication;
using RestoCommandApplicationEmployee.Utils;

namespace RestoCommandApplicationEmployee.Controllers
{
    public class Authentication : Controller
    {
        private readonly HttpClient _client;

        public Authentication()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(ApiUrl.BaseUrl);
        }

        public IActionResult Login()
        {
            UserInfo userInfo = new UserInfo();
            return View(userInfo);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Email,Password")] UserInfo userInfo)
        {
            userInfo.Id = Guid.NewGuid().ToString();
            if (ModelState.IsValid)
            {
                userInfo.Role = UserRoles.User;
                string url = _client.BaseAddress + "Authenticate/register";
                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                try
                {
                    string json = JsonConvert.SerializeObject(userInfo);
                    HttpResponseMessage response = _client.PostAsync(url, new StringContent(json, System.Text.Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        int statusCode = (int)response.StatusCode;
                        return StatusCode(statusCode, response.ReasonPhrase + "\nurl : " + url + "\njson " + json);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }

            return View(userInfo);
        }

        public IActionResult Register()
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Id = Guid.NewGuid().ToString();

            return View(userInfo);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("Email,Password")] UserInfo userInfo)
        {
            userInfo.Id = Guid.NewGuid().ToString();
            if (ModelState.IsValid)
            {
                userInfo.Role = UserRoles.User;
                string url = _client.BaseAddress + "Authenticate/login";
                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                try
                {
                    string json = JsonConvert.SerializeObject(userInfo);
                    HttpResponseMessage response = _client.PostAsync(url, new StringContent(json, System.Text.Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        int statusCode = (int)response.StatusCode;
                        return StatusCode(statusCode, response.ReasonPhrase + "\nurl : " + url + "\njson " + json);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }

            return View(userInfo);
        }
    }
}

