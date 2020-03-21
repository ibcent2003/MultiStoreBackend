using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class ProductBrandViewModel
    {
        public List<ProductBrand> brandList { get; set; }
        public ProductBrandForm ProductBrandform { get; set; }
    }

    public class ProductBrandForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the Brand Name")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}