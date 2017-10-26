using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Schedule.Models
{
    public class Group
    {
        public Group()
        {
            Memberships = new HashSet<Membership>();
            Invitations = new HashSet<Invitation>();
            Requests = new HashSet<Request>();
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        [Required]
        public bool Searchable { get; set; }
        [Required]
        public bool NeedConfirmation { get; set; }
        [Required]
        public bool Public { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Group name")]
        public string Name { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        public virtual ICollection<Membership> Memberships { get; private set; }
        public virtual ICollection<Invitation> Invitations { get; private set; }
        public virtual ICollection<Request> Requests { get; private set; }
        public virtual ICollection<Event> Events { get; private set; }
    }
}