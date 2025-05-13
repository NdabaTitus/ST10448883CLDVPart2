using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseSystem.Models
{
    public class Venue
    {
        [Key]
        [Required]
        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }

        public string? ImageUrl { get; set; } // store uploaded image

        [NotMapped]

        public IFormFile? ImageFile { get; set; }
    }
}
