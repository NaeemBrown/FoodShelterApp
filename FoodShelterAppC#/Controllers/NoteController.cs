using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodShelterAppC_.Data;
using FoodShelterAppC_.Models;
using System.Security.Claims;

namespace FoodShelterAppC_.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NoteController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// method to create a note
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(Note note)
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

                note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                note.CreatedAt = DateTime.UtcNow;
                _context.Notes.Add(note);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Note added successfully" });
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
        /// method to delete a note
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null && note.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Dashboard");
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}