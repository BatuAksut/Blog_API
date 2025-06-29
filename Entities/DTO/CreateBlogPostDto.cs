﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class CreateBlogPostDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile? Image { get; set; }

    }

}
