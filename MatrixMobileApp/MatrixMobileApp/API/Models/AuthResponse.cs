﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMobileApp.API.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

    }
}