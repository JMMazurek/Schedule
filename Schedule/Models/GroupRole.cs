using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Schedule.Models
{
    public class GroupRole
    {
        public GroupRole()
        {
            Memberships = new HashSet<Membership>();
        }

        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public virtual ICollection<Membership> Memberships { get; private set; }
        /* ToDo: Add permissions */
    }
}