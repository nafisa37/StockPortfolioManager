using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StockPortfolioApp.Models;
using Microsoft.AspNetCore.Authentication;

namespace StockPortfolioApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;  // Injecting DbContext for database access

        // Constructor to inject dependencies
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutMe()
        {
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View();  // Views/Home/Profile.cshtml
        }

        [HttpPost]
        [Authorize] // only if you're logged in
        public IActionResult UpdatePassword(string currentPassword, string newPassword)
        {
            var userEmail = User.Identity.Name;  // gets logged-in user's email
            var user = _context.UserAccounts.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View("Profile");
            }
            Console.WriteLine($"Current password: {currentPassword}, User password in database: {user.Password}");
            Console.WriteLine($"New password: {newPassword}");
            
            if (newPassword.Length < 5 || newPassword.Length > 20)
            {
                Console.WriteLine("bad password length");
                ModelState.AddModelError("", "Password must be between 5 and 20 characters.");
                return View("Profile");
            }
            if (currentPassword != user.Password)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View("Profile");
            }

            user.Password = newPassword;
            _context.SaveChanges();

            ViewBag.Message = "Password updated successfully!";
            return View("Profile");
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeleteAccount()
        {
            var userEmail = User.Identity.Name;
            var user = _context.UserAccounts.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View("Profile");
            }

            _context.UserAccounts.Remove(user);
            _context.SaveChanges();


            HttpContext.SignOutAsync();  //logs the user out after deleting account

            // Redirects to home page
            ViewBag.Message = "Your account has been deleted.";
            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}