using Project.Areas.Setup.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class StoreRegistrationViewModel
    {
        public Workflow workflow { get; set; }
        public StoreForm storeform { get; set; }
    }
}