using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtEx.Model
{
    public class User
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        [EmailAddress (ErrorMessage = "Enter valid Email Id")]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        [NotMapped]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string confirmPassword { get; set; }

    }
}
