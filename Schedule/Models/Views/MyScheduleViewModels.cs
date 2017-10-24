using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Schedule.Models.Views
{
    public class MyScheduleIndexViewModel
    {
        public DateTime Month { get; set; }
    }

    public class MyScheduleRequestsAndInvitationsViewModel
    {
        public List<Invitation> Invitations { get; set; }
        public List<Request> Requests { get; set; }
    }
}