﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Schedule.Models
{
    public class Request
    {
        public Request()
        {

        }

        public int Id { get; set; }
        [Required]
        public string RequestingUserId { get; set; }
        public virtual User RequestingUser { get; set; }
        public string AcceptingUserId { get; set; }
        public virtual User AcceptingUser { get; set; }
        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        public bool Accepted { get; set; }
        [Required]
        public bool Rejected { get; set; }
        [Required]
        public bool Canceled { get; set; }
        public string Status
        {
            get
            {
                if (Accepted)
                    return "Accepted";
                if (Rejected)
                    return "Rejected";
                if (Canceled)
                    return "Cancelled";
                return "Waiting";
            }
        }
    }
}