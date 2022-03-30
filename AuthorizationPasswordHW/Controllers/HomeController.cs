using AuthorizationPasswordHW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationPasswordHW.Data;
using AuthorizationPasswordHW.Web.Models;

namespace AuthorizationPasswordHW.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdDB;Integrated Security=true;";
        public IActionResult Index()
        {
            var repo = new AdRepo(_connectionString);
            var ads = repo.GetAllAds();
            return View(new AdsViewModel { Ads = ads });
        }

        [Authorize]
        public IActionResult SubmitAd(Ad ad)
        {
            var repo = new AdRepo(_connectionString);
            User user = repo.GetByEmail(User.Identity.Name);
            ad.UserId = user.Id;
            ad.Name = user.Name;
            repo.AddAd(ad);
            return Redirect("/");
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            var adRepo = new AdRepo(_connectionString);
            adRepo.DeleteAd(id);
            return Redirect("/");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var userRepo = new AdRepo(_connectionString);
            var user = userRepo.GetByEmail(User.Identity.Name);
            var ads = userRepo.GetAdsById(user.Id);
            return View(new AdsViewModel { Ads = ads });
        }
    }
}
