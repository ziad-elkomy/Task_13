using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.ViewModels
{
    public class RegisterVM
    {
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password) , Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }

    }
}
