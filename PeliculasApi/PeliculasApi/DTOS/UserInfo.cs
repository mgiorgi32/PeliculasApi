using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class UserInfo
    {
        [Required]
        public string Email { get; set; }
        [Required] 
        public string Password { get; set; }
    }
}
