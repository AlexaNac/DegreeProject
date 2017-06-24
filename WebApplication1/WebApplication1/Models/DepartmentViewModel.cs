using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class DepartmentViewModel
    {
       public  department department
        {
            get; set;
        }
       public IEnumerable<employee> employees
        {
            get; set; 
        }
    }
}