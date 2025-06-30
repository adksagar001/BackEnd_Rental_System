using System;
using System.ComponentModel.DataAnnotations;

namespace MobileAppsAPIS.Model
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string? Profile { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string OTP { get; set; }
    }
}
