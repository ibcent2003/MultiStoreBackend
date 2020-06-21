using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Models
{
    public class ProductManagementViewModel
    {
        //Tables Mostly Index Page
        public List<ProductCategory> CategoryList { get; set; }
        public List<StoreProduct> ProductList { get; set; }

        public StoreProduct product { get; set; }

        public ProductCategory category { get; set; }

        //Dropdown list
        public List<IntegerSelectListItem> BrandList { get; set; }
        public List<IntegerSelectListItem> SubcategoryList { get; set; }
        
        public List<IntegerSelectListItem> ChildcategoryList { get; set; }


        public Store store { get; set; }
        public ProductForm Productform { get; set; }
        public string documentPath { get; set; }

        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }


        //Available colors
        public List<ProductColor> AllColor { get; set; }
       // [Required(ErrorMessage = "Please select a color to add")]
        public List<string> ColorUsed { get; set; }
        public List<string> ColorSelected { get; set; }
        public List<string> ColorUsedTex { get; set; }


        //granted colors
        public List<ProductColor> AllSelectColor { get; set; }
     //   [Required(ErrorMessage = "Please select a color to remove")]
        public List<string> GrantedColorUsed { get; set; }
        public List<string> GrantedColorSelected { get; set; }
        public List<string> GrantedColorUsedTex { get; set; }

        
        public List<IntegerSelectListItem> AllSizeType { get; set; }
        public int SizeTypeId { get; set; }
        public List<Size> AllGrantedSize { get; set; }

        //Available size
        public List<Size> AllSize { get; set; }
       // [Required(ErrorMessage = "Please select a size to add")]
        public List<string> SizeUsed { get; set; }
        public List<string> SizeSelected { get; set; }
        public List<string> sizeUsedTex { get; set; }


        //granted size
        public List<Size> AllSelectedSize { get; set; }
       // [Required(ErrorMessage = "Please select a size to remove")]
        public List<string> GrantedSizeUsed { get; set; }
        public List<string> GrantedSizeSelected { get; set; }
        public List<string> GrantedSizeUsedTex { get; set; }

    }
    public class ProductForm
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "Please enter The Product Name")]
        public string Name { get; set; }

        [Display(Name = "Product Description")]
        [Required(ErrorMessage = "Please enter The Product Description")]
        public string Description { get; set; }

        [Display(Name = "Acutal Price")]
        [Required(ErrorMessage = "Please enter Actual price")]
        public decimal AcutalPrice { get; set; }

        [Display(Name = "Discount Price")]
        [Required(ErrorMessage = "Please enter Discount price")]
        public decimal DiscountPrice { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Please enter Quantity")]
        public int Quantity { get; set; }

        public int NewQuantity { get; set; }

        [Display(Name = "Re-order level")]
        [Required(ErrorMessage = "Please enter Re-order Level")]
        public int ReorderLevel { get; set; }

        [Display(Name = "Photo 1")]
       // [Required(ErrorMessage = "Please Upload Photo 1")]
        public HttpPostedFileBase Photo1 { get; set; }

        [Display(Name = "Photo 2")]      
        public HttpPostedFileBase Photo2 { get; set; }

        [Display(Name = "Photo 3")]
        public HttpPostedFileBase Photo3 { get; set; }

        [Display(Name = "Brand")]
        [Required(ErrorMessage = "Please select a Brand")]
        public int BrandId { get; set; }

        [Display(Name = "Has Color")]
        public bool HasColor { get; set; }

        [Display(Name = "Has Size")]
        public bool HasSize { get; set; }

        [Display(Name = "Has Sales")]
        public bool HasSales { get; set; }

        [Display(Name = "Sub Category")]
        [Required(ErrorMessage = "Please select a Sub Category")]
        public int ProductSubCategoryId { get; set; }

        [Display(Name = "Child Category")]
        [Required(ErrorMessage = "Please select a Child Category")]
        public int ProductChildCategoryId { get; set; }

        public bool IsDeleted { get; set; }
        public int CategoryId { get; set; }
    }
}