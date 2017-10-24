using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Schedule.Models.Views
{
    public class EventsIndexViewModel
    {
        public class EventsDay
        {
            public DateTime Date { get; set; }
            public List<Event> Events { get; set; }
        }
        public DateTime Month { get; set; }
        public DateTime NextMonth { get; set; }
        public DateTime PreviousMonth { get; set; }
        public List<Event> Events { get; set; }
        public List<List<EventsDay>> Weeks { get; set; }
        public int? GroupId { get; set; }
    }
}