using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.SecurityGuard.Models
{
    public class UserManagementViewModel
    {
        public UserForm userForm { get; set; }
    }

    public class UserForm
    {
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Is Locked Out")]
        public bool IsLocked { get; set; }

        public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}