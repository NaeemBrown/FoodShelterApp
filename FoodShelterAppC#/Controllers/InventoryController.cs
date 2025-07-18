using FoodShelterAppC_.Data;
using FoodShelterAppC_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodShelterAppC_.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public InventoryController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to delete food stock
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteFoodStock(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var foodStock = await _context.FoodStocks
                .Include(f => f.MealIngredients)
                .ThenInclude(mi => mi.MealPlan)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == user.Id);

            if (foodStock != null)
            {
                if (foodStock.MealIngredients.Any())
                {
                    var mealNames = foodStock.MealIngredients
                        .Select(mi => mi.MealPlan.Name)
                        .Distinct();
                    return Json(new { 
                        success = false, 
                        message = $"Cannot delete this item as it is used in the following meal plans: {string.Join(", ", mealNames)}. Please delete these meal plans first." 
                    });
                }

                try
                {
                    _context.FoodStocks.Remove(foodStock);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error deleting food stock item." });
                }
            }
            return Json(new { success = false, message = "Food stock not found." });
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to return the foodstocks with near minumum stock
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LowStock()
        {
            var user = await _userManager.GetUserAsync(User);
            var lowStockItems = await _context.FoodStocks
                .Where(f => f.UserId == user.Id && f.Quantity <= f.MinimumStock)
                .ToListAsync();
            return View(lowStockItems);
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to return food stock that is expring within a week, must still implement
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ExpiringSoon()
        {
            var user = await _userManager.GetUserAsync(User);
            var expiringItems = await _context.FoodStocks
                .Where(f => f.UserId == user.Id &&
                       f.ExpirationDate != null &&
                       f.ExpirationDate <= DateTime.Now.AddDays(7))
                .ToListAsync();
            return View(expiringItems);
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}
