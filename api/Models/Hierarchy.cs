using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Hierarchy
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public int ParentObjId { get; set; } 
        public int PrevId { get; set; }
        public int NextId { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; } = string.Empty;
    }
}