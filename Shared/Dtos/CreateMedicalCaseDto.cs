using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class CreateMedicalCaseDto
    {
        public string Title { get; set; }
        
        public IFormFile BeforeImage { get; set; }
        public IFormFile AfterImage { get; set; }
       
    }
}
