using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegisterDtos
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8,MinimumLength = 4,ErrorMessage = "Password length must be between 4 and 8")]
        public string PassWord { get; set; }
    }
}