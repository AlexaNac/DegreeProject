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
    public class ProjectController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<project> projects;
            using (var _context = new ProjectDBContext())
            {
                projects = _context.projects.Include(e=>e.employee).Include(e=>e.client).ToList();
            }
            return View(projects);
        }

        public ActionResult Details(Guid id)
        {
            ProjectViewModel project = new ProjectViewModel();
            using (var _context = new ProjectDBContext())
            {
                project.project = _context.projects.Include(e => e.employee).Include(e => e.client).FirstOrDefault(e => e.project_id == id);
                project.tasks = _context.tasks.Where(e => e.project_id == id).ToList();
            }
            if (project.project == null)
            {
                RedirectToAction("Index", "Project");// HttpNotFound();
            }
            return View(project);
        }

        public ActionResult New()
        {
            ProjectViewModel projectModel;
            using (var _context = new ProjectDBContext())
            {
                projectModel = new ProjectViewModel
                {
                    employees = _context.employees.ToList(),
                    clients = _context.clients.ToList()
                };
            }
            return View(projectModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                using (var _context = new ProjectDBContext())
                {
                    model.employees = _context.employees.ToList();
                    model.clients = _context.clients.ToList();
                }
                return View("New", model);
            }
            project newproject;
            using (var _context = new ProjectDBContext())
            {
                newproject = new project
                {
                    project_id = Guid.NewGuid(),
                    project_manager = model.project.project_manager,
                    project_name = model.project.project_name,
                    client_id = model.project.client_id
                };
                _context.projects.Add(newproject);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit", "Project", new { id = newproject.project_id });
        }

        public ActionResult Edit(Guid id)
        {
            project project = new project();
            using (var _context = new ProjectDBContext())
            {
                project = _context.projects.Include(e => e.employee).Include(e => e.client).FirstOrDefault(e => e.project_id == id);
                project.employees = _context.employees.ToList();
            }
            if (project == null)
            {
                //RedirectToAction("Index", "Department");
                return HttpNotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(project prj)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Edit", "Project", prj.project_id);
            }
            project projectDB;
            using (var _context = new ProjectDBContext())
            {
                projectDB = _context.projects.Single(e => e.project_id == prj.project_id);
                TryUpdateModel(projectDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Project");
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var project = new project();
            using (var _context = new ProjectDBContext())
            {
                project = _context.projects.Include(e => e.employee).Include(e => e.client).SingleOrDefault(e => e.project_id == id);
            }
            if (project == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(project proj)
        {
            using (var _context = new ProjectDBContext())
            {
                project project;
                IEnumerable<task> tasks = _context.tasks.Where(e => e.project_id == proj.project_id).ToList();
                if (tasks.Count() == 0)
                {
                    project = _context.projects.SingleOrDefault(e => e.project_id == proj.project_id);
                    if (project == null)
                    {
                        return RedirectToAction("Index");
                    }
                    try
                    {
                        _context.projects.Remove(project);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        return RedirectToAction("Delete", new { id = proj.project_id, saveChangesError = true });
                    }
                }
                else
                    return RedirectToAction("Delete", new { id = proj.project_id, saveChangesError = true }); //CU EROARE CA ARE EMPLOYEES ASOCIATI;                
            }
        }
    }
}