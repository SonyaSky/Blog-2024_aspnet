using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace api.Models
{
    public enum Gender
    {
        Male,
        Female
    }
    public class User
    {
        public Guid Id { get; set; } 
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreateTime{ get; set; } 

        public User()
        {
            Id = Guid.NewGuid(); 
            CreateTime = DateTime.UtcNow; 
        }
    }
}