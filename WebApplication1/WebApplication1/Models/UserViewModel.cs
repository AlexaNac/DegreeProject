using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UserViewModel
    {
        public employee employee
        {
            get; set;
        }
        public AspNetUser user
        {
            get; set;
        }
    }
}