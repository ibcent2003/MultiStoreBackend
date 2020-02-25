using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.SecurityGuard.Models
{
    public class GrantRoleViewModel
    {
        public string Id { get; set; }
        public List<SelectListItem> AllUsers { get; set; }

        public List<Roles> AllGrantedRole { get; set; }


        public List<Roles> AllRole { get; set; }
        [Required(ErrorMessage = "Please select a role to grant")]
        public List<string> RoleUsed { get; set; }
        public List<string> RoleSelected { get; set; }
        public List<string> RoleUsedTex { get; set; }



        public List<Roles> AllGranted { get; set; }
        [Required(ErrorMessage = "Please select a role to revoke")]
        public List<string> GrantedUsed { get; set; }
        public List<string> GrantedSelected { get; set; }
        public List<string> GrantedUsedTex { get; set; }


       
    }
}