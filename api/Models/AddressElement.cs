using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class AddressElement
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectGuid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public byte Level { get; set; }
        public int PrevId { get; set; }
        public int NextId { get; set; }
        public bool IsActual { get; set; }
        public bool IsActive { get; set; }
    }
}