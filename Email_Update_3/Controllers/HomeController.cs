using Email_Update_3.Models;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Email_Update_Final.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ERPContext _context;

        

        public HomeController(ILogger<HomeController> logger, ERPContext context)
        {
            _logger = logger;

            _context = context;
           

        }

        public IActionResult Index()
        {
            ViewBag.Student = null;

            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file == null)
            {
                ViewBag.msg = "Please Select an Excel File";
                return View();
            }
            var StudentList = new List<Student_Info>();

            using (var Stream = new MemoryStream())
            {
                await file.CopyToAsync(Stream);

                using (var Package = new ExcelPackage(Stream))
                {
                    ExcelWorksheet worksheet = Package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;

                    
                    for (int row = 2; row <= rowcount; row++)
                    {
                        if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                        {

                            Student_Info student = new Student_Info();
                            student.StudentId = worksheet.Cells[row, 1].Value.ToString().Trim();
                            student.Email = worksheet.Cells[row, 2].Value.ToString().Trim();


                            StudentList.Add(student);
                        }

                    }
                }
            }

            List<Student_Info> No_student_Infos = new List<Student_Info>();
            foreach (var student in StudentList)
            {
                var Check_PersonID = _context.SmisStudents.FirstOrDefault(x => x.StudentId == student.StudentId);
                if (Check_PersonID != null)
                {
                    var person = _context.HrmPeople.FirstOrDefault(x => x.PersonId == Check_PersonID.PersonId);


                    if (person != null)
                    {
                        person.Email = student.Email;
                        _context.HrmPeople.Update(person);
                    }


                    person = null;
                    Check_PersonID = null;
                }
                else
                {
                    ViewBag.msg = "Some Students ID did not Exist, Valid students email updated successfully";

                    No_student_Infos.Add(student);
                }

            }


            await _context.SaveChangesAsync();


            if (No_student_Infos.Count == 0)
            {
                ViewBag.msg = "All Email updated Successfully";
                return View();
            }
            else
            {
                return View(No_student_Infos);
            }



        }

        
        

        [HttpPost]
        [Route("/Home/GetStudentByID/{ID?}")]
        public IActionResult GetStudentByID(string ID)
        {
            if (ID == null)
            {
                ViewBag.msg = "Studen Id canot be null";
                return View("SingleEmailUpdate");
            }

            Student_Info student = new Student_Info();
            var StudentId = _context.SmisStudents.FirstOrDefault(x => x.StudentId == ID).StudentId;
            if (StudentId == null)
            {
                ViewBag.msg = "Studen Id does not exist";


                return View("SingleEmailUpdate");
            }
            student.StudentId = StudentId;
            var PersonID = _context.SmisStudents.FirstOrDefault(x => x.StudentId == ID).PersonId;


            student.Email = _context.HrmPeople.FirstOrDefault(x => x.PersonId == PersonID).Email;
            student.StudentName = "";
            ViewBag.msg = "";

            //List<Student_Info> StuList = new List<Student_Info>();
            //StuList.Add(student);


            return View("SingleEmailUpdate", student);
        }

        public IActionResult SingleEmailUpdate()
        {
            return View();
        }

        [HttpGet("/Home/GetAllSemester")]
        public IActionResult GetAllSemester()
        {
            var SemesterList = _context.SmisSemesters.OrderByDescending(x => x.SemesterId).ToList();

            return Json(SemesterList);
        }

        [HttpPost]
        [Route("/Home/UpdateEmail/{student?}")]
        public IActionResult UpdateEmail(Student_Info student)
        {
            if (student.Email == null || student.Email == "")
            {
                ViewBag.msg = "PLease Insert AN EMail";
                return View("SingleEmailUpdate");
            }
            var PersonID = _context.SmisStudents.FirstOrDefault(x => x.StudentId == student.StudentId).PersonId;


            HrmPerson person = _context.HrmPeople.FirstOrDefault(x => x.PersonId == PersonID);

            person.Email = student.Email;


            _context.HrmPeople.Update(person);
            _context.SaveChanges();


            ViewBag.msg = student.StudentId + " email has changed to " + student.Email;
            return View("SingleEmailUpdate");
        }


        public IActionResult EmailChecking()
        {
            ViewBag.SemesterList = _context.SmisSemesters.OrderByDescending(x => x.SemesterId).ToList();
            ViewBag.msg = "There is no email value according to this Semester or Semester is not Selected";

            ViewBag.Url = Request.GetTypedHeaders().Referer.ToString();
            return View();
        }
        [HttpPost]
        [Route("/Home/EmailChecking/{Semester?}")]
        public IActionResult EmailChecking(string Semester)
        {
            if (Semester == "0")
            {
                ViewBag.msg = "Select an Semester";
                return View();
            }
            //var Students = _context.SmisStudents.Where(x=>x.StudentId.Contains(Semester)).ToList();
            var people = _context.SmisStudents.Where(x => x.StudentId.Contains(Semester)).ToList().Join(_context.HrmPeople.ToList(),
                                        student => student.PersonId,
                                        person => person.PersonId,
                                        (student, person) => new Student_Info
                                        {

                                            StudentId = student.StudentId,
                                            StudentName = person.FirstName,
                                            Email = person.Email,
                                            ProgrammeID = student.StudentId.Substring(student.StudentId.IndexOf("-") + 1, 2)



                                        }).Where(x => x.Email == null || x.Email == "").ToList();

           
            ViewBag.SemesterList = _context.SmisSemesters.OrderByDescending(x => x.SemesterId).ToList();
            return View(people);
        }


        public IActionResult SearchByProggramID()
        {
            ViewBag.SemesterList = _context.SmisSemesters.OrderByDescending(x => x.SemesterId).ToList();
            ViewBag.Programme = _context.SmisPrograms.ToList();
            @ViewBag.msg = "";
            return View();
        }

        [HttpPost]
        public IActionResult SearchByProggramID(string Semester, string Programme)
        {
            if (Semester == null || Programme == null || Semester == "" || Programme == "")
            {
                return RedirectToAction("SearchByProggramID");
            }
            var StudentID = Semester + Programme;

            var StudentInfo = _context.SmisStudents.Where(x => x.StudentId.Contains(StudentID)).ToList().Join(_context.HrmPeople.ToList(),
                                        student => student.PersonId,
                                        person => person.PersonId,
                                        (student, person) => new Student_Info
                                        {

                                            StudentId = student.StudentId,
                                            StudentName = person.FirstName,

                                            ProgrammeID = student.StudentId.Substring(student.StudentId.IndexOf("-") + 1, 2),

                                            Email = person.Email,
                                            PhoneNumber = person.PresentPhone,
                                            CampusName = _context.AccommCampuses.FirstOrDefault(x => x.CampusNo == student.FkCampus).CampusName,
                                            Address = person.PresentHouse




                                        }).ToList();

            if (StudentInfo.Count == 0)
            {
                @ViewBag.msg = "No Information Available";
                return View();
            }
            ViewBag.SemesterList = _context.SmisSemesters.OrderByDescending(x => x.SemesterId).ToList();
            ViewBag.Programme = _context.SmisPrograms.ToList();
            return View(StudentInfo);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        //public string GetBaseUrl()
        //{
        //    var request = _currentContext.Request;

        //    var host = request.Host.ToUriComponent();

        //    var pathBase = request.PathBase.ToUriComponent();

        //    return $"{request.Scheme}://{host}{pathBase}";
        //}



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
