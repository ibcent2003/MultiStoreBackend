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
    public class StoreManagementViewModel
    {
        public string documentPath { get; set; }
        public string documentValue { get; set; }
        public List<Store> storelist { get; set; }
        public StoreForm storeform { get; set; }

        public StoreImageCollection ImageCollection { get; set; }
        public List<Testimonial> TestimonialList { get; set; }
        public List<IntegerSelectListItem> ThemeList { get; set; }

        public TestimonialForm Testimonialform { get; set; }

        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> ThemesList { get; set; }
        public List<SelectListItem> BankList { get; set; }

        public Store store { get; set; }
        public List<StoreSlider> StoreSliderList { get; set; }
        public List<StoreImageCollection> ImageCollectionsList { get; set; }
        public StoreSliderForm StoreSliderform { get; set; }
        public StoreCollectionForm storeCollectionForm { get; set; }
        public List<IntegerSelectListItem> MenuTypeList { get; set; }
        public List<Menu> MenuList { get; set; }
        public MenuForm Menuform { get; set; }

        public List<SelectListItem> PaymentMethodlist { get; set; }
        public int PaymentMethodId { get; set; }
        public List<PaymentMethod> StorePaymentMethod { get; set; }

    }

    public class StoreForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your Store name")]
        [Display(Name = "Store Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter preferable Domain Name(First Choice)")]
        [Display(Name = "Preferable Domain (First Choice)")]
        public string URL { get; set; }

        [Required(ErrorMessage = "Enter preferable Domain Name(Second Choice)")]
        [Display(Name = "Preferable Domain (Second Choice)")]
        public string URL2 { get; set; }

        [Required(ErrorMessage = "Enter preferable Domain Name(Thrid Choice)")]
        [Display(Name = "Preferable Domain(Thrid Choice)")]
        public string URL3 { get; set; }

        [Required(ErrorMessage = "Please enter your Store Profile")]
        [Display(Name = "Store Profile")]
        public string Description { get; set; }

        [Display(Name = "Logo")]
        public HttpPostedFileBase Logo { get; set; }

        [Display(Name = "Store Currency")]
        public string StoreCurrency { get; set; }

        public bool OwnProcurement { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Please select a currency")]
        [Display(Name = "Accepted Currency")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "Please select a theme for your store")]
        [Display(Name = "Store Theme")]
        public int ThemesId { get; set; }

        [Required(ErrorMessage = "Please select you bank ")]
        [Display(Name = "Store Theme")]
        public int BankId { get; set; }

        [Required(ErrorMessage = "Please enter your Account Name")]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Please enter your Account Number")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Account number should have minimum 11 digits")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Account number should not contain characters")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

    }

    public class StoreSliderForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your Caption One")]
        [Display(Name = "Caption One")]
        public string CaptionOne { get; set; }

        [Required(ErrorMessage = "Please enter your Caption Two")]
        [Display(Name = "Caption Two")]
        public string CaptionTwo { get; set; }

        [Required(ErrorMessage = "Please enter your Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Slider Photo")]
        public HttpPostedFileBase SliderPhoto { get; set; }

        [Required(ErrorMessage = "Please enter your Button Text")]
        [Display(Name = "Button Text")]
        public string ButtonText { get; set; }     

        public bool IsDeleted { get; set; }
        public int StoreId { get; set; }




    }

    public class StoreCollectionForm
    {
        public int Id { get; set; }

        public string CollectionName { get; set; }

        [Required(ErrorMessage = "Please the photo collection")]
        [Display(Name = "Collection Photo")]
        public HttpPostedFileBase CollectionPhoto { get; set; }     

        public bool IsDeleted { get; set; }
        public int StoreId { get; set; }




    }

    public class TestimonialForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter the Full Name")]
        [Display(Name = "Full Name")]
        public string ClientName { get; set; }
        [Required(ErrorMessage = "Please enter the name")]
        [Display(Name = "Message")]
        public string Message { get; set; }
        public bool IsDeleted { get; set; }
        public int StoreId { get; set; }




    }

    public class MenuForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter the content")]
        [Display(Name = "Menu")]
        public string Content { get; set; }       
        public bool IsDeleted { get; set; }
        public int StoreId { get; set; }

        [Required(ErrorMessage = "Please select a menu")]
        [Display(Name = "Menu")]
        public int MenuTypeId { get; set; }




    }
}