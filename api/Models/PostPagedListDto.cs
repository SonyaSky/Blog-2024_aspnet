using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Post;

namespace api.Models
{
    public class PostPagedListDto
    {
        public List<PostDto> Posts { get; set; } = new List<PostDto>();
        public PageInfoModel Pagination { get; set; }

    }
}