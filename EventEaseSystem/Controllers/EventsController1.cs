using EventEaseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEaseSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchType,int? venueId,DateTime? startDate, DateTime? endDate)
        {
            var eventsss = _context.Events.Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if(!string.IsNullOrEmpty(searchType))
                eventsss=eventsss.Where(e=>e.EventType.Name == searchType);

            if(venueId.HasValue)
                eventsss=eventsss.Where(e =>e.VenueID ==venueId.Value); 

            if(startDate.HasValue&&endDate.HasValue)
                eventsss=eventsss.Where(e=> e.EventDate >= startDate.Value && e.EventDate<=endDate);

            ViewData["EventType"]=_context.EventType.ToList();
            ViewData["Venue"]=_context.Venues.ToList();
            return View(await eventsss.ToListAsync());
        }

        // reate 1
        public async Task<IActionResult> Create()
        {
            ViewData["EventType"]= _context.EventType.ToList();
            ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Events @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully";
                return RedirectToAction(nameof(Index));
;            }

            ViewData["Venue"] = _context.Venues.ToList();
            ViewData["EventType"] = _context.EventType.ToList();
            return View(@event);
        }

        public async Task<IActionResult> Edit (int? id)
        {
            if(id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            ViewData["EventType"] = _context.EventType.ToList();
            ViewData["Venue"] = _context.Venues.ToList();
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
         public async Task<IActionResult> Edit(int id, Events @event)
        {
            if (id != @event.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventType"] = _context.EventType.ToList();
            ViewData["Venue"] = _context.Venues.ToList();
            return View(@event);
        }

        //DELETE
        public async Task<IActionResult> Delete (int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (@event == null) return NotFound();
            return View(@event);
            }

        // PERFORM DELETION
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            var isBooked = await _context.Bookings.AnyAsync(b => b.EventID == id);
            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has existing bookigs.";
                return RedirectToAction(nameof(Index));
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted sucessfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null) return NotFound();

            return View(@event);
        }
    }

}
