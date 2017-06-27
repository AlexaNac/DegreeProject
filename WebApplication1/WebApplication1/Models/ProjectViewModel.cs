using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ProjectViewModel
    {
        public project project { get; set; }
        public IEnumerable<task> tasks;
        public IEnumerable<employee> employees;
        public IEnumerable<client> clients;
    }
}