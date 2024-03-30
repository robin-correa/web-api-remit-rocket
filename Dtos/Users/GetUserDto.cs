using System.ComponentModel.DataAnnotations;

namespace web_api_remit_rocket.Dtos.Users
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;

        public int Gender { get; set; }
        public string BirthDate { get; set; } = string.Empty;
        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
