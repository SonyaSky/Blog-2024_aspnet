using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace api.Queries
{
    public class QueryObject
    {
        [SwaggerSchema("tag list to filter by tags")]
        public List<Guid> Tags { get; set; } = new List<Guid>();
        [SwaggerSchema("part of author name - for filtering by author")]
        public string? Author { get; set; } = null;
        [SwaggerSchema("minimum reading time in minutes - for filtering by reading time")]
        public int? Min { get; set; } = null;
        [SwaggerSchema("maximum reading time in minutes - for filtering by reading time")]
        public int? Max { get; set; } = null;
        [SwaggerSchema("option to sort posts")]
        public PostSorting? Sorting { get; set; } = null;
        [SwaggerSchema("flag to display posts only from communities the user is subscribed to")]
        public bool? OnlyMyCommunities { get; set; } = false;
        [SwaggerSchema("page number")]
        [DefaultValue(1)]
        public int Page {get; set; } = 1;
        [SwaggerSchema("required number of elements per page")]
        [DefaultValue(5)]
        public int Size { get; set; } = 5;
    }
}