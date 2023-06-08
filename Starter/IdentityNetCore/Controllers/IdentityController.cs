using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityNetCore.Models;
using IdentityNetCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers;

public class IdentityController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signinManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEMailService _eMailService;

    public IdentityController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signinManager,
        RoleManager<IdentityRole> roleManager,
        IEMailService eMailService)
    {
        _userManager = userManager;
        _signinManager = signinManager;
        _roleManager = roleManager;
        _eMailService = eMailService;
    }
    
    // GET
    public ActionResult Signup()
    {
        var model = new SignupViewModel() {Role = "Member"};
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Signup(SignupViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _userManager.FindByEmailAsync(model.Email) == null)
            {
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    var role = new IdentityRole { Name = model.Role };
                    var roleResult = await _roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        var errors = roleResult.Errors.Select(s => s.Description);
                        ModelState.AddModelError("Role", string.Join(",", errors));
                        return View(model);
                    }
                }
                
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync(model.Email);
                    var claim = new Claim("Department", model.Department);
                    await _userManager.AddClaimAsync(user, claim);
                    await _userManager.AddToRoleAsync(user, model.Role);
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

    public IActionResult Signin()
    {
        return View(new SigninViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Signin(SigninViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signinManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                // var userClaims = await _userManager.GetClaimsAsync(user);
                // if (!userClaims.Any(c => c.Type == "Department"))
                // {
                //     ModelState.AddModelError("Claim", "User not in Tech department");
                //     return View(model);
                // }
                if (await _userManager.IsInRoleAsync(user, "Member"))
                {
                    return RedirectToAction("Member", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("Login", "Cannot login");
        }
        return View(model);
    }

    public async Task<IActionResult> Signout()
    {
        await _signinManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    public IActionResult AccessDenied()
    {
        return View();
    }
}