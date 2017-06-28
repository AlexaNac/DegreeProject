using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TaskViewModel
    {
        public task task { get; set; }
        public IEnumerable<employee> employees;
        public IEnumerable<project> projects;
        public IEnumerable<importance> importanceList;
        public IEnumerable<status> statusList;
    }
}