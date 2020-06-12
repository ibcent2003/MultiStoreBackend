using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.Admin.Models
{
    public class ResourceViewModel : PageInfoModel
    {
        public IList<Resource> Rows { get; set; }
    }
}