﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudyPlatform.Data.Models
{
    // <summary>
    // Customization towards the default identity user. default is student.
    // </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Age { get; set; }
        public string Role { get; set; }

        //public string ProfilePicture { get; set; }

        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }

        // location (country, city)??? there might be some service
    }
}
