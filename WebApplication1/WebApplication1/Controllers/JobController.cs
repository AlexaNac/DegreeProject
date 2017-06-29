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
    public class JobController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<job> jobs;
            using (var _context = new ProjectDBContext())
            {
                jobs = _context.jobs.ToList();
            }
            return View(jobs);
        }

        public ActionResult Details(Guid id)
        {
            job job = new job();
            using (var _context = new ProjectDBContext())
            {
                job = _context.jobs.FirstOrDefault(e => e.job_id == id);
                job.employees = _context.employees.Where(e => e.job_id == id).ToList();
            }
            if (job == null)
            {
                RedirectToAction("Index", "Job");// HttpNotFound();
            }
            return View(job);
        }

        public ActionResult New()
        {
            job jobModel;
            using (var _context = new ProjectDBContext())
            {
                jobModel = new job
                {
                    employees = _context.employees.ToList(),
                };
            }
            return View(jobModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(job model)
        {
            if (!ModelState.IsValid)
            {
                using (var _context = new ProjectDBContext())
                {
                    model.employees = _context.employees.ToList();
                }
                return View("New", model);
            }
            job newjob;
            using (var _context = new ProjectDBContext())
            {
                newjob = new job
                {
                    job_id = Guid.NewGuid(),
                    job_title = model.job_title,
                    min_salary = model.min_salary,
                    max_salary = model.max_salary
                };
                _context.jobs.Add(newjob);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Job", new { id = newjob.job_id });
        }

        public ActionResult Edit(Guid id)
        {
            job job = new job();
            using (var _context = new ProjectDBContext())
            {
                job = _context.jobs.FirstOrDefault(e => e.job_id == id);
                job.employees = _context.employees.ToList();
            }
            if (job == null)
            {
                //RedirectToAction("Index", "Department");
                return HttpNotFound();
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(job dep)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Edit", "Job", dep.job_id);
            }
            job jobDB;
            using (var _context = new ProjectDBContext())
            {
                jobDB = _context.jobs.Single(e => e.job_id == dep.job_id);
                TryUpdateModel(jobDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Job", new { id = jobDB.job_id });
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var job = new job();
            using (var _context = new ProjectDBContext())
            {
                job = _context.jobs.SingleOrDefault(e => e.job_id == id);
            }
            if (job == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(job jb)
        {
            using (var _context = new ProjectDBContext())
            {
                job job;
                IEnumerable<employee> employees = _context.employees.Where(e => e.job_id == jb.job_id).ToList();
                if (employees.Count() == 0)
                {
                    job = _context.jobs.SingleOrDefault(e => e.job_id == jb.job_id);
                    if (job == null)
                    {
                        return RedirectToAction("Index");
                    }
                    try
                    {
                        _context.jobs.Remove(job);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        return RedirectToAction("Delete", new { id = jb.job_id, saveChangesError = true });
                    }
                }
                else
                    return RedirectToAction("Delete", new { id = jb.job_id, saveChangesError = true }); //CU EROARE CA ARE EMPLOYEES ASOCIATI;                
            }
        }
    }
}