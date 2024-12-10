using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class House
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectGuid { get; set; } = string.Empty;
        public string? HouseNum { get; set; } = string.Empty;
        public string? AddNum1 { get; set; } = string.Empty;
        public string? AddNum2 { get; set; } = string.Empty;
        public byte? HouseType { get; set; }
        public byte? AddType1 { get; set; }
        public string? AddType2 { get; set; } = string.Empty;
        public int PrevId { get; set; }
        public int NextId { get; set; }
        public bool IsActual { get; set; }
        public bool IsActive { get; set; }
    }
}