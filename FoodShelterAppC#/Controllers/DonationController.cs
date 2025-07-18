using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodShelterAppC_.Data;
using FoodShelterAppC_.Models;
using System.Security.Claims;

namespace FoodShelterAppC_.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// creates donation
        /// </summary>
        /// <param name="donation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(Donation donation)
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

                donation.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                donation.Date = DateTime.Now;
                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Donation added successfully" });
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

        /// <summary>
        /// deletes donation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation != null && donation.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.Donations.Remove(donation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Dashboard");
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}
