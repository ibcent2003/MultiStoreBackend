using Project.Areas.SecurityGuard.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public Guid RoleId { get; set; }
        public List<SelectListItem> RolesList { get; set; }
        public List<Roles> storeRoles { get; set; }

        public List<Roles> storeUserRoles { get; set; }

        public List<Users> users { get; set; }
        public Guid UserId { get; set; }
        public Store store { get; set; }

        public int TototalRole { get; set; }
        public int TotalUser { get; set; }
        public int TotalContactinfo { get; set; }
        public int TotalAddress { get; set; }

        public UserForm userForm { get; set; }
        public UserAccountForm userAccount { get; set; }
    }
}