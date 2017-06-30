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
    [Authorize(Roles = "IT")]
    public class TaskController : Controller
    {
        public ActionResult Index(int sortedBy)
        {
            TaskListViewModel model = new TaskListViewModel();
            string userid = User.Identity.GetUserId();
            using (var _context = new ProjectDBContext())
            {
                if(sortedBy == 1)
                {
                    model.Tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.importance_id);
                    model.MyTasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.importance_id).Where(e => e.employee.user_id == userid);

                }
                else
                    if(sortedBy == 2)
                    {
                        model.Tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.endDate);
                        model.MyTasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.endDate).Where(e => e.employee.user_id == userid);
                    }
                    else
                    {
                        model.Tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().OrderBy(e => e.task_name);
                        model.MyTasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).ToList().Where(e => e.employee.user_id == userid).OrderBy(e => e.task_name);
                    }
            }
            return View(model);
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
                Guid pm = new Guid("6533434A-2EB6-4B23-8C5A-8CC5CC5EA57D");
                Guid dev = new Guid("24957F95-79A8-4B49-B3E5-C113C04C9939");
                taskModel = new TaskViewModel
                {
                    employees = _context.employees.Where(e => e.job_id == pm  || e.job_id == dev ).ToList(),
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
           return RedirectToAction("Details", "Task", new { id = newTask.task_id });
        }

        public ActionResult Edit(Guid id)
        {
            task task = new task();
            Guid pm = new Guid("6533434A-2EB6-4B23-8C5A-8CC5CC5EA57D");
            Guid dev = new Guid("24957F95-79A8-4B49-B3E5-C113C04C9939");
                 
            using (var _context = new ProjectDBContext())
            {
                task = _context.tasks.FirstOrDefault(e => e.task_id == id);
                task.employees = _context.employees.Where(e => e.job_id == pm || e.job_id == dev).ToList();
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
                return RedirectToAction("Details", "Task", new { id = taskDB.task_id });
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

        public ActionResult TaskPrioritization(int period)
        {
            List<task> finale = new List<task>();
            using (var _context = new ProjectDBContext())
            {
                TaskViewModel model = new TaskViewModel();
                string user_id = User.Identity.GetUserId();

                employee emp = _context.employees.FirstOrDefault(e => e.user_id == user_id);

                List<task> tasks = _context.tasks.Include(e => e.employee).Include(e => e.project).Include(e => e.status).Include(e => e.importance).Where(e => e.time <= period / 2).Where(e => e.employee_id == emp.employee_id).OrderBy(e => e.time).ToList();

                int nr = tasks.Count;
                int auxnr = nr + 1;

                int[] time = new int[auxnr];

                for (int i = 1; i <= nr; i++)
                    time[i] = tasks[i - 1].time ?? default(int);

                int[][] matrix = new int[nr + 1][];
                for (int i = 0; i <= nr; i++)
                    matrix[i] = new int[period + 1];

                for (int i = 0; i <= period; i++)
                {
                    matrix[0][i] = 0;
                    for (int j = 1; j <= nr; j++)
                    {
                        matrix[j][0] = 0;
                        for (int gr = 1; gr <= period; gr++)
                        {
                            if (time[j] <= gr)
                            {
                                int castig = 10 / tasks[j - 1].importance_id ?? default(int);
                                if (castig + matrix[j - 1][gr - time[j]] > matrix[j - 1][gr])
                                {
                                    matrix[j][gr] = castig + matrix[j - 1][gr - time[j]];
                                }
                                else
                                {
                                    matrix[j][gr] = matrix[j - 1][gr];
                                }
                            }
                            else
                            {
                                matrix[j][gr] = matrix[j - 1][gr];
                            }
                        }
                    }
                }
                int max = matrix[nr][period];
                int grr = period;
                int ii = nr;

                while (grr > 0 && ii > 0)
                {
                    int castig = 10 / tasks[ii - 1].importance_id ?? default(int);
                    if (time[ii] <= grr)
                        if (matrix[ii][grr] == castig + matrix[ii - 1][grr - time[ii]])
                        {
                            //System.out.print(ii + " ");
                            finale.Add(tasks[ii - 1]);
                            grr = grr - time[ii];

                        }
                    ii--;
                }
            }
            return View(finale);
        }
    }
}