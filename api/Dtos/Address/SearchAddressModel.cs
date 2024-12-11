using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Address
{
    public class SearchAddressModel
    {
        public int ObjectId { get; set; }
        public string ObjectGuid { get; set; } = string.Empty;
        public string? Text { get; set; } = string.Empty;
        public GarAddressLevel ObjectLevel { get; set; }
        public string? ObjectLevelText { get; set; }
    }
}