﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Entities
{
    public class User : IdentityUser
    {
        public DateTimeOffset BirthDate { get; set; }
    }
}
