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
    public class ClientController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<client> clients;
            using (var _context = new ProjectDBContext())
            {
                clients = _context.clients.Include(e => e.employee).ToList();
            }
            return View(clients);
        }

        public ActionResult Details(Guid id)
        {
            ClientViewModel client = new ClientViewModel();
            using (var _context = new ProjectDBContext())
            {
                client.client = _context.clients.Include(e => e.employee).FirstOrDefault(e => e.client_id == id);
                client.projects = _context.projects.Where(e => e.client_id == id).ToList();
            }
            if (client.client == null)
            {
                RedirectToAction("Index", "Project");// HttpNotFound();
            }
            return View(client);
        }

        public ActionResult New()
        {
            ClientViewModel clientModel;
            using (var _context = new ProjectDBContext())
            {
                clientModel = new ClientViewModel
                {
                    employees = _context.employees.ToList(),
                };
            }
            return View(clientModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                using (var _context = new ProjectDBContext())
                {
                    model.employees = _context.employees.ToList();
                }
                return View("New", model);
            }
            client newclient;
            using (var _context = new ProjectDBContext())
            {
                newclient = new client
                {
                    client_id = Guid.NewGuid(),
                    client_name = model.client.client_name,
                    adress = model.client.adress,
                    contactPhone = model.client.contactPhone,
                    salesPerson = model.client.salesPerson
                };
                _context.clients.Add(newclient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit", "Project", new { id = newclient.client_id });
        }

        public ActionResult Edit(Guid id)
        {
            client client = new client();
            using (var _context = new ProjectDBContext())
            {
                client = _context.clients.Include(e => e.employee).FirstOrDefault(e => e.client_id == id);
                client.employees = _context.employees.ToList();
            }
            if (client == null)
            {
                //RedirectToAction("Index", "Department");
                return HttpNotFound();
            }
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(client clt)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Edit", "Client", clt.client_id);
            }
            client clientDB;
            using (var _context = new ProjectDBContext())
            {
                clientDB = _context.clients.Single(e => e.client_id == clt.client_id);
                TryUpdateModel(clientDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Client");
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var client = new client();
            using (var _context = new ProjectDBContext())
            {
                client = _context.clients.Include(e => e.employee).SingleOrDefault(e => e.client_id == id);
            }
            if (client == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(client clt)
        {
            using (var _context = new ProjectDBContext())
            {
                client client;
                IEnumerable<project> projects = _context.projects.Where(e => e.client_id == clt.client_id).ToList();
                if (projects.Count() == 0)
                {
                    client = _context.clients.SingleOrDefault(e => e.client_id == clt.client_id);
                    if (client == null)
                    {
                        return RedirectToAction("Index");
                    }
                    try
                    {
                        _context.clients.Remove(client);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        return RedirectToAction("Delete", new { id = clt.client_id, saveChangesError = true });
                    }
                }
                else
                    return RedirectToAction("Delete", new { id = clt.client_id, saveChangesError = true }); //CU EROARE CA ARE EMPLOYEES ASOCIATI;                
            }
        }
    }
}