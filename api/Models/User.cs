using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace api.Models
{
    public enum Gender
    {
        Male,
        Female
    }
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public DateTime CreateTime{ get; set; } 

        public User()
        {
            CreateTime = DateTime.UtcNow; 
        }
    }
}