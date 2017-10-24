using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Schedule.Models.Views;
using Schedule.Models;

namespace Schedule.Controllers
{
    [Authorize]
    public class MyScheduleController : Controller
    {
        private SheduleDbContext db = new SheduleDbContext();

        // GET: MySchedule
        public ActionResult Index(int? month, int? year)
        {
            DateTime monthDate = DateTime.Now;
            monthDate = new DateTime(year ?? monthDate.Year, month ?? monthDate.Month, 1);

            MyScheduleIndexViewModel model = new MyScheduleIndexViewModel
            {
                Month = monthDate
            };
            return View(model);
        }

        // GET: MySchedule/Notifications
        public ActionResult Notifications()
        {
            var userId = User.Identity.GetUserId();
            var notifications = from n in db.Notifications
                                where n.UserId == userId
                                select n;
            return View(notifications.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkNotificationAsSeen(int id)
        {
            var userId = User.Identity.GetUserId();
            Notification notification = db.Notifications.Find(id);
            if(notification == null || notification.UserId != userId)
            {
                //bad request
            }
            notification.Seen = true;
            db.SaveChanges();

            return RedirectToAction("Notifications");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNotification(int id)
        {
            var userId = User.Identity.GetUserId();
            Notification notification = db.Notifications.Find(id);
            if (notification == null || notification.UserId != userId)
            {
                //bad request
            }
            db.Notifications.Remove(notification);
            db.SaveChanges();

            return RedirectToAction("Notifications");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelRequest(int id)
        {
            var userId = User.Identity.GetUserId();
            Request request = db.Requests.Find(id);
            if (request == null || request.RequestingUserId != userId || request.Status != "Waiting")
            {
                //bad request
            }
            request.Canceled = true;
            db.SaveChanges();

            return RedirectToAction("RequestsAndInvitations");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRequest(int id)
        {
            var userId = User.Identity.GetUserId();
            Request request = db.Requests.Find(id);
            if (request == null || request.RequestingUserId != userId)
            {
                //bad request
            }
            db.Requests.Remove(request);
            db.SaveChanges();

            return RedirectToAction("RequestsAndInvitations");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptInvitation(int id)
        {
            var userId = User.Identity.GetUserId();
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null || invitation.InvitedUserId != userId || invitation.Status != "Waiting")
            {
                //bad request
            }

            Membership membership = db.Memberships.Find(userId, invitation.GroupId);
            if (membership == null)
            {
                membership = new Membership
                {
                    UserId = userId,
                    GroupId = invitation.GroupId,
                    GroupRoleId = 1
                };
                db.Memberships.Add(membership);
                invitation.Accepted = true;
                string groupName = db.Groups.Find(invitation.GroupId).Name;
                string userName = User.Identity.Name;
                Notification notification = new Notification
                {
                    Date = DateTime.Now,
                    UserId = invitation.InvitingUserId,
                    Decription = userName + " accepted your invitation to group " + groupName,
                    Seen = false
                };
                db.Notifications.Add(notification);
                db.SaveChanges();
            }

            return RedirectToAction("RequestsAndInvitations");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectInvitation(int id)
        {
            var userId = User.Identity.GetUserId();
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null || invitation.InvitedUserId != userId || invitation.Status != "Waiting")
            {
                //bad request
            }

            invitation.Rejected = true;
            string groupName = db.Groups.Find(invitation.GroupId).Name;
            string userName = User.Identity.Name;
            Notification notification = new Notification
            {
                Date = DateTime.Now,
                UserId = invitation.InvitingUserId,
                Decription = userName + " rejected your invitation to group " + groupName,
                Seen = false
            };
            db.Notifications.Add(notification);
            db.SaveChanges();

            return RedirectToAction("RequestsAndInvitations");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteInvitation(int id)
        {
            var userId = User.Identity.GetUserId();
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null || invitation.InvitedUserId != userId)
            {
                //bad request
            }

            if (invitation.Status == "Waiting")
            {
                string groupName = db.Groups.Find(invitation.GroupId).Name;
                string userName = User.Identity.Name;
                Notification notification = new Notification
                {
                    Date = DateTime.Now,
                    UserId = invitation.InvitingUserId,
                    Decription = userName + " rejected your invitation to group " + groupName,
                    Seen = false
                };
                db.Notifications.Add(notification);
            }
            db.Invitations.Remove(invitation);
            db.SaveChanges();

            return RedirectToAction("RequestsAndInvitations");
        }

        // GET: MySchedule/RequestsAndInvitations
        public ActionResult RequestsAndInvitations()
        {
            var userId = User.Identity.GetUserId();
            var requests = from r in db.Requests
                           where r.RequestingUserId == userId
                           select r;
            var invitations = from i in db.Invitations
                              where i.InvitedUserId == userId
                              select i;

            MyScheduleRequestsAndInvitationsViewModel model = new MyScheduleRequestsAndInvitationsViewModel
            {
                Requests = requests.ToList(),
                Invitations = invitations.ToList()
            };

            return View(model);
        }
    }
}
