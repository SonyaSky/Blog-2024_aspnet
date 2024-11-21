using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dtos.Post
{
    public class PostPagedListDto
    {
        public List<PostDto>? Posts { get; set; }
        public PageInfoModel? Pagination { get; set; }
    }
}