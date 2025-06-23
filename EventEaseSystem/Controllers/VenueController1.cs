using Azure.Storage.Blobs;
using EventEaseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseSystem.Controllers
{
    public class VenueController : Controller
    {


        private readonly ApplicationDbContext _context;

        public VenueController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venues venue)
        {
            if (ModelState.IsValid)
            {


                // Handle image upload to Azure Blob Storage if an image file was provided
                // This is Step 4C: Modify Controller to receive ImageFile from View (user upload)
                // This is Step 5: Upload selected image to Azure Blob Storage
                if (venue.ImageFile != null)
                {

                    // Upload image to Blob Storage (Azure)
                    /**
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile); //Main part of Step 5 B (upload image to Azure Blob Storage)
                    
                    // Step 6: Save the Blob URL into ImageUrl property (the database)
                    venue.ImageUrl = blobUrl;
                    **/
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venues venue)
        {
            if (id != venue.VenueID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        /**
                        // Upload new image if provided
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                        // STep 6
                        // Update Venue.ImageUrl with new Blob URL
                        venue.ImageUrl = blobUrl;
                        **/
                    }
                    else
                    {
                        // Keep the existing ImageUrl (Optional depending on your UI design)
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                   /**
                    if (!VenueExists(venue.VenueID))
                        return NotFound();
                    else
                        throw;
                   **/
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }


        // This is Step 5 (C): Upload selected image to Azure Blob Storage.
        // It completes the entire uploading process inside Step 5 â€” from connecting to Azure to returning the Blob URL after upload.
        // This will upload the Image to Blob Storage Account
        // Uploads an image to Azure Blob Storage and returns the Blob URL
        /***
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=st10448883;AccountKey=NVsWtcrXzjkS5dMlsfUWLj8EiFToubKTyDzLhk1jjNfKzcz2Jo/JkezgrE2iqeNeoGv4YOk9/o74+AStMrodmg==;EndpointSuffix=core.windows.net";
            var containerName = "first";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }
        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }
        ***/

        //STEP 1: Confirm Deletion(GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(v => v.VenueID == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        //STEP 2: Perform Deletion (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete venue because it is in bookings.";
                return RedirectToAction(nameof(Index));

            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully ";
            return RedirectToAction(nameof(Index));

        }
    }
}