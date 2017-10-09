using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Schedule.Models
{
    public class Invitation
    {
        public Invitation()
        {

        }

        public int Id { get; set; }
        [Required]
        public string InvitedUserId { get; set; }
        public virtual User InvitedUser { get; set; }
        [Required]
        public string InvitingUserId { get; set; }
        public virtual User InvitingUser { get; set; }
        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        [Required]
        public bool Accepted { get; set; }
        [Required]
        public bool Rejected { get; set; }
    }
}