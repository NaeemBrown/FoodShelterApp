using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodShelterAppC_.Data;
using FoodShelterAppC_.Models;
using System.Security.Claims;

namespace FoodShelterAppC_.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// adds a budget
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Dashboard");
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Deletes budget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null && budget.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Dashboard");
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}