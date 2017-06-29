using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace WebApplication1.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        public ActionResult Index()
        {
            IEnumerable<department> departments;
            using (var _context = new ProjectDBContext())
            {
                departments = _context.departments.Include(e => e.employee).ToList();
            }
            return View(departments);
        }

        public ActionResult Details(Guid id)
        {
            department dep = new department();
            using (var _context = new ProjectDBContext())
            {
                //dep.department = _context.departments.Include(e => e.manager_id).FirstOrDefault(e => e.department_id == id);
                dep = _context.departments.Include(e => e.employee).FirstOrDefault(e => e.department_id == id);
                dep.employees = _context.employees.Where(e => e.department_id == id).ToList();
            }
            if (dep == null)
            {
                RedirectToAction("Index", "Department");// HttpNotFound();
            }
            return View(dep);
        }

        public ActionResult New()
        {
            DepartmentViewModel departmentModel;
            using (var _context = new ProjectDBContext())
            {
                departmentModel = new DepartmentViewModel
                {
                    employees = _context.employees.ToList(),                 
                };
            }
            return View(departmentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                using (var _context = new ProjectDBContext())
                {
                    model.employees = _context.employees.ToList();
                }
                return View("New", model);
            }
            department newdep;
            using (var _context = new ProjectDBContext())
            {
                newdep = new department
                {
                    department_id = Guid.NewGuid(),
                    department_name = model.department.department_name,
                    manager_id = model.department.manager_id
                };
                _context.departments.Add(newdep);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Department", new { id = newdep.department_id });
        }

        public ActionResult Edit(Guid id)
        {
            department dep = new department();
            using (var _context = new ProjectDBContext())
            {
                dep = _context.departments.FirstOrDefault(e => e.department_id == id);
                dep.employees = _context.employees.ToList();
            }
            if (dep == null)
            {
                //RedirectToAction("Index", "Department");
                return HttpNotFound();
            }
            return View(dep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(department dep)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Edit", "Department", dep.department_id);
            }
            department departmentDB;
            using (var _context = new ProjectDBContext())
            {
                departmentDB = _context.departments.Single(e => e.department_id == dep.department_id);
                TryUpdateModel(departmentDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Department", new { id = departmentDB.department_id });
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var department = new department();
            using (var _context = new ProjectDBContext())
            {
                department = _context.departments.SingleOrDefault(e => e.department_id == id);
            }
            if (department == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(department dep)
        {
            using (var _context = new ProjectDBContext())
            {
                department department;
                IEnumerable<employee> employees = _context.employees.Where(e => e.department_id == dep.department_id).ToList();
                if (employees.Count() == 0)
                {
                    department = _context.departments.SingleOrDefault(e => e.department_id == dep.department_id);
                    if (department == null)
                    {
                        return RedirectToAction("Index");
                    }
                    try
                    {
                        _context.departments.Remove(department);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        return RedirectToAction("Delete", new { id = dep.department_id, saveChangesError = true });
                    }
                }
                else
                    return RedirectToAction("Delete", new { id = dep.department_id, saveChangesError = true }); //CU EROARE CA ARE EMPLOYEES ASOCIATI;                
            }
        }
    }
}