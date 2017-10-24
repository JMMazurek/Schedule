using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Schedule.Models.Views
{
    public class GroupsGroupEventsModel
    {
        public DateTime Month { get; set; }
        public Group Group { get; set; }
    }

    public class GroupsRequestsAndInvitationsViewModel
    {
        public List<Invitation> Invitations { get; set; }
        public List<Request> Requests { get; set; }
        public int GroupId { get; set; }
    }

    public class GroupsFindUserViewModel
    {
        public string UserName { get; set; }
        public int GroupId { get; set; }
        public List<User> Users { get; set; }
    }

    public class GroupsFindGroupViewModel
    {
        public string Name { get; set; }
        public List<Group> Groups { get; set; }
    }
}