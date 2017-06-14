using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            IEnumerable<employee> employees;
            using (var _context = new ProjectDBContext())
            {
                employees = _context.employees.ToList();
            }
            return View(employees);
        }

        public ActionResult New()
        {
            employee employeeModel;
            using (var _context = new ProjectDBContext())
            {
                var departmentsList = _context.departments.ToList();
                var jobsList = _context.jobs.ToList();
                var managersList = _context.employees.ToList();
                var usersList = _context.AspNetUsers.ToList();
                employeeModel = new employee
                {
                    jobs = jobsList,
                    departments = departmentsList,
                    employees1 = managersList,
                    users = usersList
                };
            }
            return View(employeeModel);
        }

        public ActionResult Edit(Guid id)
        {
            employee emp;          
            using (var _context = new ProjectDBContext())
            {
                emp = _context.employees.FirstOrDefault(e => e.employee_id == id);
            }       
            if(emp == null)
            {
                RedirectToAction("Index", "Employee");// HttpNotFound();
            }
            return View(emp);
        }

        public ActionResult Details(Guid id)
        {
            employee emp;
            using (var _context = new ProjectDBContext())
            {
                emp = _context.employees.Include(e => e.job).Include(e => e.AspNetUser).Include(e => e.department).Include(e => e.employee1).FirstOrDefault(e => e.employee_id == id);
            }
            if (emp == null)
            {
                RedirectToAction("Index", "Employee");// HttpNotFound();
            }
            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(employee emp)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Edit","Employee",emp.employee_id);
            }
            employee employeeDB;
            using (var _context = new ProjectDBContext()) {
                employeeDB = _context.employees.Single(e => e.employee_id == emp.employee_id);
                TryUpdateModel(employeeDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Employee");
            }           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(employee emp)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("New","Employee", emp);
            }
            employee newemp;
            using (var _context = new ProjectDBContext())
            {
                    newemp = new employee
                    {
                        employee_id = Guid.NewGuid(),
                        first_name = emp.first_name,
                        last_name = emp.last_name,
                        birth_date = emp.birth_date,
                        hire_date = emp.hire_date,
                        job_id = emp.job_id,
                        department_id = emp.department_id,
                        phone_number = emp.phone_number,
                        email = emp.email,
                        salary = emp.salary,
                        user_id = emp.user_id,
                        manager_id = emp.manager_id
                    };
                _context.employees.Add(newemp);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit","Employee", newemp.employee_id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id)
        {
            employee employeeDB;
            using (var _context = new ProjectDBContext())
            {
                employeeDB = _context.employees.Single(e => e.employee_id == id);
                _context.employees.Remove(employeeDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Employee");
            }
        }
    }
}