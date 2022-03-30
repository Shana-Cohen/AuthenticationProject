using AuthorizationPasswordHW.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AuthorizationPasswordHW.Web.Controllers
{
    public class Account : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdDB;Integrated Security=true;";
        public IActionResult LogIn()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            var repo = new AdRepo(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Login Failed!";
                Redirect("/account/login");
            }
            var claims = new List<Claim> {
            new Claim("user", email)};
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/home/myaccount");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var repo = new AdRepo(_connectionString);
            user.Id = repo.AddUser(user, password);
            return Redirect("/login");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/index");
        }
    }
}
