using EventEaseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEaseSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                b.Venue.VenueName.Contains(searchString) ||
                b.Event.EventName.Contains(searchString));
            }

            return View(await bookings.ToListAsync());

        }

        public async Task<IActionResult> Create()
        {
            ViewBag.EventID = new SelectList( await _context.Events.ToListAsync(),"EventID","EventName");
            ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName"); ;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bookings booking)
        {
            var selectedEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewData["Events"] = _context.Events.ToList();
                ViewData["Venues"] = _context.Venues.ToList();
                return View(booking);
            }

            // Check manually for double booking
            var conflict = await _context.Bookings
                .Include(b => b.Event)
                .AnyAsync(b => b.VenueID == booking.VenueID &&
                               b.Event.EventDate.Date == selectedEvent.EventDate.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewData["Events"] = _context.Events.ToList();
                ViewData["Venues"] = _context.Venues.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If database constraint fails (e.g., unique key violation), show friendly message
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                    ViewData["Events"] = _context.Events.ToList();
                    ViewData["Venues"] = _context.Venues.ToList();
                    return View(booking);
                }
            }

            ViewData["Events"] = _context.Events.ToList();
            ViewData["Venues"] = _context.Venues.ToList();
            return View(booking);
        }
    }
}
