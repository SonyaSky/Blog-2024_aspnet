using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Queries
{
    public class SearchQuery
    {
        public int ParentObjectId { get; set; } = 0;
        public string? Query { get; set; } = null;
    }
}