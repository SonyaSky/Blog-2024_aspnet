using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Post;

namespace api.Models
{
    public class PostPagedList
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public PageInfoModel Pagination { get; set; } = new PageInfoModel();

    }
}