﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stellmart.Auth.Services
{
    public interface ITotpService
    {
        bool Validate(string token, string code);
    }
}