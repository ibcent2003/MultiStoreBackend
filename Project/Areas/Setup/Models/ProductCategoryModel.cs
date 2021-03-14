using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Models
{
    public class ProductCategoryModel
    {
        public string documentPath { get; set; }
        public string PhotoValue { get; set; }

        //default items
        public ProductCategory productcategory { get; set; }
        public ProductSubCategory productsubcategory { get; set; }

        //list items
        public List<ProductCategory> ProductCategorylist { get; set; }
        public List<ProductSubCategory> ProductSubCategorylist { get; set; }
        public List<ProductChildCategory> ProductChildCategoryList { get; set; }

        //form items
        public ProductCategoryForm ProductCategoryform { get; set; }
        public ProductSubCategoryForm ProductSubCategoryform { get; set; }
        public ProductChildCategoryForm ProductChildCategoryform { get; set; }


        public List<SelectListItem> Brandlist {get;  set; }
        public int BrandId { get; set; }
        public int ProductCategoryId { get; set; }
        public List<ProductBrand> prductbrand { get; set; }
    }


    public class ProductCategoryForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter the Product Category Name")]
        [Display(Name = "Product Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the Product Icon")]
        [Display(Name = "Category Icon")]
        public string Icon { get; set; }

        
        [Display(Name = "Sample Photo")]
        public HttpPostedFileBase SampleImage { get; set; }

        public bool IsDeleted { get; set; }

    }

    public class ProductSubCategoryForm
    {
        public int Id { get; set; }
        
        public int ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Enter the Product sub category")]
        [Display(Name = "Sub Category")]
        public string Name { get; set; }

        [Display(Name = "Sample Photo")]
        public HttpPostedFileBase SampleImage { get; set; }

        public bool IsDeleted { get; set; }

    }

    
}