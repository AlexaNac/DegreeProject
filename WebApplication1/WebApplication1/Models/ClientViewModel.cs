using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ClientViewModel
    {
        public client client { get; set; }
        public IEnumerable<employee> employees;
        public IEnumerable<project> projects;
    }
}