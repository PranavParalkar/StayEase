using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.Web.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly RoomService _svc = new();

        public IActionResult Index(string? search, int? status, int? type)
        {
            var rooms = _svc.GetAllRooms();
            if (!string.IsNullOrEmpty(search))
                rooms = _svc.SearchRooms(search);
            if (status.HasValue && status >= 0)
                rooms = rooms.Where(r => r.Status == status).ToList();
            if (type.HasValue && type >= 0)
                rooms = rooms.Where(r => r.RoomType == type).ToList();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Type = type;
            return View(rooms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Room { RoomID = _svc.GenerateID() });
        }

        [HttpPost]
        public IActionResult Create(Room room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomName))
            {
                ViewBag.Error = "Room name is required.";
                return View(room);
            }
            _svc.AddRoom(room);
            TempData["Success"] = "Room added successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var room = _svc.GetAllRooms().FirstOrDefault(r => r.RoomID == id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost]
        public IActionResult Edit(Room room)
        {
            _svc.UpdateRoom(room);
            TempData["Success"] = "Room updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _svc.DeleteRoom(id);
            TempData["Success"] = "Room deleted.";
            return RedirectToAction("Index");
        }
    }
}
