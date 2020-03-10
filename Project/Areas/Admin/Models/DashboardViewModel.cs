using Project.Areas.SecurityGuard.Models;
using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int StateId { get; set; }
        public Guid RoleId { get; set; }
        public List<SelectListItem> RolesList { get; set; }
        public List<Roles> storeRoles { get; set; }

        public List<Roles> storeUserRoles { get; set; }

        public List<Users> users { get; set; }

        public List<ContactInfo> contactInfoList { get; set; }
        public List<AddressBook> addressList { get; set; }
        public Guid UserId { get; set; }
        public Store store { get; set; }

        public int TototalRole { get; set; }
        public int TotalUser { get; set; }
        public int TotalContactinfo { get; set; }
        public int TotalAddress { get; set; }

        public UserForm userForm { get; set; }
        public UserAccountForm userAccount { get; set; }

        public ContactInfoForm contactform { get; set; }
        public CompanyAddressForm addressform { get; set; }

        public List<IntegerSelectListItem> AddressTypeList{get;set;}

        public List<IntegerSelectListItem> LgaList{get; set;}
        public List<IntegerSelectListItem> StateList { get; set; }
    }
}