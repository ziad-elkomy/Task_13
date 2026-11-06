using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models
{
    public class UserOTPS
    {
        public UserOTPS(string userId, string oTP)
        {
            UserId = userId;
            OTP = oTP;
            isValid = true;
            CreatedAt = DateTime.UtcNow;
            VaildTo = DateTime.UtcNow.AddMinutes(10);
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser applicationUser { get; set; }
        public string OTP { get; set; }
        public bool isValid { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime VaildTo { get; set; }

    }
}
