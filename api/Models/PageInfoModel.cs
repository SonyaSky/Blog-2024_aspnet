using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class PageInfoModel
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public int Current { get; set; }
    }
}