using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TaskListViewModel
    {
        public IEnumerable<task> Tasks;
        public IEnumerable<task> MyTasks;
    }
}