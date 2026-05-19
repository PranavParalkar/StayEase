using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly InvoiceService _svc = new();
        private readonly BookingMgmtService _bookingSvc = new();

        public IActionResult Index()
        {
            var dt = _svc.GetInvoiceReport();
            return View(dt);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Bookings = _bookingSvc.GetAllBookings();
            ViewBag.InvoiceID = _svc.GenerateID();
            return View();
        }

        [HttpPost]
        public IActionResult Create(string invoiceId, string bookingId, int discount, int surcharge, short paymentMethod)
        {
            var inv = new Invoice
            {
                InvoiceID = invoiceId,
                BookingID = bookingId,
                Discount = discount,
                Surcharge = surcharge,
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod
            };
            _svc.CreateInvoice(inv);
            _bookingSvc.UpdateBookingStatus(bookingId, 1);
            TempData["Success"] = "Invoice created!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetTotals(string bookingId)
        {
            int roomTotal = _svc.GetRoomTotal(bookingId);
            int serviceTotal = _svc.GetServiceTotal(bookingId);
            return Json(new { roomTotal, serviceTotal });
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _svc.DeleteInvoice(id);
            TempData["Success"] = "Invoice deleted.";
            return RedirectToAction("Index");
        }
    }
}
