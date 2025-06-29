﻿using Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser user, List<string> roles);
    }
}
