using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.UI.DataVisualization.Charting;
using Color = System.Drawing.Color;
//using System.Drawing;

namespace WebApplication1.Controllers
{
    public class ChartController : Controller
    {
        public PdfPTable RaportData(PdfPTable tableLayout, List<ProjectViewModel> projects)
        {

            //TABELUL CU PROIECTE PENTRU ANGAJATUL CURENT
            float[] headers = { 25, 50, 40, 10 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 90; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            tableLayout.SpacingBefore = 10;
            tableLayout.SpacingAfter = 20;

            AddCellToHeader(tableLayout, "Project name");
            AddCellToHeader(tableLayout, "Project manager");
            AddCellToHeader(tableLayout, "Client");
            AddCellToHeader(tableLayout, "Number of tasks");
            foreach (var pr in projects)
            {
                AddCellToBody(tableLayout, pr.project.project_name);
                AddCellToBody(tableLayout, pr.project.employee.first_name + " " + pr.project.employee.last_name);
                AddCellToBody(tableLayout, pr.project.client.client_name);
                AddCellToBody(tableLayout, pr.tasks.Count().ToString());
            }
            return tableLayout;
        }

        public FileResult CreatePdf(Guid id)
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;

            //file name to be created   
            string strPDFFileName = string.Format("HR Raport " + dTime.ToString("yyyyMMdd") + ".pdf");
            Document doc = new Document();
            doc.SetMargins(2f, 2f, 2f, 2f);

            //PdfWriter writer = PdfWriter.GetInstance(doc, workStream);

            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();


            //--------------------GET THE DATA FOR THE EMPLOYEE--------------------------------------------     
            employee employee;
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            List<task> activeTasks = new List<task>();
            Dictionary<Guid, ProjectTasksViewModel> proj = new Dictionary<Guid, ProjectTasksViewModel>();

            using (var _context = new ProjectDBContext())
            {
                employee = _context.employees.Include(e => e.job).Include(e => e.AspNetUser).Include(e => e.department).Include(e => e.employee1).FirstOrDefault(e => e.employee_id == id);

                var projectsGroups = from p in _context.projects
                                     join t in _context.tasks on p.project_id equals t.project_id
                                     where t.employee_id == id
                                     group t by t.project_id into g
                                     select g;
                foreach (var group in projectsGroups)
                {
                    ProjectViewModel model = new ProjectViewModel();
                    List<task> tasks = new List<task>();
                    model.project = _context.projects.Include(e => e.employee).Include(e => e.client).FirstOrDefault(e => e.project_id == group.Key);
                    foreach (var task in group)
                    {
                        tasks.Add(task);
                    }
                    model.tasks = tasks;
                    projects.Add(model);
                }

                activeTasks = _context.tasks.Where(e => e.employee_id == id).ToList();

                var allTasks = from p in _context.projects
                               join t in _context.tasks on p.project_id equals t.project_id
                               group t by t.project_id into g
                               select g;

                foreach (var t in allTasks)
                {
                    ProjectTasksViewModel project = new ProjectTasksViewModel();
                    project.project = _context.projects.FirstOrDefault(p => p.project_id == t.Key);
                    project.nrOfTasks = t.Count();
                    proj.Add(t.Key, project);
                }

            }

            //--------------------------datele de la inceput------------------------------


            PdfPTable table = new PdfPTable(1);
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            //table.SetWidths(new float[] { 1f });
            table.WidthPercentage = 90;

            DateTime Time = DateTime.Now;
            string raportName = string.Format(Time.ToString("yyyyMMdd"));

            table.AddCell(new PdfPCell(new Phrase("HR Raport - " + raportName, new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            //Name and function
            Phrase phrase = new Phrase();
            phrase.Add(new Chunk(employee.first_name + " " + employee.last_name + "\n", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            phrase.Add(new Chunk(employee.job.job_title.ToString() + " - " + employee.department.department_name + " " + "Department", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)));

            PdfPCell cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.PaddingBottom = 8;
            cell.PaddingLeft = 20;

            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            table.AddCell(cell);
            doc.Add(table);


            //--------------------------tabel cu datele angajatului----------------------------------------

            table = new PdfPTable(2);
            table.SetWidths(new float[] { 3f, 5f });
            table.TotalWidth = 340f;
            table.LockedWidth = true;
            table.SpacingBefore = 5f;
            table.HorizontalAlignment = Element.ALIGN_LEFT;

            //Email
            table.AddCell(PhraseCell(new Phrase("Email:", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            table.AddCell(PhraseCell(new Phrase(employee.email, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
            cell.Colspan = 2;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);

            //Phone number
            table.AddCell(PhraseCell(new Phrase("Phone number:", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            phrase = new Phrase(new Chunk(employee.phone_number + "\n", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)));
            table.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_LEFT));
            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
            cell.Colspan = 2;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);

            //Date of Birth
            table.AddCell(PhraseCell(new Phrase("Date of Birth:", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            table.AddCell(PhraseCell(new Phrase(Convert.ToDateTime(employee.birth_date).ToString("dd MMMM, yyyy"), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
            cell.Colspan = 2;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);

            //Hire Date
            table.AddCell(PhraseCell(new Phrase("Hire date:", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            table.AddCell(PhraseCell(new Phrase(Convert.ToDateTime(employee.hire_date).ToString("dd MMMM, yyyy"), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
            cell.Colspan = 2;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);

            //Salary
            decimal a = (decimal)employee.salary;
            table.AddCell(PhraseCell(new Phrase("Salary:", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            table.AddCell(PhraseCell(new Phrase(a.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
            cell.Colspan = 2;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);

            //Manager
            table.AddCell(PhraseCell(new Phrase("Manager:", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            table.AddCell(PhraseCell(new Phrase(employee.employee1.last_name + " " + employee.employee1.first_name, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
            cell.Colspan = 2;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);

            //User
            table.AddCell(PhraseCell(new Phrase("Platform username:", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            table.AddCell(PhraseCell(new Phrase(employee.AspNetUser.UserName, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
            cell.Colspan = 2;
            cell.PaddingBottom = 20;
            table.AddCell(cell);

            doc.Add(table);

            //--------------------------tabel cu proiectele angajatului----------------------------------------

            //Create PDF Table FOR POJECTS with 5 columns  
            PdfPTable tableLayout = new PdfPTable(4);

            //Populate the doc with the table filled with data
            doc.Add(RaportData(tableLayout, projects));

            var image = Image.GetInstance(Chart(proj, projects));
            image.ScalePercent(80f);
            image.Alignment = Element.ALIGN_MIDDLE;

            doc.Add(image);

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", strPDFFileName);

        }

        private Byte[] Chart(Dictionary<Guid, ProjectTasksViewModel> ListOfProjects, List<ProjectViewModel> EmployeeProjects)
        {
            var chart = new Chart
            {
                Width = 400,
                Height = 450,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            chart.Titles.Add("Number of task for employee and project");
            chart.Titles[0].Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            chart.Titles[0].Font = new System.Drawing.Font("Arial", 12f);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.Title = "Number of taks";
            chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 10f);
            chart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 10f);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 8f);
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;

            chart.ChartAreas[0].BackColor = Color.White;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Bar;

            foreach (var q in EmployeeProjects)
            {
                var EmpName = "emp";
                chart.Series[0].Points.AddXY(EmpName, Convert.ToDouble(q.tasks.Count()));

                var TotName = "total";
                chart.Series[0].Points.AddXY(TotName, Convert.ToDouble(ListOfProjects[q.project.project_id].nrOfTasks));

                var space = "";
                chart.Series[0].Points.AddXY(space, 0);
            }
            using (var chartimage = new MemoryStream())
            {
                chart.SaveImage(chartimage, ChartImageFormat.Png);
                return chartimage.GetBuffer();
            }
        }

        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 5;
            cell.PaddingTop = 0f;
            cell.PaddingLeft = 35;
            return cell;
        }

        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(119, 136, 153)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }


    }
}