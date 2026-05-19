using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly RoomService _roomSvc = new();
        private readonly CustomerService _custSvc = new();
        private readonly BookingMgmtService _bookingSvc = new();

        public IActionResult Index()
        {
            ViewBag.TotalRooms = _roomSvc.GetAllRooms().Count;
            ViewBag.AvailableRooms = _roomSvc.GetAvailableRooms().Count;
            ViewBag.TotalCustomers = _custSvc.GetAllCustomers().Count;
            ViewBag.TotalBookings = _bookingSvc.GetAllBookings().Count;
            ViewBag.PendingBookings = _bookingSvc.GetPendingBookings().Count;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
