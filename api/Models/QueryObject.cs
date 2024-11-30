using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Models
{
    public class QueryObject
    {
        [SwaggerSchema("part of author name - for filtering by author")]
        public string? Author { get; set; } = null;
        [SwaggerSchema("minimum reading time in minutes - for filtering by reading time")]
        public int? Min { get; set; } = null;
        [SwaggerSchema("maximum reading time in minutes - for filtering by reading time")]
        public int? Max { get; set; } = null;
        [SwaggerSchema("option to sort posts")]
        public PostSorting? Sorting { get; set; } = null;
        [SwaggerSchema("flag to display posts only from communities the user is subscribed to")]
        public bool? OnlyMyCommunities { get; set; } = null;
    }
}