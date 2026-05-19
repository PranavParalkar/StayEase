using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly InvoiceService _svc = new();

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetRevenueByRoomType()
        {
            var dt = _svc.GetRevenueByRoomType();
            var result = new List<object>();
            foreach (System.Data.DataRow row in dt.Rows)
                result.Add(new { type = row["RoomType"].ToString(), revenue = Convert.ToInt32(row["Revenue"]) });
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetRevenueByDate(DateTime from, DateTime to)
        {
            var dt = _svc.GetRevenueByDateRange(from, to);
            var result = new List<object>();
            foreach (System.Data.DataRow row in dt.Rows)
                result.Add(new { date = Convert.ToDateTime(row["PayDate"]).ToString("yyyy-MM-dd"), revenue = Convert.ToInt32(row["RoomRevenue"]) });
            return Json(result);
        }
    }
}
