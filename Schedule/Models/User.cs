using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Schedule.Models
{
    // You can add profile data for the user by adding more properties to your User class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public User()
        {
            Events = new HashSet<Event>();
            EventTypes = new HashSet<EventType>();
            Requests = new HashSet<Request>();
            Invitations = new HashSet<Invitation>();
            Memberships = new HashSet<Membership>();
        }

        public virtual ICollection<Event> Events { get; private set; }
        public virtual ICollection<EventType> EventTypes { get; private set; }
        public virtual ICollection<Request> Requests { get; private set; }
        public virtual ICollection<Invitation> Invitations { get; private set; }
        public virtual ICollection<Membership> Memberships { get; private set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}