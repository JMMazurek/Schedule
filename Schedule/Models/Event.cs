using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Schedule.Models
{
    public class Event
    {
        public Event()
        {

        }

        public int Id { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Starts { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Ends { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        [Required]
        public string OwnerId { get; set; }
        public virtual User Owner { get; set; }
        public int EventTypeId { get; set; }
        public virtual EventType EventType { get; set; }
    }
}