using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class PostController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public PostController(ApplicationDBContext context)
        {
            _context = context;
        }
    }
}