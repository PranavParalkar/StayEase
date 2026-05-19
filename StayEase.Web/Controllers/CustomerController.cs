using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly CustomerService _svc = new();

        public IActionResult Index(string? search)
        {
            var list = string.IsNullOrEmpty(search) ? _svc.GetAllCustomers() : _svc.SearchCustomers(search);
            ViewBag.Search = search;
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Customer { CustomerID = _svc.GenerateID() });

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FullName)) { ViewBag.Error = "Name required."; return View(customer); }
            _svc.AddCustomer(customer);
            TempData["Success"] = "Customer added!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var c = _svc.GetAllCustomers().FirstOrDefault(x => x.CustomerID == id);
            return c == null ? NotFound() : View(c);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            _svc.UpdateCustomer(customer);
            TempData["Success"] = "Customer updated!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _svc.DeleteCustomer(id);
            TempData["Success"] = "Customer deleted.";
            return RedirectToAction("Index");
        }
    }
}
