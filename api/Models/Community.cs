using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Community
    {
        public Guid Id { get; set; }
        public DateTime CreateTime{ get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsClosed { get; set; } = false;
        public int SubscribersCount { get; set; } = 0;
        public List<User>? Administrators { get; set; }

        public Community() 
        {
            Id = new Guid();
            CreateTime = DateTime.UtcNow;
        }
    }
}