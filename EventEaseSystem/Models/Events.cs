﻿using System.ComponentModel.DataAnnotations;

namespace EventEaseSystem.Models
{
    public class Events
    {
        [Key]
        [Required]
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Details { get; set; } // description
        public int? VenueID { get; set; } // foreign key
        public Venues? Venue { get; set; }
     
        public int EventTypeId { get; set; }
        public EventType? EventType { get; set; }               



    }
}
