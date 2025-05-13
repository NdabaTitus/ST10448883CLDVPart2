using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseSystem.Models
{
    public class Bookings
    {
        [Key] // Make sure you have this OR use EF naming convention
        [Required]
        public int BookingID { get; set; }

        public int VenueID { get; set; }

        // Navigation property (optional)
        public Venue? Venue { get; set; }

        [ForeignKey("EventID")]
        public int EventID { get; set; }

        public Events? Event { get; set; }

      

        public DateTime BookingDate { get; set; }
    }
}

