using Project.Areas.SecurityGuard.Models;
using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Models
{
    public class DashboardViewModel
    {


        #region Approval

        public IList<Store> StoreApproval { get; set; }
        public ApprovalForm approvalForm { get; set; }
        public List<SelectListItem> WorkflowStepActions { get; set; }
        public List<SelectListItem> OtherWorkflowSteps { get; set; }
        public List<StoreAction> ActionLogs { get; set; }

        #endregion

        public List<StoreProduct> storeProducts { get; set; }
        public int TotalNewRegistration { get; set; }
        public int TotalApproved { get; set;}
        public int TotalRejected { get; set; }
        public int TotalPending { get; set; }
        public int OwnedBy { get; set; }

       // public int TotalNewOrder { get; set; }
        public int TotalConfirmOrder { get; set; }
        public int TotalConfirmPayment { get; set; }
        public int TotalCancelledOrder { get; set; }
        public int TotalCancelledPayment { get; set; }
        public string orderType { get; set; }
        public int OverallOrder { get; set; }
        public List<Guid> TotalCustomers { get; set; }
        public List<Guid> storeuser { get; set; }

        public int StateId { get; set; }
        public Guid RoleId { get; set; }
        public List<SelectListItem> RolesList { get; set; }
        public List<Roles> storeRoles { get; set; }
        public List<ProductOrder> OrderList { get; set; }
        public List<ProductOrder> RecentOrder { get; set; }
        public ProductOrder Order { get; set; }
        public List<CartItem> cartItemList { get; set; }
        public ProductOrder productOrder { get; set; }
        public List<UserDetail> userDetails { get; set; }
        public List<Roles> storeUserRoles { get; set; }

        public string documentPath { get; set; }
        public List<Users> users { get; set; }

        public List<ContactInfo> contactInfoList { get; set; }
        public List<AddressBook> addressList { get; set; }
        public Guid UserId { get; set; }
        public Store store { get; set; }
        public StoreAction storeAction { get; set; }
        public int TotalNewOrder { get; set; }
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


        //list items
        public List<SelectListItem> ChildCategorylist { get; set; }
        public int ProductChildCategoryId { get; set; }
        public List<ProductChildCategory> StoreProductChildCategory { get; set; }

        public ProductSubCategory storesubCate { get; set; }
        public bool HasAllChildCategory { get; set; }

        public CustomerAddressBook DeliveryAddress { get; set; }

    }
    public class ApprovalForm
    {
        
        public Guid Id { get; set; }


        [Required(ErrorMessage = "Select an action to perform on the application")]
        [Display(Name = "Action")]
        public int ActionId { get; set; }

        [Display(Name = "Select Step")]
        public int? NextStep { get; set; }

        [Required(ErrorMessage = "Please enter reason for Accepting/Rejecting")]
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        

        //newly added property
        public string OwnedBy { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
     
    }
}