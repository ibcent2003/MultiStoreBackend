using Project.Areas.SecurityGuard.Models;
using Project.Areas.Setup.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Models
{
    public class StoreRegistrationViewModel
    {
        public Workflow workflow { get; set; }
        public string documentPath { get; set; }
        public int countryId { get; set; }
        public string currency { get; set; }
        public string logos { get; set; }
       
        public Store store { get; set; }
        public UserDetail userDetail { get; set; }
        public Memberships LoginDetails { get; set; }
        public TempUser tempUser { get; set; }

       

        public List<IntegerSelectListItem> AddressTypeList { get; set; }

        public List<IntegerSelectListItem> LgaList { get; set; }
        public List<IntegerSelectListItem> StateList { get; set; }
        public int StateId { get; set; }

        public List<ContactInfo> contactInfoList { get; set; }
        public List<AddressBook> addressList { get; set; }

        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> ThemesList { get; set; }


        //list items
        public List<SelectListItem> Categorylist { get; set; }
        public int ProductCategoryId { get; set; }
        public List<ProductCategory> StoreProductCategory { get; set; }


        //list items
        public List<SelectListItem> SubCategorylist { get; set; }
        public int ProductSubCategoryId { get; set; }
        public List<ProductSubCategory> StoreProductSubCategory { get; set; }

        public ProductCategory storeCate { get; set; }
        public bool HasAllSubCategory { get; set; }


        public StoreForm storeform { get; set; }
        public CompanyAddressForm addressform { get; set; }
        public ContactInfoForm contactform { get; set; }
        public TempUserForm tempUserform { get; set; }


        //list items
        public List<SelectListItem> ChildCategorylist { get; set; }
        public int ProductChildCategoryId { get; set; }
        public List<ProductChildCategory> StoreProductChildCategory { get; set; }

        public ProductSubCategory storesubCate { get; set; }
        public bool HasAllChildCategory { get; set; }

        public bool TempUseradded { get; set; }
    }
}