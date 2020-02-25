using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Models
{
    public class GrantResourceToRoleViewModel : PageInfoModel
    {
        public Resource Row { get; set; }
        public string Id { get; set; }
        public List<SelectListItem> AllRoles { get; set; }
        public List<SelectListItem> AvailableResources { get; set; }
        public List<SelectListItem> GrantedResources { get; set; }


        public List<Resource> AllGrantedResources { get; set; }
        public List<Roles> rolelist { get; set; }

        public List<Resource> AllResources { get; set; }
        [Required(ErrorMessage = "Please select a resource to grant")]
        public List<int> ResourceUsed { get; set; }
        public List<string> ResourceSelected { get; set; }
        public List<string> ResourceUsedTex { get; set; }

        public List<Resource> AllGranted { get; set; }
        [Required(ErrorMessage = "Please select a resource to revoke")]
        public List<int> GrantedUsed { get; set; }
        public List<string> GrantedSelected { get; set; }
        public List<string> GrantedUsedTex { get; set; }
    }
}