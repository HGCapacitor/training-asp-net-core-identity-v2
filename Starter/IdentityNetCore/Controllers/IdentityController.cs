using System.Linq;
using System.Threading.Tasks;
using IdentityNetCore.Models;
using IdentityNetCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers;

public class IdentityController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEMailService _eMailService;

    public IdentityController(UserManager<IdentityUser> userManager, IEMailService eMailService)
    {
        _userManager = userManager;
        _eMailService = eMailService;
    }
    
    // GET
    public async Task<IActionResult> Signup()
    {
        var model = new SignupViewModel();
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Signup(SignupViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync(model.Email);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.ActionLink("ConfirmEmail", "Identity", new {userId = user.Id, token = token});
                    await _eMailService.SendEmailAsync(new EMail
                    {
                        From = "info@fun.com",
                        To = user.Email,
                        Subject = "Confirm you email address",
                        Body = confirmationLink
                    });
                    return RedirectToAction("Signin");
                }
                
                ModelState.AddModelError("Signup", string.Join(", ", result.Errors.Select(x => x.Description)));
                return View(model);
            }
        }
        
        return View(model);
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("Signin");
            }
        }

        return new NotFoundResult();
    }

    public async Task<IActionResult> Signin()
    {
        return View();
    }
    
    public async Task<IActionResult> AccessDenied()
    {
        return View();
    }
}