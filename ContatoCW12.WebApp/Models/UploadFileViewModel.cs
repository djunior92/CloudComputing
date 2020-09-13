using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContatoCW12.WebApp.Models
{
    public class UploadFileViewModel
    {
        public int Id { get; set; }
        public IFormFile File { get; set; }
    }
}
