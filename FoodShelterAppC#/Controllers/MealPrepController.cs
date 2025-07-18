using FoodShelterAppC_.Data;
using FoodShelterAppC_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodShelterAppC_.Controllers
{
    [Authorize]
    public class MealPrepController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MealPrepController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //------------------------------------------------------------------------------------------------------------------

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var meals = await _context.MealPlans
                .Include(m => m.Ingredients)
                .ThenInclude(i => i.FoodStock)
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            ViewBag.FoodStock = await _context.FoodStocks
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            return View(meals);
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to add a meal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddMeal([FromBody] MealPlanDto model)
        {
            if (!ModelState.IsValid) return Json(new { success = false });

            var user = await _userManager.GetUserAsync(User);
            var mealPlan = new MealPlan
            {
                Name = model.Name,
                Servings = model.Servings,
                UserId = user.Id,
                IsPrepared = false
            };

            foreach (var ingredient in model.Ingredients)
            {
                mealPlan.Ingredients.Add(new MealIngredient
                {
                    FoodStockId = ingredient.FoodStockId,
                    QuantityPerServing = ingredient.QuantityPerServing
                });
            }

            _context.MealPlans.Add(mealPlan);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to remove items from food stock table once meal is prepared
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PrepareMeal(int id)
        {
            var meal = await _context.MealPlans
                .Include(m => m.Ingredients)
                .ThenInclude(i => i.FoodStock)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meal == null)
                return Json(new { success = false, message = "Meal not found" });

            if (meal.IsPrepared)
                return Json(new { success = false, message = "Meal already prepared" });

            // Check if we have enough ingredients
            foreach (var ingredient in meal.Ingredients)
            {
                decimal requiredAmount = ingredient.QuantityPerServing * meal.Servings;
                if (ingredient.FoodStock.Quantity < requiredAmount)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Not enough {ingredient.FoodStock.ItemName} (need {requiredAmount}, have {ingredient.FoodStock.Quantity})" 
                    });
                }
            }

            // Deduct ingredients
            foreach (var ingredient in meal.Ingredients)
            {
                decimal deductAmount = ingredient.QuantityPerServing * meal.Servings;
                ingredient.FoodStock.Quantity -= deductAmount;
            }

            meal.IsPrepared = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to delete a meal plan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var meal = await _context.MealPlans
                .Include(m => m.Ingredients)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meal == null)
                return Json(new { success = false, message = "Meal plan not found." });

            try
            {
                // First remove all related ingredients
                _context.MealIngredients.RemoveRange(meal.Ingredients);
                // Then remove the meal plan
                _context.MealPlans.Remove(meal);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting meal plan." });
            }
        }
        //------------------------------------------------------------------------------------------------------------------
    }

    public class MealPlanDto
    {
        public string Name { get; set; }
        public int Servings { get; set; }
        public List<MealIngredientDto> Ingredients { get; set; }
    }

    public class MealIngredientDto
    {
        public int FoodStockId { get; set; }
        public decimal QuantityPerServing { get; set; }
    }
}
