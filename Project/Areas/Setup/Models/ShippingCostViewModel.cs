using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class ShippingCostViewModel
    {
        public ShippingCostForm shippingCostForm { get; set; }

        public List<ShippingCost> Rows { get; set; }
    }

    public class ShippingCostForm
    {
        public int Id { get; set; }

        [Display(Name = "Package Type")]
        [Required(ErrorMessage = "Please enter Package Type")]
        public string Name { get; set; }

        [Display(Name = "Price Per kg")]
        [Required(ErrorMessage = "Please enter Price per kg")]
        public decimal Fees { get; set; }

        public bool IsDeleted { get; set; }
    }
}