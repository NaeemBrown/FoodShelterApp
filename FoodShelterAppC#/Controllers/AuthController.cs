using FoodShelterAppC_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FoodShelterAppC_.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        //------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid password. Please try again.");
                    return View();
                }
            }

            ModelState.AddModelError(string.Empty, "Email not found. Please register if you don't have an account.");
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            var user = new User { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Auth",
                    new { userId = user.Id, token = token }, Request.Scheme);

                await _emailService.SendEmailAsync(email, "Confirm your email",
                    $"Please confirm your email by clicking this link: {confirmationLink}");

                TempData["Message"] = "Registration successful! Please check your email to confirm your account.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                string errorMessage = error.Code switch
                {
                    "PasswordRequiresDigit" => "Password must contain at least one number.",
                    "PasswordRequiresLower" => "Password must contain at least one lowercase letter.",
                    "PasswordRequiresUpper" => "Password must contain at least one uppercase letter.",
                    "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character.",
                    "PasswordTooShort" => "Password must be at least 6 characters long.",
                    _ => error.Description
                };
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Confirms email for registering user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "Thank you for confirming your email. You can now login.";
            }
            else
            {
                TempData["Error"] = "Error confirming your email.";
            }

            return RedirectToAction("Login");
        }
        //------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Method to send reset password link to selected email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Auth",
                    new { email = email, token = token }, Request.Scheme);

                await _emailService.SendEmailAsync(email, "Reset Password",
                    $"Please reset your password by clicking here: {callbackUrl}");

                TempData["Message"] = "If an account exists with this email, you will receive password reset instructions.";
            }

            // Don't reveal that the user does not exist
            TempData["Message"] = "If an account exists with this email, you will receive password reset instructions.";
            return RedirectToAction("Login");
        }
        //------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// method to reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData["Message"] = "Your password has been reset successfully.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        //------------------------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        //------------------------------------------------------------------------------------------------------------------
    }
}
