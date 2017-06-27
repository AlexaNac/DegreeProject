using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class JobViewModel
    {
        public job job { get; set; }
        public IEnumerable<employee> employees;
    }
}