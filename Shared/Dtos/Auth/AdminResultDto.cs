﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Auth
{
    public class AdminResultDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; } 
        public string Token { get; set; }
       
    }
}
