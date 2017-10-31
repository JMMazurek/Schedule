using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Schedule.Models;
using Schedule.Models.Views;
using Microsoft.AspNet.Identity;

namespace Schedule.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private SheduleDbContext db = new SheduleDbContext();

        // GET: Events
        public ActionResult Index(int? month, int? year, int? group)
        {
            var userID = User.Identity.GetUserId();
            if (group.HasValue)
            {
                Membership membership = (from m in db.Memberships
                                  where m.UserId == userID
                                  && m.GroupId == @group.Value
                                  select m).FirstOrDefault();
                if (membership == null)
                {
                    return HttpNotFound();
                }
            }
            DateTime fromDate = DateTime.Now;
            fromDate = new DateTime(year ?? fromDate.Year, month ?? fromDate.Month, 1);
            DateTime monthDate = fromDate.Date;
            DateTime toDate = fromDate.AddMonths(1);
            fromDate = fromDate.AddDays(-(int)fromDate.DayOfWeek);
            toDate = toDate.AddDays(6 - (int)toDate.DayOfWeek);

            IQueryable<Event> queriedEvents;

            if(group.HasValue)
            {
                queriedEvents = from e in db.Events.Include(e => e.Group)
                                where e.GroupId == @group.Value
                                select e;
            }
            else
            {
                queriedEvents = (from e in db.Events.Include(e => e.Group)
                                 where e.OwnerId == userID
                                 select e)
                                .Union
                                (from e in db.Events.Include(e => e.Group)
                                 let gs = (from m in db.Memberships
                                           where m.UserId == userID
                                           select m.GroupId)
                                 where gs.Contains(e.GroupId.Value)
                                 select e);
            }


            List<Event> events = (from e in queriedEvents
                                 where (e.Starts >= fromDate && e.Starts < toDate)
                                || (e.Ends >= fromDate && e.Ends < toDate)
                                || (e.Starts < fromDate && e.Ends > toDate)
                                 select e).ToList();

            List < List < EventsIndexViewModel.EventsDay >> weeks = new List<List<EventsIndexViewModel.EventsDay>>();
            List<EventsIndexViewModel.EventsDay> week = null;

            for (DateTime currentDate = fromDate; currentDate <= toDate; currentDate = currentDate.AddDays(1))
            {
                if(currentDate.DayOfWeek == DayOfWeek.Sunday)
                    week = new List<EventsIndexViewModel.EventsDay>();

                DateTime nextDate = currentDate.AddDays(1);

                EventsIndexViewModel.EventsDay day = new EventsIndexViewModel.EventsDay
                {
                    Date = currentDate,
                    Events = (from e in events
                              where (e.Starts >= currentDate && e.Starts < nextDate)
                             || (e.Ends >= currentDate && e.Ends < nextDate)
                             || (e.Starts < currentDate && e.Ends > nextDate)
                              select e).ToList()
                };
                week.Add(day);

                if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                    weeks.Add(week);
            }

            var model = new EventsIndexViewModel
            {
                Month = monthDate,
                NextMonth = monthDate.AddMonths(1),
                PreviousMonth = monthDate.AddMonths(-1),
                Events = events,
                Weeks = weeks,
                GroupId = group
            };
            return PartialView(model);
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            ViewBag.groupId = @event.GroupId;

            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create(int? day, int? month, int? year, int? group)
        {
            var userID = User.Identity.GetUserId();
            DateTime fromDate = DateTime.Now;
            fromDate = new DateTime(year ?? fromDate.Year, month ?? fromDate.Month, day ?? fromDate.Day);

            if (group.HasValue)
            {
                Membership membership = (from m in db.Memberships
                                         where m.UserId == userID
                                         && m.GroupId == @group.Value
                                         select m).FirstOrDefault();
                if (membership == null)
                {
                    return HttpNotFound();
                }
            }

            Event @event = new Event
            {
                GroupId = group,
                Starts = fromDate,
                Ends = fromDate.AddDays(1),
                OwnerId = userID
            };

            ViewBag.groupId = group;

            return View(@event);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Starts,Ends,Name,Description,GroupId,OwnerId")] Event @event)
        {
            var userID = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                if (@event.GroupId.HasValue)
                {
                    Membership membership = (from m in db.Memberships
                                             where m.UserId == userID
                                             && m.GroupId == @event.GroupId.Value
                                             select m).FirstOrDefault();
                    if (membership == null)
                    {
                        return HttpNotFound();
                    }
                }
                @event.OwnerId = userID;
                db.Events.Add(@event);
                db.SaveChanges();
                if(!@event.GroupId.HasValue)
                    return RedirectToAction("Index", "MySchedule", new { month = @event.Starts.Month, year = @event.Starts.Year});
                else
                    return RedirectToAction("GroupEvents", "Groups", new { month = @event.Starts.Month, year = @event.Starts.Year, group = @event.GroupId});
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            var userID = User.Identity.GetUserId();

            if (@event.GroupId.HasValue)
            {
                Membership membership = (from m in db.Memberships.Include(m => m.GroupRole)
                                         where m.UserId == userID
                                         && m.GroupId == @event.GroupId.Value
                                         select m).FirstOrDefault();
                if (membership == null)
                {
                    return HttpNotFound();
                }

                if (@event.OwnerId != userID && !membership.GroupRole.CanManageUsersEvents)
                    return HttpNotFound();
            }
            else
            {
                if (@event.OwnerId != userID)
                    return HttpNotFound();
            }

            ViewBag.groupId = @event.GroupId;

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Starts,Ends,Name,Description,OwnerId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                Event editedEvent = db.Events.Find(@event.Id);
                if(editedEvent == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var userID = User.Identity.GetUserId();

                if (@event.GroupId.HasValue)
                {
                    Membership membership = (from m in db.Memberships.Include(m => m.GroupRole)
                                             where m.UserId == userID
                                             && m.GroupId == @event.GroupId.Value
                                             select m).FirstOrDefault();
                    if (membership == null)
                    {
                        return HttpNotFound();
                    }

                    if (@event.OwnerId != userID && !membership.GroupRole.CanManageUsersEvents)
                        return HttpNotFound();
                }
                else
                {
                    if (@event.OwnerId != userID)
                        return HttpNotFound();
                }

                editedEvent.Starts = @event.Starts;
                editedEvent.Ends = @event.Ends;
                editedEvent.Name = @event.Name;
                editedEvent.Description = @event.Description;

                db.SaveChanges();
                if (!editedEvent.GroupId.HasValue)
                    return RedirectToAction("Index", "MySchedule", new { month = editedEvent.Starts.Month, year = editedEvent.Starts.Year });
                else
                    return RedirectToAction("GroupEvents", "Groups", new { month = editedEvent.Starts.Month, year = editedEvent.Starts.Year, group = editedEvent.GroupId });
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            var userID = User.Identity.GetUserId();

            if (@event.GroupId.HasValue)
            {
                Membership membership = (from m in db.Memberships.Include(m => m.GroupRole)
                                         where m.UserId == userID
                                         && m.GroupId == @event.GroupId.Value
                                         select m).FirstOrDefault();
                if (membership == null)
                {
                    return HttpNotFound();
                }

                if (@event.OwnerId != userID && !membership.GroupRole.CanManageUsersEvents)
                    return HttpNotFound();
            }
            else
            {
                if (@event.OwnerId != userID)
                    return HttpNotFound();
            }

            ViewBag.groupId = @event.GroupId;

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);

            var userID = User.Identity.GetUserId();

            if (@event.GroupId.HasValue)
            {
                Membership membership = (from m in db.Memberships.Include(m => m.GroupRole)
                                         where m.UserId == userID
                                         && m.GroupId == @event.GroupId.Value
                                         select m).FirstOrDefault();
                if (membership == null)
                {
                    return HttpNotFound();
                }

                if (@event.OwnerId != userID && !membership.GroupRole.CanManageUsersEvents)
                    return HttpNotFound();
            }
            else
            {
                if (@event.OwnerId != userID)
                    return HttpNotFound();
            }

            db.Events.Remove(@event);
            db.SaveChanges();
            if (!@event.GroupId.HasValue)
                return RedirectToAction("Index", "MySchedule", new { month = @event.Starts.Month, year = @event.Starts.Year });
            else
                return RedirectToAction("GroupEvents", "Groups", new { month = @event.Starts.Month, year = @event.Starts.Year, group = @event.GroupId });
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
