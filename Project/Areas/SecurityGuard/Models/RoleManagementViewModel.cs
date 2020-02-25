using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.SecurityGuard.Models
{

    public class RoleManagementViewModel
    {
        public List<Roles> RoleList { get; set; }

        public RoleForm roleForm { get; set; }
    }

    public class RoleForm
    {
        [Required(ErrorMessage = "Role Name is required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public Guid roleId { get; set; }
    }
}