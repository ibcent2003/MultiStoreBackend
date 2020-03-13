using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class ProductChildModel
    {
        public ProductChildCategoryForm ProductChildCategoryform { get; set; }
    }

    public class ProductChildCategoryForm
    {
        public int Id { get; set; }
        public int ProductSubCategoryId { get; set; }

        [Required(ErrorMessage = "Enter the Product child category")]
        [Display(Name = "Child Category")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

    }
}