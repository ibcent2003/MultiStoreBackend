using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class ProductColorViewModel
    {
        public List<ProductColor> ProductColorlist { get; set; }
        public ProductColorForm Productcolorform { get; set; }

        public ProductColor Productcolor { get; set; }
    }

    public class ProductColorForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your Store name")]
        [Display(Name = "Color Name")]
        public string Name { get; set; }      

        public bool IsDeleted { get; set; }

    }
}