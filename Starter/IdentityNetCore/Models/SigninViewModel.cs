using System.ComponentModel.DataAnnotations;

namespace IdentityNetCore.Models;

public class SigninViewModel
{
    [Required(ErrorMessage = "Username must be provided")]
    [DataType(DataType.EmailAddress)]
    public string Username { get; set; }
    [Required(ErrorMessage = "Password must be provided")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}