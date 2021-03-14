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
        public List<StoreSlider> StoreSliderList { get; set; }
        public List<StoreImageCollection> ImageCollectionsList { get; set; }
        public StoreSliderForm StoreSliderform { get; set; }
        public StoreCollectionForm storeCollectionForm { get; set; }
    }


    public class StoreForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your Store name")]
        [Display(Name = "Store Name")]
        public string Name { get; set; }

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
}