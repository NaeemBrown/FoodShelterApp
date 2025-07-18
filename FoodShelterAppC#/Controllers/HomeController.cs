using FoodShelterAppC_.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using FoodShelterAppC_.Data;
using System.Text.Json;

namespace FoodShelterAppC_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// collects data for map in home view
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var locations = await _context.ShelterLocations
                .Where(s => s.Latitude.HasValue && s.Longitude.HasValue)
                .Select(s => new
                {
                    latitude = s.Latitude,
                    longitude = s.Longitude,
                    address = s.Address
                })
                .ToListAsync();

            var users = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.Name))
                .Include(u => u.Volunteers)
                .Include(u => u.ShelterLocations)
                .ToListAsync();
            
            ViewBag.Users = users;
            ViewBag.MapLocations = JsonSerializer.Serialize(locations);
            
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------

        public IActionResult Privacy()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}