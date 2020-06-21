using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class StoreManagementViewModel
    {
        public string documentPath { get; set; }
        public string documentValue { get; set; }
        public List<Store> storelist { get; set; }
        public StoreForm storeform { get; set; }

        public Store store { get; set; }
    }


    public class StoreForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your Store name")]
        [Display(Name = "Store Name")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public HttpPostedFileBase Logo { get; set; }

        [Display(Name = "Store Currency")]
        public string StoreCurrency { get; set; }

        public bool OwnProcurement { get; set; }

        public bool IsDeleted { get; set; }

    }
}