
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtEx.Model;

public class LoginCredentials
{

    [Required]
    [EmailAddress(ErrorMessage = "Enter valid Email Id")]
    public string email { get; set; }

    [Required]
    public string password { get; set; }
}
