using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class EmployeeViewModel
    {
        public employee employee
        {
            get; set;
        }
        public IEnumerable<task> tasks;
        public IEnumerable<department> departments;
        public IEnumerable<project> projects;
        public IEnumerable<employee> managers;
        public IEnumerable<job> jobs;
        public IEnumerable<AspNetUser> users;

        public EmployeeViewModel()
        {
            //this.departments = new HashSet<department>();
            //this.managers = new HashSet<employee>();
            //this.projects = new HashSet<project>();
            //this.tasks = new HashSet<task>();
            //this.jobs = new HashSet<job>();
            //this.users = new HashSet<AspNetUser>();
        }
    }
}