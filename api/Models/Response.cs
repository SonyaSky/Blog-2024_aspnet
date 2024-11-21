using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Response
    {
        public string? Status { get; set; } = String.Empty;
        public string? Message { get; set; }
    }
}