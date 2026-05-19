using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly HotelServiceService _svc = new();

        public IActionResult Index(string? search, string? category)
        {
            var list = _svc.GetAllServices();
            if (!string.IsNullOrEmpty(search))
                list = _svc.SearchServices(search);
            if (!string.IsNullOrEmpty(category) && category != "All")
                list = list.Where(s => s.ServiceType == category).ToList();
            ViewBag.Search = search;
            ViewBag.Category = category;
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Service { ServiceID = _svc.GenerateID() });

        [HttpPost]
        public IActionResult Create(Service svc)
        {
            if (string.IsNullOrWhiteSpace(svc.ServiceName)) { ViewBag.Error = "Name required."; return View(svc); }
            _svc.AddService(svc);
            TempData["Success"] = "Service added!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var s = _svc.GetAllServices().FirstOrDefault(x => x.ServiceID == id);
            return s == null ? NotFound() : View(s);
        }

        [HttpPost]
        public IActionResult Edit(Service svc)
        {
            _svc.UpdateService(svc);
            TempData["Success"] = "Service updated!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _svc.DeleteService(id);
            TempData["Success"] = "Service deleted.";
            return RedirectToAction("Index");
        }
    }
}
