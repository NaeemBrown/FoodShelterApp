// Controllers/DashboardController.cs
using FoodShelterAppC_.Data;
using FoodShelterAppC_.Models;
using FoodShelterAppC_.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using FoodShelterAppC_.Services;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DashboardController> _logger;
    private readonly OpenStreetMapService _geocodingService;

    public DashboardController(
        ApplicationDbContext context,
        UserManager<User> userManager,
        OpenStreetMapService geocodingService,
        ILogger<DashboardController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _geocodingService = geocodingService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var viewModel = new DashboardViewModel
        {
            FoodStocks = await _context.FoodStocks
                .Where(f => f.UserId == userId)
                .ToListAsync(),

            ShelterLocations = await _context.ShelterLocations
                .Where(s => s.UserId == userId)
                .ToListAsync(),

            Notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(),

            Volunteers = await _context.Volunteers
                .Where(v => v.UserId == userId)
                .ToListAsync(),

            BudgetEntries = await _context.Budgets
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Date)
                .ToListAsync(),

            Donations = await _context.Donations
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Date)
                .ToListAsync()
        };

        return View(viewModel);
    }
    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// adds a shelter location in the modal using nominatm geolation api
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddShelterLocation([FromForm] ShelterLocation model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { 
                    success = false, 
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => new { errorMessage = e.ErrorMessage })
                });
            }

            var (latitude, longitude) = await _geocodingService.GeocodeAddress(model.Address);
            
            if (!latitude.HasValue || !longitude.HasValue)
            {
                return Json(new { 
                    success = false, 
                    errors = new[] { new { errorMessage = "Could not geocode address" } }
                });
            }

            model.Latitude = latitude.Value;
            model.Longitude = longitude.Value;

            _context.ShelterLocations.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = $"Location added successfully. Lat: {latitude}, Lon: {longitude}" 
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                errors = new[] { new { errorMessage = $"Error adding location: {ex.Message}" } }
            });
        }
    }
    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// adds a note in modals
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddNote([FromForm] Note model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { 
                    success = false, 
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => new { errorMessage = e.ErrorMessage })
                });
            }

            model.CreatedAt = DateTime.UtcNow;
            _context.Notes.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Note added successfully" 
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                errors = new[] { new { errorMessage = $"Error adding note: {ex.Message}" } }
            });
        }
    }
    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// deletes food stock in modals
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteFoodStock(int id)
    {
        var foodStock = await _context.FoodStocks.FindAsync(id);
        if (foodStock != null)
        {
            _context.FoodStocks.Remove(foodStock);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }
    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// addds a volunteer for modals
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddVolunteer([FromForm] Volunteer model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { 
                    success = false, 
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => new { errorMessage = e.ErrorMessage })
                });
            }


            _context.Volunteers.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Volunteer added successfully" 
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                errors = new[] { new { errorMessage = $"Error adding volunteer: {ex.Message}" } }
            });
        }
    }
    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// adds a budeget for modals
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddBudget([FromForm] Budget model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { 
                    success = false, 
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => new { errorMessage = e.ErrorMessage })
                });
            }


            _context.Budgets.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Budget entry added successfully" 
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                errors = new[] { new { errorMessage = $"Error adding budget entry: {ex.Message}" } }
            });
        }
    }
    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// adds a donation for modals
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddDonation([FromForm] Donation model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { 
                    success = false, 
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => new { errorMessage = e.ErrorMessage })
                });
            }

            _context.Donations.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Donation added successfully" 
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                errors = new[] { new { errorMessage = $"Error adding donation: {ex.Message}" } }
            });
        }
    }
    //------------------------------------------------------------------------------------------------------------------
}
