using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _svc = new();

        public IActionResult Index(string? search)
        {
            var list = string.IsNullOrEmpty(search) ? _svc.GetAllEmployees() : _svc.SearchEmployees(search);
            ViewBag.Search = search;
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Employee { EmployeeID = _svc.GenerateID() });

        [HttpPost]
        public IActionResult Create(Employee emp)
        {
            if (string.IsNullOrWhiteSpace(emp.FullName)) { ViewBag.Error = "Name required."; return View(emp); }
            _svc.AddEmployee(emp);
            TempData["Success"] = "Employee added!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var e = _svc.GetAllEmployees().FirstOrDefault(x => x.EmployeeID == id);
            return e == null ? NotFound() : View(e);
        }

        [HttpPost]
        public IActionResult Edit(Employee emp)
        {
            _svc.UpdateEmployee(emp);
            TempData["Success"] = "Employee updated!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _svc.DeleteEmployee(id);
            TempData["Success"] = "Employee deleted.";
            return RedirectToAction("Index");
        }
    }
}
