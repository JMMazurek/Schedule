using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Schedule.Models
{
    public class Membership
    {
        public Membership()
        {

        }
        [Required]
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        [Required]
        public int GroupRoleId { get; set; }
        public virtual GroupRole GroupRole { get; set; }
    }
}