using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodShelterAppC_.Data;
using FoodShelterAppC_.Models;
using System.Security.Claims;

namespace FoodShelterAppC_.Controllers
{
    [Authorize]
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteerController(ApplicationDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to add a volunter
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(Volunteer volunteer)
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

                volunteer.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Volunteers.Add(volunteer);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Volunteer added successfully" });
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
        /// method to delete a volunteer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer != null && volunteer.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.Volunteers.Remove(volunteer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Dashboard");
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}