using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Queries
{
    public class SmallQueryObject
    {
        [SwaggerSchema("tag list to filter by tags")]
        public List<Guid> Tags { get; set; } = new List<Guid>();
        [SwaggerSchema("option to sort posts")]
        public PostSorting? Sorting { get; set; } = null;
        [SwaggerSchema("page number")]
        [DefaultValue(1)]
        public int Page {get; set; } = 1;
        [SwaggerSchema("required number of elements per page")]
        [DefaultValue(5)]
        public int Size { get; set; } = 5;
    }
}