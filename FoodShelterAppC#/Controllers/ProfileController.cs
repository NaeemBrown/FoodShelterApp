using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using FoodShelterAppC_.Models;
using FoodShelterAppC_.Data;
using System.Security.Claims;

namespace FoodShelterAppC_.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to update user profile, must delete or fix
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// method to update user profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(User model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;
            user.Bio = model.Bio;
            user.Website = model.Website;
            user.Contact = model.Contact;
            user.DonationLink = model.DonationLink;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully";
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}
