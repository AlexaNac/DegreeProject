using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;

namespace WebApplication1.Controllers
{
    public class TaskController : Controller
    {
        public ActionResult Index()
        {
            TaskListViewModel model = new TaskListViewModel();
            string userid = User.Identity.GetUserId();
            using (var _context = new ProjectDBContext())
            {
                model.Tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.task_name);
                model.MyTasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().Where(e => e.employee.user_id == userid).OrderBy(e => e.task_name);
                //tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.importance_id);
                //tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.endDate);

            }
            return View(model);
        }

        public ActionResult MyTasks()
        {
            IEnumerable<task> tasks;
            string userid = User.Identity.GetUserId();
            using (var _context = new ProjectDBContext())
            {
                tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().Where(e => e.employee.user_id == userid).OrderBy(e => e.task_name);
                //tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.importance_id).Where(e => e.employee.user_id == userid);
                //tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.endDate).Where(e => e.employee.user_id == userid);
            }
            return View(tasks);
        }

        public ActionResult Details(Guid id)
        {
            task task = new task();
            using (var _context = new ProjectDBContext())
            {
                task = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).FirstOrDefault(e => e.task_id == id);
            }
            if (task == null)
            {
                RedirectToAction("Index", "Task");// HttpNotFound();
            }
            return View(task);
        }

        public ActionResult New()
        {
            TaskViewModel taskModel;
            using (var _context = new ProjectDBContext())
            {
                taskModel = new TaskViewModel
                {
                    employees = _context.employees.ToList(),
                    projects = _context.projects.ToList(),
                    importanceList = _context.importances.ToList(),
                    statusList = _context.status.ToList()
                };
            }
            return View(taskModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                using (var _context = new ProjectDBContext())
                {
                    model.employees = _context.employees.ToList();
                    model.projects = _context.projects.ToList();
                    model.importanceList = _context.importances.ToList();
                    model.statusList = _context.status.ToList();
                }
                return View("New", model);
            }
            task newTask;
            using (var _context = new ProjectDBContext())
            {
                newTask = new task
                {
                    task_id = Guid.NewGuid(),
                    task_name = model.task.task_name,
                    project_id = model.task.project_id,
                    importance_id = model.task.importance_id,
                    status_id = model.task.status_id,
                    startDate = model.task.startDate,
                    endDate = model.task.endDate,
                    employee_id = model.task.employee_id,
                    detail = model.task.detail,
                    time = model.task.time
                };
                _context.tasks.Add(newTask);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Task");
            //return RedirectToAction("Edit", "Task", new { id = newTask.task_id });
        }

        public ActionResult Edit(Guid id)
        {
            task task = new task();
            using (var _context = new ProjectDBContext())
            {
                task = _context.tasks.FirstOrDefault(e => e.task_id == id);
                task.employees = _context.employees.ToList();
                task.statusList = _context.status.ToList();
            }
            if (task == null)
            {
                //RedirectToAction("Index", "Department");
                return HttpNotFound();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(task tsk)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Edit", "Task", tsk.task_id);
            }
            task taskDB;
            using (var _context = new ProjectDBContext())
            {
                taskDB = _context.tasks.Single(e => e.task_id == tsk.task_id);
                TryUpdateModel(taskDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Task");
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var task = new task();
            using (var _context = new ProjectDBContext())
            {
                task = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).SingleOrDefault(e => e.task_id == id);
            }
            if (task == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(task tsk)
        {
            using (var _context = new ProjectDBContext())
            {
                task task;

                    task = _context.tasks.SingleOrDefault(e => e.task_id == tsk.task_id);
                    if (task == null)
                    {
                        return RedirectToAction("Index");
                    }
                    try
                    {
                        _context.tasks.Remove(task);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        return RedirectToAction("Delete", new { id = tsk.task_id, saveChangesError = true });
                    }                            
            }
        }

    }
}