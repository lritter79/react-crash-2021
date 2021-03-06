﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace react_crash_2021.Models
{
    /// <summary>
    /// An instance of the register view model the user will interact with
    /// </summary>
    public class RegisterModel
    {
        //[Required]
        //[StringLength(256)]
        public string Email { get; set; }
        [Required]
        [StringLength(256)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
