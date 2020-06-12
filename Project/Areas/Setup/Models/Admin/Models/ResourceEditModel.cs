using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Models
{
    public class ResourceEditModel
    {
        public Resource Row { get; set; }
        public List<SelectListItem> Resources { get; set; }
    }
}