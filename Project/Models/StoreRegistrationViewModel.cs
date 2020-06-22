using Project.Areas.SecurityGuard.Models;
using Project.Areas.Setup.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public List<IntegerSelectListItem> AddressTypeList { get; set; }

        public List<IntegerSelectListItem> LgaList { get; set; }
        public List<IntegerSelectListItem> StateList { get; set; }

        public List<ContactInfo> contactInfoList { get; set; }
        public List<AddressBook> addressList { get; set; }


        public StoreForm storeform { get; set; }
        public CompanyAddressForm addressform { get; set; }
    }
}