using System.ComponentModel.DataAnnotations;

namespace Adapter.BlogApi.Services.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(80, ErrorMessage = "Password required between 8 and 20 symbols", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public DateTime Created { get; set; }
    }
}
