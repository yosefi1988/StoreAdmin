using System;

namespace WebApplicationCar.Models
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public string DisplayName
        {
            get
            {
                var full = ((FirstName ?? "") + " " + (LastName ?? "")).Trim();
                return string.IsNullOrEmpty(full) ? (UserName ?? MobileNumber) : full;
            }
        }
    }

    public class EditProfileViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}