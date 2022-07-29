﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teste_Csharp
{
    public class MyUser:IdentityUser
    {
        public string FullName { get; set; }
        public string OrgId { get; set; }

    }

    public class Organization
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
