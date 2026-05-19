using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly BookingMgmtService _svc = new();
        private readonly CustomerService _custSvc = new();
        private readonly RoomService _roomSvc = new();
        private readonly HotelServiceService _hotelSvc = new();

        public IActionResult Index()
        {
            var bookings = _svc.GetAllBookings();
            return View(bookings);
        }

        public IActionResult Details(string id)
        {
            var booking = _svc.GetAllBookings().FirstOrDefault(b => b.BookingID == id);
            if (booking == null) return NotFound();
            ViewBag.Rooms = _svc.GetBookingRooms(id);
            ViewBag.Services = _svc.GetBookingServices(id);
            return View(booking);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Customers = _custSvc.GetAllCustomers();
            ViewBag.Rooms = _roomSvc.GetAvailableRooms();
            ViewBag.BookingID = _svc.GenerateBookingID();
            return View();
        }

        [HttpPost]
        public IActionResult Create(string bookingId, string customerId, string roomId,
            int rentalType, DateTime checkIn, DateTime checkOut, int rentalPrice, int deposit)
        {
            string empId = User.FindFirst("EmployeeID")?.Value ?? "NV190526001";
            var booking = new Booking
            {
                BookingID = bookingId,
                CustomerID = customerId,
                EmployeeID = empId,
                BookingDate = DateTime.Now,
                Deposit = deposit
            };
            _svc.CreateBooking(booking);

            var br = new BookingRoom
            {
                BookingID = bookingId,
                RoomID = roomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                RentalType = rentalType,
                RentalPrice = rentalPrice
            };
            _svc.AddBookingRoom(br);

            TempData["Success"] = $"Booking {bookingId} created!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CheckIn(string bookingId, string roomId, DateTime checkInDate)
        {
            _svc.CheckIn(bookingId, roomId, checkInDate);
            TempData["Success"] = "Checked in!";
            return RedirectToAction("Details", new { id = bookingId });
        }

        [HttpPost]
        public IActionResult CheckOut(string bookingId, string roomId, DateTime checkInDate)
        {
            _svc.CheckOut(bookingId, roomId, checkInDate);
            TempData["Success"] = "Checked out!";
            return RedirectToAction("Details", new { id = bookingId });
        }

        [HttpGet]
        public IActionResult AddService(string id)
        {
            ViewBag.BookingID = id;
            ViewBag.Services = _hotelSvc.GetAllServices();
            return View();
        }

        [HttpPost]
        public IActionResult AddService(string bookingId, string serviceId, int quantity)
        {
            var svc = _hotelSvc.GetAllServices().First(s => s.ServiceID == serviceId);
            _svc.AddBookingService(new BookingService
            {
                BookingID = bookingId,
                ServiceID = serviceId,
                UsageDate = DateTime.Today,
                Quantity = quantity,
                Price = svc.Price
            });
            TempData["Success"] = "Service added!";
            return RedirectToAction("Details", new { id = bookingId });
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _svc.DeleteBooking(id);
            TempData["Success"] = "Booking deleted.";
            return RedirectToAction("Index");
        }
    }
}
