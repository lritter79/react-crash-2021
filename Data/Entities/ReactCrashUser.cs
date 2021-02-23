﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace react_crash_2021.Data.Entities
{
    public class reactCrashUser : IdentityUser<Guid>
    {
        public IEnumerable<TaskEntity> tasks;
        public DateTime? dateCreated;
        
    }
}
