using System.ComponentModel.DataAnnotations;
namespace Auction.Models
{
    public class RegisterViewModel : BaseEntity
    {
        [Required]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string LastName { get; set; }
 
        [Required]
        [MinLength(3,ErrorMessage = "User Name must be longer than 3 characters")]
        [MaxLength(20,ErrorMessage = "User Name must be shorter than 20 characters")]
        public string UserName { get; set; }
 
        [Required]
        [MinLength(8, ErrorMessage="password must be 8 charactors long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation must match.")]
        public string PasswordConfirmation { get; set; }
    }
}