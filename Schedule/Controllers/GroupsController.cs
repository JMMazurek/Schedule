using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Schedule.Models;
using Schedule.Models.Views;

namespace Schedule.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private SheduleDbContext db = new SheduleDbContext();

        // GET: Groups
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            List<Group> groups = (from m in db.Memberships.Include(m => m.Group)
                                 where m.UserId == userID
                                 select m.Group).ToList();
            return View(groups);
        }

        public ActionResult FindGroup(string name)
        {
            var groups = from g in db.Groups
                         where g.Name.Contains(name)
                         && g.Searchable
                         select g;

            GroupsFindGroupViewModel model = new GroupsFindGroupViewModel
            {
                Groups = groups.ToList(),
                Name = name
            };

            return View(model);
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id.Value);
            if (group == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            Membership membership = db.Memberships.Find(userId, id.Value);
            if (membership == null && !group.Searchable)
                return HttpNotFound();

            ViewBag.groupId = id.Value;

            if (membership != null)
            {
                ViewBag.GroupAction = "Leave";
            }
            else if(group.NeedConfirmation)
            {
                ViewBag.GroupAction = "Request";
            }
            else
            {
                ViewBag.GroupAction = "Join";
            }

            return View(group);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Searchable,NeedConfirmation,Public,Name,Description")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                var userId = User.Identity.GetUserId();
                Membership membership = new Membership
                {
                    Group = group,
                    GroupRole = db.GroupRoles.Where(gr => gr.Name == "Administrator").First(),
                    UserId = userId
                };
                db.Memberships.Add(membership);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            Membership membership = db.Memberships.Find(userId, id.Value);

            if (membership == null || !membership.GroupRole.CanManageGroup)
                return HttpNotFound();

            ViewBag.groupId = id.Value;
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Searchable,NeedConfirmation,Public,Name,Description")] Group group)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                Membership membership = db.Memberships.Find(userId, group.Id);

                if (membership == null || !membership.GroupRole.CanManageGroup)
                    return HttpNotFound();

                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            Membership membership = db.Memberships.Find(userId, id.Value);

            if (membership == null || !membership.GroupRole.CanManageGroup)
                return HttpNotFound();

            ViewBag.groupId = id.Value;

            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            Membership membership = db.Memberships.Find(userId, id);

            if (membership == null || !membership.GroupRole.CanManageGroup)
                return HttpNotFound();
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Groups/GroupEvents/...
        public ActionResult GroupEvents(int? month, int? year, int? group)
        {
            var userID = User.Identity.GetUserId();
            Group groupObject;
            if (group.HasValue)
            {
                groupObject = (from g in db.Groups
                               where g.Id == @group.Value
                               select g).FirstOrDefault();
                if (groupObject == null)
                    return HttpNotFound();
            }
            else
            {
                return HttpNotFound();
            }

            ViewBag.groupId = group.Value;

            if (!groupObject.Public)
            {
                var membership = (from m in db.Memberships
                                  where m.UserId == userID
                                  && m.GroupId == @group.Value
                                  select m).FirstOrDefault();
                if (membership == null)
                {
                    return HttpNotFound();
                }
            }

            DateTime monthDate = DateTime.Now;
            monthDate = new DateTime(year ?? monthDate.Year, month ?? monthDate.Month, 1);


            GroupsGroupEventsModel model = new GroupsGroupEventsModel
            {
                Month = monthDate,
                Group = groupObject
            };

            return View(model);
        }

        public ActionResult RequestsAndInvitations(int? group)
        {
            if(!group.HasValue)
            {
                return HttpNotFound();
            }
            var requests = from r in db.Requests
                           where r.GroupId == @group
                           && !r.Accepted && !r.Rejected && !r.Canceled
                           select r;
            var invitations = from i in db.Invitations
                              where i.GroupId == @group
                              && !i.Accepted && !i.Rejected && !i.Canceled
                              select i;

            GroupsRequestsAndInvitationsViewModel model = new GroupsRequestsAndInvitationsViewModel
            {
                Requests = requests.ToList(),
                Invitations = invitations.ToList(),
                GroupId = group.Value
            };

            ViewBag.groupId = group.Value;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptRequest(int id)
        {
            var userId = User.Identity.GetUserId();
            Models.Request request = db.Requests.Find(id);

            Membership m = db.Memberships.Find(userId, request.GroupId);

            if (request == null || request.Status != "Waiting")
            {
                return HttpNotFound();
            }

            if (m == null || !m.GroupRole.CanManageUsers)
                return HttpNotFound();

            Membership membership = db.Memberships.Find(request.RequestingUserId, request.GroupId);
            if (membership == null)
            {
                membership = new Membership
                {
                    UserId = request.RequestingUserId,
                    GroupId = request.GroupId,
                    GroupRole = db.GroupRoles.Where(gr => gr.Name == "User").First()
                };
                db.Memberships.Add(membership);
                request.Accepted = true;
                string groupName = db.Groups.Find(request.GroupId).Name;
                string userName = User.Identity.Name;
                Notification notification = new Notification
                {
                    Date = DateTime.Now,
                    UserId = request.RequestingUserId,
                    Decription = userName + " accepted your request to group " + groupName,
                    Seen = false
                };
                db.Notifications.Add(notification);
                db.SaveChanges();
            }

            return RedirectToAction("RequestsAndInvitations", new { group = request.GroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectRequest(int id)
        {
            var userId = User.Identity.GetUserId();
            Request request = db.Requests.Find(id);
            
            if (request == null || request.Status != "Waiting")
            {
                return HttpNotFound();
            }

            Membership m = db.Memberships.Find(userId, request.GroupId);

            if (m == null || !m.GroupRole.CanManageUsers)
                return HttpNotFound();

            request.Rejected = true;
            string groupName = db.Groups.Find(request.GroupId).Name;
            string userName = User.Identity.Name;
            Notification notification = new Notification
            {
                Date = DateTime.Now,
                UserId = request.RequestingUserId,
                Decription = userName + " rejected your invitation to group " + groupName,
                Seen = false
            };
            db.Notifications.Add(notification);
            db.SaveChanges();

            return RedirectToAction("RequestsAndInvitations", new { group = request.GroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelInvitation(int id)
        {
            var userId = User.Identity.GetUserId();
            Invitation invitation = db.Invitations.Find(id);
            
            if (invitation == null || invitation.Status != "Waiting")
            {
                return HttpNotFound();
            }

            Membership m = db.Memberships.Find(userId, invitation.GroupId);

            if (m == null || !m.GroupRole.CanManageUsers)
                return HttpNotFound();

            invitation.Canceled = true;
            db.SaveChanges();

            return RedirectToAction("RequestsAndInvitations", new { group = invitation.GroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinGroup(int id)
        {
            var userId = User.Identity.GetUserId();
            Group group = db.Groups.Find(id);

            if (group == null || group.NeedConfirmation)
            {
                HttpNotFound();
            }
            Membership membership = db.Memberships.Find(userId, id);
            
            if(membership == null)
            {
                membership = new Membership
                {
                    UserId = userId,
                    GroupId = id,
                    GroupRole = db.GroupRoles.Where(gr => gr.Name == "User").First()
                };
                db.Memberships.Add(membership);
                db.SaveChanges();
            }
            
            return RedirectToAction("GroupEvents", new { group = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(int groupId, string userId)
        {
            var currentUserId = User.Identity.GetUserId();
            Group group = db.Groups.Find(groupId);
            
            if (group == null)
            {
                return HttpNotFound();
            }
            User user = db.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            Membership m = db.Memberships.Find(currentUserId, groupId);

            if (m == null || !m.GroupRole.CanManageUsers)
                return HttpNotFound();

            Membership membership = db.Memberships.Find(userId, groupId);

            if (membership == null)
            {
                Invitation invitation = new Invitation
                {
                    InvitingUserId = currentUserId,
                    InvitedUserId = userId,
                    Date = DateTime.Now,
                    GroupId = groupId
                };
                string groupName = group.Name;
                string userName = User.Identity.Name;
                Notification notification = new Notification
                {
                    Date = DateTime.Now,
                    UserId = userId,
                    Decription = userName + " invited you to group " + groupName,
                    Seen = false
                };
                db.Notifications.Add(notification);
                db.Invitations.Add(invitation);
                db.SaveChanges();
            }
            
            return RedirectToAction("RequestsAndInvitations", new { group = groupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestGroup(int id)
        {
            var userId = User.Identity.GetUserId();
            Group group = db.Groups.Find(id);
 
            if (group == null || !group.NeedConfirmation || !group.Searchable)
            {
                return HttpNotFound();
            }
            Membership membership = db.Memberships.Find(userId, id);

            if (membership == null)
            {
                Models.Request request = new Models.Request
                {
                    Date = DateTime.Now,
                    RequestingUserId = userId,
                    GroupId = id
                };
                
                db.Requests.Add(request);
                db.SaveChanges();
            }

            return RedirectToAction("RequestsAndInvitations", "MySchedule");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveGroup(int id)
        {
            var userId = User.Identity.GetUserId();
            Group group = db.Groups.Find(id);
            
            if (group == null)
            {
                return HttpNotFound();
            }
            Membership membership = db.Memberships.Find(userId, id);

            if (membership == null)
            {
                return HttpNotFound();
            }

            db.Memberships.Remove(membership);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult FindUser(string userName, int? groupId)
        {
            if(!groupId.HasValue)
            {
                return HttpNotFound();
            }
            var users = from u in db.Users
                        let gs = from m in u.Memberships
                                 select m.GroupId
                        where u.UserName.Contains(userName)
                        && !gs.Contains(groupId.Value)
                        select u;

            GroupsFindUserViewModel model = new GroupsFindUserViewModel
            {
                Users = users.ToList(),
                UserName = userName,
                GroupId = groupId.Value
            };

            ViewBag.groupId = groupId.Value;

            return View(model);
        }

        public ActionResult Members(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            Group group = db.Groups.Find(groupId);

            if (group == null)
            {
                return HttpNotFound();
            }

            Membership membership = db.Memberships.Find(userId, groupId.Value);

            if (membership == null)
            {
                return HttpNotFound();
            }

            ViewBag.groupId = groupId.Value;

            var options = from gr in db.GroupRoles
                           select new SelectListItem
                           {
                               Value = gr.Id.ToString(),
                               Text = gr.Name
                           };

            ViewBag.options = options;

            var memberships = group.Memberships.ToList();

            return View(memberships);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveMember(int? groupId, string userId)
        {
            if (!groupId.HasValue || String.IsNullOrEmpty(userId))
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            Group group = db.Groups.Find(groupId);

            if (group == null)
            {
                return HttpNotFound();
            }

            Membership membership = db.Memberships.Find(userId, groupId.Value);
            Membership currentUserMembership = db.Memberships.Find(currentUserId, groupId.Value);

            if (membership == null || currentUserMembership == null || !currentUserMembership.GroupRole.CanManageUsersRoles)
            {
                return HttpNotFound();
            }

            db.Memberships.Remove(membership);
            db.SaveChanges();

            return RedirectToAction("Members", new { groupId = groupId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMembership([Bind(Include = "UserId,GroupId,GroupRoleId")] Membership membership)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                Membership m = db.Memberships.Find(membership.UserId, membership.GroupId);

                Membership currentUserMembership = db.Memberships.Find(currentUserId, membership.GroupId);

                if (m == null || currentUserMembership == null || !currentUserMembership.GroupRole.CanManageUsersRoles)
                {
                    return HttpNotFound();
                }

                m.GroupRoleId = membership.GroupRoleId;
                db.SaveChanges();
            }

            return RedirectToAction("Members", new { groupId = membership.GroupId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
