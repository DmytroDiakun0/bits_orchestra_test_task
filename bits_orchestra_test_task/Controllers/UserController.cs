using bits_orchestra_test_task.Data;
using bits_orchestra_test_task.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace bits_orchestra_test_task.Controllers
{
    public class UserController : Controller
    {
        private readonly UserDbContext dbContext;

        public UserController(UserDbContext context)
        {
            dbContext = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await dbContext.Users.ToListAsync();
            if (users == null || users.Count == 0)
            {
                users = [];
            }
            return View(users);
        }

        /*
         * example of .csv file:
         * 
         * Name,Date of birth,Married,Phone,Salary
         * John Doe,1985-03-15,TRUE,+1234567890,55000.75
         * Jane Smith,1990-07-22,FALSE,+1987654321,62000.00
         * Bob Johnson,1978-11-02,TRUE,+1123456789,47500.50
         * Alice Brown,1983-05-29,FALSE,+1223344556,72000.25
         * Charlie White,1995-12-17,TRUE,+1445566778,48000.00
         */
        public async Task<IActionResult> Create(IFormFile csvFile)
        {
            using var reader = new StreamReader(csvFile.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<UserMap>();

            var records = csv.GetRecords<User>().ToList();

            dbContext.Users.AddRange(records);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateRequest request)
        {
            var user = await dbContext.Users.FindAsync(request.Id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            switch (request.Field)
            {
                case "Name":
                    user.Name = request.Value;
                    break;
                case "DateOfBirth":
                    if (DateTime.TryParse(request.Value, out DateTime date))
                    {
                        user.DateOfBirth = date;
                    }
                    break;
                case "Married":
                    user.Married = request.Value.ToLower() == "true";
                    break;
                case "Phone":
                    user.Phone = request.Value;
                    break;
                case "Salary":
                    if (decimal.TryParse(request.Value, out decimal salary))
                    {
                        user.Salary = salary;
                    }
                    break;
                default:
                    return Json(new { success = false, message = "Invalid field" });
            }

            await dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await dbContext.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        public class UpdateRequest
        {
            public int Id { get; set; }
            public string Field { get; set; }
            public string Value { get; set; }
        }

        public class UserMap : ClassMap<User>
        {
            public UserMap()
            {
                Map(m => m.Name).Name("Name");
                Map(m => m.DateOfBirth).Name("Date of birth");
                Map(m => m.Married).Name("Married");
                Map(m => m.Phone).Name("Phone");
                Map(m => m.Salary).Name("Salary");
            }
        }
    }
}