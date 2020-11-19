using Project.Areas.Admin.Models;
using Project.Areas.SecurityGuard.Models;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Roles = System.Web.Security.Roles;

namespace Project.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator, Store Admin")]
    public class DashboardController : Controller
    {
        Backbone services = new Backbone();
        private PROEntities db = new PROEntities();
        private IMembershipService membershipService;
        // GET: /Admin/Dashboard/


        public DashboardController()
        {
            
            this.membershipService = new MembershipService(Membership.Provider);
           
        }

        private Store storeDetails()
        {
            try
            {
                var username = Membership.GetUser().UserName;
                var user = db.Users.Where(x => x.UserName == username).FirstOrDefault();

                return user.Store.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public ActionResult Index()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                string username = Membership.GetUser().UserName;
                string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);
                if (Roles.GetRolesForUser(User.Identity.Name).Contains("Administrator"))
                 {
                    //count total store
                   
                    var getstore = db.Store.Where(x => x.Status == "Registration Verification" && x.OwnedBy  == "Administrator").ToList();
                    model.TotalNewRegistration = getstore.Count();

                    var getApproved = db.Store.Where(x => x.Status == "Approved").ToList().Count();
                    model.TotalApproved = getApproved;

                    var getRejected = db.Store.Where(x => x.Status == "Registration Rejected").ToList().Count();
                    model.TotalRejected = getRejected;

                    var getOwnedBy = db.Store.Where(x => x.OwnedBy == User.Identity.Name).ToList().Count();
                    model.OwnedBy = getOwnedBy;

                    return View(model);
                }
                else if (Roles.GetRolesForUser(User.Identity.Name).Contains("Store Admin") || Roles.GetRolesForUser(User.Identity.Name).Contains("Product Manager"))
                {
                    var store = storeDetails();
                    if(store.Status== "Registration Verification" || store.Status== "Registration Rejected")
                    {
                        return RedirectToAction("PendingApproval", "Dashboard", new { area = "Admin", Id = store.ProcessInstaceId });
                    }
                    else if(store.Status=="Approved")
                    {
                        return RedirectToAction("StoreDashboard", "Dashboard", new { area = "Admin", Id = store.ProcessInstaceId });
                    }
                   
                }                             
                return View(model); 
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }

        public ActionResult PendingApproval(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();


                var storeDetail = storeDetails();
                if (storeDetail == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                    TempData["message"] = "Unauthorised Access";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                if (storeDetail.Status == "Registration Rejected")
                {
                    var GetActionLog = db.StoreAction.Where(x => x.StoreId == storeDetail.Id && x.Name == "Registration Rejected").OrderByDescending(x => x.ModifiedDate).FirstOrDefault();                   
                    model.storeAction = GetActionLog;
                    TempData["Reason"] = GetActionLog.Reason;
                }
                model.documentPath = Properties.Settings.Default.DocumentPath;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        public ActionResult Details(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();


                var storeDetail = storeDetails();
                if (storeDetail == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                    TempData["message"] = "Unauthorised Access";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.documentPath = Properties.Settings.Default.DocumentPath;
                model.addressList = Backbone.GetStorAddress(db, Id);
                model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        public ActionResult StoreDashboard(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();


                var storeDetail = storeDetails();
                if (!System.Web.Security.Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
               
                model.store = Backbone.GetStore(db, Id);
                if(model.store==null)
                {
                  
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.TototalRole = model.store.Roles.Count();
                model.TotalUser = model.store.Users.Count();
                model.TotalContactinfo = model.store.ContactInfo.Count();
                model.TotalAddress = model.store.AddressBook.Count();
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        public ActionResult StoreCategory(Guid Id)
        {
            try
            {

                //var storeDetail = storeDetails();
                //if (storeDetail == null || !Roles.GetRolesForUser(User.Identity.Name).Contains("ADMINISTRATOR"))
                //{
                //    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                //    TempData["message"] = "Unauthorised Access";
                //    return RedirectToAction("Index", "Store", new { area = "Setup" });
                //}

                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }                               
                List<int> list = (from x in model.store.ProductCategory  select x.Id).ToList<int>();
                model.Categorylist = db.ProductCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
               
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }

        }

        [HttpPost]
        public ActionResult StoreCategory(DashboardViewModel model)
        {
            try
            {

                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }


                var getcategory = db.ProductCategory.Where(x => x.Id == model.ProductCategoryId).FirstOrDefault();
                if (getcategory == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                List<int> list = (from x in model.store.ProductCategory select x.Id).ToList<int>();
                model.Categorylist = db.ProductCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

                model.StoreProductCategory = model.store.ProductCategory.ToList();
               
                if (model.ProductCategoryId == 0)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "Please select a Category from the dropdownlist and click on the Add button";
                    return RedirectToAction("StoreCategory", "Dashboard", new { Id = model.store.ProcessInstaceId, area = "Admin" });
                }
               
                model.store.ProductCategory.Add(getcategory);
                db.SaveChanges();
                base.TempData["message"] = "The Category " + getcategory.Name.ToUpper() + " has been added Successfully";
                return RedirectToAction("StoreCategory", "Dashboard", new { Id = model.store.ProcessInstaceId, area = "Admin" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult RemoveStoreCategory(Guid Id, int CategoryId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();

                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var getcategory = db.ProductCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                if (getcategory == null)
                {
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreCategory", "Dashboard", new { Id = Id, area = "Admin" });
                }
            
                //check if product has been uploaded for this category
                var validateProduct = model.store.StoreProduct.Where(x => x.ProductCategoryId == CategoryId).ToList();
                if (validateProduct.Any())
                {
                    TempData["message"] = "You can not remove this category from your store because there is an existing product attacted to the category "+getcategory.Name.ToUpper()+". Kindly delete the product before deleting the category or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreCategory", "Dashboard", new { Id = Id, area = "Admin" });
                }
             
                model.store.ProductCategory.Remove(getcategory);
                db.SaveChanges();
                base.TempData["message"] = "The " + getcategory.Name + " has been deleted successfully.";
                return RedirectToAction("StoreCategory", "Dashboard", new { Id = Id, area = "Admin" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult StoreSubCategory(Guid Id, int CategoryId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var GetCate = db.ProductCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                if (GetCate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
               

                model.storeCate = GetCate;
                List<int> list = (from x in model.store.ProductSubCategory select x.Id).ToList<int>();
                model.SubCategorylist = db.ProductSubCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductCategoryId==CategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

                model.StoreProductSubCategory = model.store.ProductSubCategory.Where(x=>x.ProductCategoryId==CategoryId).ToList();
                if (model.SubCategorylist.Count() == 0)
                {
                    model.HasAllSubCategory = false;
                }
                else
                {
                    model.HasAllSubCategory = true;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult StoreSubCategory(DashboardViewModel model)
        {
            try
            {
                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var getSubcategory = db.ProductSubCategory.Where(x => x.Id == model.ProductSubCategoryId).FirstOrDefault();
                if (getSubcategory == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                 List<int> list = (from x in model.store.ProductSubCategory select x.Id).ToList<int>();
                 model.SubCategorylist = db.ProductSubCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductCategoryId == getSubcategory.ProductCategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                 if (model.SubCategorylist == null)
                 {
                     model.HasAllSubCategory = true;
                 }
                 else
                 {
                     model.HasAllSubCategory = false;
                 }


                model.StoreProductSubCategory = model.store.ProductSubCategory.Where(x => x.ProductCategoryId == getSubcategory.ProductCategoryId).ToList();
                model.store.ProductSubCategory.Add(getSubcategory);
                db.SaveChanges();
                base.TempData["message"] = "The Category " + getSubcategory.Name.ToUpper() + " has been added Successfully";
                return RedirectToAction("StoreSubCategory", "Dashboard", new { Id = model.store.ProcessInstaceId, CategoryId = getSubcategory.ProductCategoryId, area = "Admin" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult RemoveStoreSubCategory(Guid Id, int SubCategoryId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var getSubcategory = db.ProductSubCategory.Where(x => x.Id == SubCategoryId).FirstOrDefault();
                if (getSubcategory == null)
                {
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                //check if product has been uploaded for this subcategory
                var validateProduct = model.store.StoreProduct.Where(x => x.ProductSubCategoryId == SubCategoryId).ToList();
                if (validateProduct.Any())
                {
                    TempData["message"] = "You can not remove this category from your store because there is an existing product attacted to the category " + getSubcategory.Name.ToUpper() + ". Kindly delete the product before deleting the category or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreSubCategory", "Dashboard", new { Id = Id,CategoryId=getSubcategory.ProductCategoryId, area = "Admin" });
                }
                model.store.ProductSubCategory.Remove(getSubcategory);
                db.SaveChanges();
                base.TempData["message"] = "The " + getSubcategory.Name + " has been deleted successfully.";
                return RedirectToAction("StoreSubCategory", "Dashboard", new { Id = Id, CategoryId = getSubcategory.ProductCategoryId, area = "Admin" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult StoreChildCategory(Guid Id, int SubCategoryId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var GetSubCate = db.ProductSubCategory.Where(x => x.Id == SubCategoryId).FirstOrDefault();
                if (GetSubCate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.storesubCate = GetSubCate;
                List<int> list = (from x in model.store.ProductChildCategory select x.Id).ToList<int>();
                model.ChildCategorylist = db.ProductChildCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductSubCategoryId==SubCategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.StoreProductChildCategory = model.store.ProductChildCategory.Where(x => x.ProductSubCategoryId == SubCategoryId).ToList();
                if (model.ChildCategorylist.Count() == 0)
                {
                    model.HasAllChildCategory = false;
                }
                else
                {
                    model.HasAllChildCategory = true;
                }
                var GetCate = db.ProductCategory.Where(x => x.Id == GetSubCate.ProductCategoryId).FirstOrDefault();
                model.storeCate = GetCate;
                model.storesubCate = GetSubCate;

                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            }
        }

        [HttpPost]
        public ActionResult StoreChildCategory(DashboardViewModel model)
        {
            try
            {
                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var getChildbcategory = db.ProductChildCategory.Where(x => x.Id == model.ProductChildCategoryId).FirstOrDefault();
                if (getChildbcategory == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                List<int> list = (from x in model.store.ProductChildCategory select x.Id).ToList<int>();

                model.ChildCategorylist = db.ProductChildCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductSubCategoryId == getChildbcategory.ProductSubCategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                if (model.ChildCategorylist == null)
                {
                    model.HasAllSubCategory = true;
                }
                else
                {
                    model.HasAllSubCategory = false;
                }


                model.StoreProductChildCategory = model.store.ProductChildCategory.Where(x => x.ProductSubCategoryId == getChildbcategory.ProductSubCategoryId).ToList();
                model.store.ProductChildCategory.Add(getChildbcategory);
                db.SaveChanges();
                base.TempData["message"] = "The Category " + getChildbcategory.Name.ToUpper() + " has been added Successfully";
                return RedirectToAction("StoreChildCategory", "Dashboard", new { Id = model.store.ProcessInstaceId, SubCategoryId = getChildbcategory.ProductSubCategoryId, area = "Admin" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult RemoveStoreChildCategory(Guid Id, int ChildCategoryId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var getSubcategory = db.ProductChildCategory.Where(x => x.Id == ChildCategoryId).FirstOrDefault();
                if (getSubcategory == null)
                {
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                //check if product has been uploaded for this subcategory
                var validateProduct = model.store.StoreProduct.Where(x => x.ProductChildCategoryId == ChildCategoryId).ToList();
                if (validateProduct.Any())
                {
                    TempData["message"] = "You can not remove this Child category from your store because there is an existing product attacted to " + getSubcategory.Name.ToUpper() + ". Kindly delete the product before deleting the child category or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreChildCategory", "Dashboard", new { Id = Id, SubCategoryId = getSubcategory.ProductSubCategoryId, area = "Admin" });
                }
                model.store.ProductChildCategory.Remove(getSubcategory);
                db.SaveChanges();
                base.TempData["message"] = "The " + getSubcategory.Name + " has been deleted successfully.";
                return RedirectToAction("StoreChildCategory", "Dashboard", new { Id = Id, SubCategoryId = getSubcategory.ProductSubCategoryId, area = "Admin" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        #region manage store roles
        public ActionResult StoreRoles(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                List<Guid> list = (from x in model.store.Roles select x.RoleId).ToList<Guid>();

                model.storeRoles = model.store.Roles.OrderBy(x => x.RoleName).ToList();
                model.RolesList = (from a in db.Roles where !list.Contains(a.RoleId) orderby a.RoleName select a).AsEnumerable().Select(x => new SelectListItem() { Value = x.RoleId.ToString(), Text = x.RoleName }).ToList();
               
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        [HttpPost]
        public ActionResult StoreRoles(DashboardViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == model.store.ProcessInstaceId).FirstOrDefault();
                var GetRole = db.Roles.Where(x => x.RoleId == model.RoleId).FirstOrDefault();
                GetStore.Roles.Add(GetRole);
                db.SaveChanges();
                List<Guid> list = (from x in model.store.Roles select x.RoleId).ToList<Guid>();

                model.storeRoles = model.store.Roles.OrderBy(x => x.RoleName).ToList();
                model.RolesList = (from a in db.Roles where !list.Contains(a.RoleId) orderby a.RoleName select a).AsEnumerable().Select(x => new SelectListItem() { Value = x.RoleId.ToString(), Text = x.RoleName }).ToList();

                TempData["message"] = "The role has been attached Successfully.";
                return RedirectToAction("StoreRoles", "Dashboard", new {Id=GetStore.ProcessInstaceId, area = "Admin" });
               
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }
     
        public ActionResult RemoveStoreRole(int Id, Guid RoleId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var GetStore = db.Store.Where(x => x.Id == Id).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var GetRole = db.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault(); 
                if(GetRole ==null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                GetStore.Roles.Remove(GetRole);
                db.SaveChanges();
                TempData["message"] = "The role has been removed Successfully.";
                return RedirectToAction("StoreRoles", "Dashboard", new { Id = GetStore.ProcessInstaceId, area = "Admin" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }
        #endregion

        #region manage store user

        public ActionResult StoreUserList(Guid Id)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }

                DashboardViewModel model = new DashboardViewModel();
                var GetStore = Backbone.GetStore(db, storeDetail.ProcessInstaceId);
                model.store = GetStore;
                model.users = GetStore.Users.ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        public ActionResult CreateStoreuser(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var GetStore = Backbone.GetStore(db, Id);
                model.store = GetStore;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        [HttpPost]
         
        public ActionResult CreateStoreuser(DashboardViewModel model)
        {
            try
            {
                var GetStore = Backbone.GetStore(db, model.store.ProcessInstaceId);
                model.store = GetStore;
                MembershipCreateStatus membershipCreateStatu;
                if (base.ModelState.IsValid)
                {                   
                    if (model.userAccount.Password != model.userAccount.ConfirmPassword)
                    {
                        base.TempData["MessageType"] = "danger";
                        base.TempData["Message"] = "The Password and Confirm Password does not match. Please must sure both Password are the same.";
                        return base.View(model);
                    }
                    if ((
                        from x in this.db.Users
                        where x.UserName == model.userAccount.EmailAddress
                        select x).ToList<Users>().Any<Users>())
                    {
                        base.TempData["MessageType"] = "danger";
                        base.TempData["Message"] = string.Concat("The Email address ", model.userAccount.EmailAddress, " has been used by another user. Please enter different Email Address.");
                        return base.View(model);
                    }

                    if ((
                        from x in this.db.Users
                        where x.UserName == model.userAccount.Username
                        select x).ToList<Users>().Any<Users>())
                    {
                        base.TempData["MessageType"] = "danger";
                        base.TempData["Message"] = string.Concat("The Username ", model.userAccount.Username, " has been used by another user. Please enter different username.");
                        return base.View(model);
                    }
                    this.membershipService.CreateUser(model.userAccount.Username, model.userAccount.Password, model.userAccount.EmailAddress, null, null, true, out membershipCreateStatu);
                    if (membershipCreateStatu == MembershipCreateStatus.Success)
                    {

                        var store = db.Store.Where(x => x.ProcessInstaceId == GetStore.ProcessInstaceId).FirstOrDefault();

                        Users user = (
                            from x in this.db.Users
                            where x.UserName == model.userAccount.Username
                            select x).FirstOrDefault<Users>();
                        UserDetail userDetail = new UserDetail()
                        {
                            FirstName = model.userAccount.FirstName,
                            LastName = model.userAccount.LastName,
                            EmailAddres = model.userAccount.EmailAddress,
                            MobileNumber = model.userAccount.MobileNumber,
                            UserId = user.UserId,
                            ModifiedBy = base.User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            
                        };
                        this.db.UserDetail.AddObject(userDetail);
                        store.Users.Add(user);
                        this.db.SaveChanges();
                        Alert alert = (
                            from x in this.db.Alert
                            where x.Id == Properties.Settings.Default.NewAccount
                            select x).FirstOrDefault<Alert>();
                        Backbone.SendEmailNotificationToUser(db,alert.SubjectEmail, alert.Email.Replace("%Email%", model.userAccount.EmailAddress).Replace("%uname%", model.userAccount.Username).Replace("%pwd%", model.userAccount.Password).Replace("%First_Name%",model.userAccount.FirstName), model.userAccount.EmailAddress, Settings.Default.EmailReplyTo, alert.Id);
                        Backbone.SendSMSNotificationToUser(db,alert.SubjectSms, alert.Sms.Replace("%First_Name%", model.userAccount.FirstName), model.userAccount.MobileNumber, "FORTRESS", alert.Id);
                        base.TempData["message"] = string.Concat(new string[] { "<b>", model.userAccount.FirstName, "</b> <b>", model.userAccount.LastName, "</b> has been added Successfully. Please assign role to user" });
                        return base.RedirectToAction("GrantStoreUserRole", new {Id=model.store.ProcessInstaceId, UserId = user.UserId });
                    }
                    base.ModelState.AddModelError("", DashboardController.ErrorCodeToString(membershipCreateStatu));
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }

        }

        public ActionResult EditStoreUser(Guid Id, Guid userId)
        {
            try
            {
                var GetStore = Backbone.GetStore(db, Id);
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var getUser = Backbone.StoreUser(db, userId);
                if (getUser == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var GetUserDetail = getUser.UserDetail.FirstOrDefault();               
                DashboardViewModel model = new DashboardViewModel();
                model.userForm = new SecurityGuard.Models.UserForm();
                if (GetUserDetail != null)
                {
                    model.userForm.FirstName = GetUserDetail.FirstName;
                    model.userForm.LastName = GetUserDetail.LastName;
                    model.userForm.MobileNumber = GetUserDetail.MobileNumber;
                    model.userForm.EmailAddress = GetUserDetail.EmailAddres;
                }
                var GetMembership = getUser.Memberships;               
                model.userForm.Username = getUser.UserName;
                model.userForm.EmailAddress = GetMembership.Email;             
                model.userForm.IsApproved = GetMembership.IsApproved;
                model.userForm.IsLocked = GetMembership.IsLockedOut;
                model.userForm.UserId = getUser.UserId;
                model.UserId = getUser.UserId;
                model.store = GetStore;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }

        }


        [HttpPost]
        public ActionResult EditStoreUser(DashboardViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetUser = db.Users.Where(x => x.UserId == model.userForm.UserId).FirstOrDefault();
                    var GetMembership = GetUser.Memberships;
                    var GetUserDetail = GetUser.UserDetail.FirstOrDefault();
                    if (GetUserDetail != null)
                    {

                        //update user details

                        GetUserDetail.FirstName = model.userForm.FirstName;
                        GetUserDetail.LastName = model.userForm.LastName;
                        GetUserDetail.EmailAddres = model.userForm.EmailAddress;
                        GetUserDetail.MobileNumber = model.userForm.MobileNumber;
                        GetUserDetail.ModifiedBy = User.Identity.Name;
                        GetUserDetail.ModifiedDate = DateTime.Now;
                                            
                    }
                    else
                    {
                        // insert user details
                        UserDetail addnew = new UserDetail
                        {
                            FirstName = model.userForm.FirstName,
                            LastName = model.userForm.LastName,
                            EmailAddres = model.userForm.EmailAddress,
                            MobileNumber = model.userForm.MobileNumber,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            UserId = GetUser.UserId
                        };
                        db.UserDetail.AddObject(addnew);
                        db.SaveChanges();
                    }

                    GetMembership.IsLockedOut = model.userForm.IsLocked;
                    GetMembership.IsApproved = model.userForm.IsApproved;
                    GetMembership.Email = model.userForm.EmailAddress;
                    
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.userForm.FirstName + " " + model.userForm.LastName + "</b> was Successfully updated";
                    return base.RedirectToAction("StoreUserList", new { Id = model.store.ProcessInstaceId });

                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index");
            }
        }


        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.InvalidUserName:
                    {
                        return "The user name provided is invalid. Please check the value and try again.";
                    }
                case MembershipCreateStatus.InvalidPassword:
                    {
                        return "The password provided is invalid. Minimum of six characters in length is required.";
                    }
                case MembershipCreateStatus.InvalidQuestion:
                    {
                        return "The password retrieval question provided is invalid. Please check the value and try again.";
                    }
                case MembershipCreateStatus.InvalidAnswer:
                    {
                        return "The password retrieval answer provided is invalid. Please check the value and try again.";
                    }
                case MembershipCreateStatus.InvalidEmail:
                    {
                        return "The e-mail address provided is invalid. Please check the value and try again.";
                    }
                case MembershipCreateStatus.DuplicateUserName:
                    {
                        return "User name already exists. Please enter a different user name.";
                    }
                case MembershipCreateStatus.DuplicateEmail:
                    {
                        return "A user name for that e-mail address already exists. Please enter a different e-mail address.";
                    }
                case MembershipCreateStatus.UserRejected:
                    {
                        return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                    }
                case MembershipCreateStatus.InvalidProviderUserKey:
                case MembershipCreateStatus.DuplicateProviderUserKey:
                    {
                        return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                    }
                case MembershipCreateStatus.ProviderError:
                    {
                        return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                    }
                default:
                    {
                        return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                    }
            }
        }
        #endregion

        #region Grant Store User Role
        public ActionResult GrantStoreUserRole(Guid Id, Guid UserId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.storeUserRoles = Backbone.StoreUser(db, UserId).Roles.ToList();

                var role = Backbone.StoreUser(db, UserId).Roles.Select(x=>x.RoleId).ToList();
                model.RolesList = (from a in model.store.Roles where !role.Contains(a.RoleId) orderby a.RoleName select a).AsEnumerable().Select(x => new SelectListItem() { Value = x.RoleId.ToString(), Text = x.RoleName }).ToList();
                model.UserId = UserId;
               
             

                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        [HttpPost]
        public ActionResult GrantStoreUserRole(DashboardViewModel model)
        {
            try
            {
                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var getuser = db.Users.Where(x => x.UserId == model.UserId).FirstOrDefault();
                var GetRole = db.Roles.Where(x => x.RoleId == model.RoleId).FirstOrDefault();
                getuser.Roles.Add(GetRole);
                db.SaveChanges();

                var role = Backbone.StoreUser(db, model.UserId).Roles.Select(x => x.RoleId).ToList();
                model.RolesList = (from a in model.store.Roles where !role.Contains(a.RoleId) orderby a.RoleName select a).AsEnumerable().Select(x => new SelectListItem() { Value = x.RoleId.ToString(), Text = x.RoleName }).ToList();

                TempData["message"] = "The role has been attached Successfully.";
                return RedirectToAction("GrantStoreUserRole", "Dashboard", new { Id = model.store.ProcessInstaceId, UserId=model.UserId, area = "Admin" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        public ActionResult RemoveStoreUserRole(int Id, Guid RoleId, Guid UserId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var GetStore = db.Store.Where(x => x.Id == Id).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var GetRole = db.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault();
                if (GetRole == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var getuser = db.Users.Where(x => x.UserId == UserId).FirstOrDefault();
                if (getuser == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                getuser.Roles.Remove(GetRole);
                db.SaveChanges();
                TempData["message"] = "The role has been removed Successfully.";
                return RedirectToAction("GrantStoreUserRole", "Dashboard", new { Id = GetStore.ProcessInstaceId,UserId=getuser.UserId, area = "Admin" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });
            }
        }

        #endregion

        #region contact info 

        public ActionResult ContactInfoList(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.contactInfoList = Backbone.GetStoreContactInfo(db, Id);
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }


        public ActionResult NewContactInfo(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }                
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }

        [HttpPost]
        public ActionResult NewContactInfo(DashboardViewModel model)
        {
            try
            {
               // DashboardViewModel model = new DashboardViewModel();
               var store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                if (base.ModelState.IsValid)
                {
                    //validate email address
                    var getemail = db.ContactInfo.Where(x => x.EmailAddress == model.contactform.EmailAddress).ToList();
                    if(getemail.Any())
                    {
                        TempData["message"] ="The Email address <b>"+model.contactform.EmailAddress+"</b> has already been used by someone. Please enter another email address";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    ContactInfo contactInfo = new ContactInfo()
                    {
                        FirstName = model.contactform.FirstName,
                        LastName = model.contactform.LastName,
                        EmailAddress = model.contactform.EmailAddress,
                        MobileNo = model.contactform.MobileNumber,
                        ModifiedBy = base.User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    this.db.ContactInfo.AddObject(contactInfo);
                    store.ContactInfo.Add(contactInfo);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat(new string[] { "<b>", model.contactform.FirstName, "</b> <b>", model.contactform.LastName, "</b>  was Successfully Added" });
                   return RedirectToAction("ContactInfoList", "Dashboard", new {Id=store.ProcessInstaceId, area = "Admin" });
                }
                TempData["message"] = "<b>Ops!</b> something went wrong. Please make sure you enter all fields";
                TempData["messageType"] = "danger";
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }

        public ActionResult RemoveContactInfo(Guid Id, int ContactInfoId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var store = Backbone.GetStore(db, Id);
                if (store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var GetContact = db.ContactInfo.Where(x => x.Id == ContactInfoId).FirstOrDefault();
                if(GetContact==null)
                {
                    TempData["message"] ="Record cannot be deleted. Please try again or contact the system administrator";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("NewContactInfo", "Dashboard", new {Id=Id, area = "Admin" });
                }
                ContactInfo del = new ContactInfo {
                };
                db.ContactInfo.DeleteObject(GetContact);
                store.ContactInfo.Remove(GetContact);
                db.SaveChanges();
                TempData["message"] = "Contact Information deleted successfully.";
                return RedirectToAction("ContactInfoList", "Dashboard", new { Id = store.ProcessInstaceId, area = "Admin" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }

        public ActionResult EditContactInfo(Guid Id, int ContactInfoId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
               var store = Backbone.GetStore(db, Id);
                if (store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var GetContact = db.ContactInfo.Where(x => x.Id == ContactInfoId).FirstOrDefault();
                if (GetContact == null)
                {
                    TempData["message"] = "Record cannot be deleted. Please try again or contact the system administrator";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("NewContactInfo", "Dashboard", new { Id = Id, area = "Admin" });
                }
                model.contactform = new SecurityGuard.Models.ContactInfoForm();
                model.contactform.FirstName = GetContact.FirstName;
                model.contactform.LastName = GetContact.LastName;
                model.contactform.EmailAddress = GetContact.EmailAddress;
                model.contactform.MobileNumber = GetContact.MobileNo;
                model.contactform.Id = GetContact.Id;
                model.store = store;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }


        [HttpPost]
        public ActionResult EditContactInfo(DashboardViewModel model)
        {
            try
            {
                var store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                if (base.ModelState.IsValid)
                {
                    var GetContact = store.ContactInfo.Where(x => x.Id == model.contactform.Id).FirstOrDefault();
                    if(GetContact != null)
                    {
                        GetContact.FirstName = model.contactform.FirstName;
                        GetContact.LastName = model.contactform.LastName;
                        GetContact.EmailAddress = model.contactform.EmailAddress;
                        GetContact.MobileNo = model.contactform.MobileNumber;
                        GetContact.ModifiedBy = User.Identity.Name;
                        GetContact.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        base.TempData["message"] = string.Concat(new string[] { "<b>", model.contactform.FirstName, "</b> <b>", model.contactform.LastName, "</b>  was Successfully updated" });
                        return RedirectToAction("ContactInfoList", "Dashboard", new { Id = store.ProcessInstaceId, area = "Admin" });
                    }
                }
                TempData["message"] = "<b>Ops!</b> something went wrong. Please make sure you enter all fields";
                TempData["messageType"] = "danger";
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }

        #endregion

        #region Address management

        public ActionResult AddressList(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.addressList = Backbone.GetStorAddress(db, Id);
                return View(model);               
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }


        [HttpPost]
        public ActionResult GetLGAId(int stateId)
        {
            List<IntegerSelectListItem> list = (
                from d in this.db.LGA
                where d.StateId == stateId
                orderby d.Name
                select new IntegerSelectListItem()
                {
                    Text = d.Name,
                    Value = d.Id
                }).ToList<IntegerSelectListItem>();
            return base.Json(list);
        }

        public ActionResult NewAddress(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.StateList = (from s in this.db.State select new IntegerSelectListItem(){Text = s.Name, Value = s.Id} into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.LgaList = (from s in this.db.LGA select new IntegerSelectListItem(){Text = s.Name,Value = s.Id} into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }

        [HttpPost]
        public ActionResult NewAddress(DashboardViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //validate email
                    var check = db.AddressBook.Where(x => x.EmailAddress == model.addressform.EmailAddress).ToList();
                    if(check.Any())
                    {
                        model.StateList = (from s in this.db.State select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                        model.LgaList = (from s in this.db.LGA select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                        model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                        base.TempData["messageType"] = "danger";
                        base.TempData["message"] = string.Concat("The Email ", model.addressform.EmailAddress, " already exist. Please try different email");
                        return View(model);
                    }

                    var store = Backbone.GetStore(db, model.store.ProcessInstaceId);

                    AddressBook addressBook = new AddressBook()
                    {
                        AddressTypeId = model.addressform.AddressTypeId,
                        Street = model.addressform.Street,
                        LgaId = model.addressform.LgaId,
                        MobileNumber = model.addressform.MobileNumber,
                        EmailAddress = model.addressform.EmailAddress,
                        GoogleMapURL = model.addressform.GoogleMap,
                        ModifiedBy = base.User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false,
                    };
                    this.db.AddressBook.AddObject(addressBook);
                    store.AddressBook.Add(addressBook);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat("<b>", model.addressform.Street, "</b> was Successfully created");
                   return RedirectToAction("AddressList", "Dashboard", new {Id=model.store.ProcessInstaceId, area = "admin" });

                }
                model.StateList = (from s in this.db.State select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.LgaList = (from s in this.db.LGA select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = string.Concat("Ops! Something went wrong. Please make sure you enter all fields with red *.");
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }


        public ActionResult EditAddress(Guid Id, int addressId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.StateList = (from s in this.db.State select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();              
                model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                AddressBook addressBook = (from x in model.store.AddressBook where x.Id == addressId select x).FirstOrDefault<AddressBook>();
                model.LgaList = (from s in this.db.LGA where s.StateId == addressBook.LGA.StateId select new IntegerSelectListItem(){Text = s.Name, Value = s.Id} into x orderby x.Text select x).ToList<IntegerSelectListItem>();

                model.addressform = new CompanyAddressForm()
                {
                    AddressTypeId = addressBook.AddressTypeId,
                    Street = addressBook.Street,
                    LgaId = addressBook.LgaId.Value,
                    MobileNumber = addressBook.MobileNumber,
                    EmailAddress = addressBook.EmailAddress,
                    GoogleMap = addressBook.GoogleMapURL
                };
                model.StateId = addressBook.LGA.StateId;
                model.addressform.Id = addressId;               
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }

        [HttpPost]
        public ActionResult EditAddress(DashboardViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    var store = Backbone.GetStore(db, model.store.ProcessInstaceId);

                    var addressType = store.AddressBook.Where(x => x.Id == model.addressform.Id).FirstOrDefault();
                    //AddressBook addressType = (from x in this.db.AddressBook  where x.Id == model.addressform.Id select x).FirstOrDefault<AddressBook>();


                    addressType.AddressTypeId = model.addressform.AddressTypeId;
                    addressType.Street = model.addressform.Street;
                    addressType.LgaId = model.addressform.LgaId;
                    addressType.MobileNumber = model.addressform.MobileNumber;
                    addressType.EmailAddress = model.addressform.EmailAddress;
                    addressType.ModifiedBy = base.User.Identity.Name;
                    addressType.ModifiedDate = DateTime.Now;
                    addressType.GoogleMapURL = model.addressform.GoogleMap;
                    this.db.SaveChanges();                                    
                    base.TempData["message"] = string.Concat("<b>", model.addressform.Street, "</b> was Successfully created");
                    return RedirectToAction("AddressList", "Dashboard", new { Id = model.store.ProcessInstaceId, area = "admin" });

                }
                model.StateList = (from s in this.db.State select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.LgaList = (from s in this.db.LGA select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = string.Concat("Ops! Something went wrong. Please make sure you enter all fields with red *.");
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }


        public ActionResult RemoveAddress(Guid Id, int addressId)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var store = Backbone.GetStore(db, Id);
                if (store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var Getaddress =store.AddressBook.Where(x => x.Id == addressId).FirstOrDefault();
                if (Getaddress == null)
                {
                    TempData["message"] = "Record cannot be deleted. Please try again or contact the system administrator";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("AddressList", "Dashboard", new { Id = Id, area = "Admin" });
                }
                AddressBook del = new AddressBook
                {
                };
                db.AddressBook.DeleteObject(Getaddress);
                store.AddressBook.Remove(Getaddress);
                db.SaveChanges();
                TempData["message"] = "Address Information has been deleted successfully.";
                return RedirectToAction("AddressList", "Dashboard", new { Id = store.ProcessInstaceId, area = "Admin" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Store", new { area = "Setup" });

            }
        }
        #endregion

        public ActionResult DocumentsUploadedPath(string path)
        {
            try
            {
                var filepath = new Uri(path);
                if (System.IO.File.Exists(filepath.AbsolutePath))
                {
                    byte[] filedata = System.IO.File.ReadAllBytes(filepath.AbsolutePath);
                    string contentType = MimeMapping.GetMimeMapping(filepath.AbsolutePath);

                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = path,
                        Inline = true,
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(filedata, contentType);
                }
                else
                {
                    return null;
                    throw new Exception("ERROR: System could not generate report.");
                }
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        #region store approval methods
        public ActionResult RegistrationList()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var rowsToShow = Backbone.GetNewStoreRegistration(db, System.Web.Security.Roles.GetRolesForUser(User.Identity. Name),Properties.Settings.Default.StoreRegistrationWorkFlowId);
                model.StoreApproval = rowsToShow.OrderByDescending(x => x.ModifiedDate).ToList();
                model.documentPath = Properties.Settings.Default.DocumentPath;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult ApprovedRegistration()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var rowsToShow = Backbone.GetApprovedRegistration(db, System.Web.Security.Roles.GetRolesForUser(User.Identity.Name), Properties.Settings.Default.StoreRegistrationWorkFlowId);
                model.StoreApproval = rowsToShow.OrderByDescending(x => x.ModifiedDate).ToList();
                model.documentPath = Properties.Settings.Default.DocumentPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult RejectedRegistration()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var rowsToShow = Backbone.GetRejectedRegistration(db, System.Web.Security.Roles.GetRolesForUser(User.Identity.Name), Properties.Settings.Default.StoreRegistrationWorkFlowId);
                model.StoreApproval = rowsToShow.OrderByDescending(x => x.ModifiedDate).ToList();
                model.documentPath = Properties.Settings.Default.DocumentPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult PendingRegistration()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var rowsToShow = Backbone.GetPendingRegistration(db, System.Web.Security.Roles.GetRolesForUser(User.Identity.Name), Properties.Settings.Default.StoreRegistrationWorkFlowId);
                model.StoreApproval = rowsToShow.OrderByDescending(x => x.ModifiedDate).ToList();
                model.documentPath = Properties.Settings.Default.DocumentPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult OwnedByRegistration()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var rowsToShow = Backbone.GetOwnStoreRegistration(db, System.Web.Security.Roles.GetRolesForUser(User.Identity.Name), Properties.Settings.Default.StoreRegistrationWorkFlowId);
                model.StoreApproval = rowsToShow.OrderByDescending(x => x.ModifiedDate).ToList();
                model.documentPath = Properties.Settings.Default.DocumentPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult RegistrationDetails(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var store = Backbone.GetStore(db, Id);
                if (store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.store = store;
                model.addressList = Backbone.GetStorAddress(db, Id);
                model.approvalForm = new ApprovalForm();
                model.approvalForm.Id = Id;
                model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                model.store.OwnedBy = User.Identity.Name;
               this.db.SaveChanges();
               Project.DAL.WorkflowSteps currentStep = model.store.WorkflowSteps.FirstOrDefault();

                model.WorkflowStepActions = Backbone.GetWorkflowStepActionForStoreRegistration(db, Id);
                model.OtherWorkflowSteps = Backbone.GetOtherStepsForWorkflowRegistration(db, Id);
                model.documentPath = Properties.Settings.Default.DocumentPath;
                model.ActionLogs = model.store.StoreAction.ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult RegistrationDetails(DashboardViewModel model)
        {
            try
            {
                
                var store = Backbone.GetStore(db, model.approvalForm.Id);
                if (store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
               // model.approvalForm = new ApprovalForm();
                model.store = store;               
                model.addressList = Backbone.GetStorAddress(db, store.ProcessInstaceId);
                model.contactInfoList = Backbone.GetStoreContactInfo(db, store.ProcessInstaceId);
                model.StoreProductCategory = model.store.ProductCategory.ToList();
               
                Project.DAL.WorkflowSteps currentStep = model.store.WorkflowSteps.FirstOrDefault();

                model.WorkflowStepActions = Backbone.GetWorkflowStepActionForStoreRegistration(db, store.ProcessInstaceId);
                model.OtherWorkflowSteps = Backbone.GetOtherStepsForWorkflowRegistration(db, store.ProcessInstaceId);
                
                model.documentPath = Properties.Settings.Default.DocumentPath;
                model.ActionLogs = model.store.StoreAction.ToList();

                if (ModelState.IsValid)
                {
                    var GetUser = db.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

                    var GetUserDetails = GetUser.UserDetail.FirstOrDefault();
                    if (GetUserDetails != null)
                    {
                        KeyValuePair<bool, string> result = Backbone.ApplyActionStoreApplication(db, model.approvalForm);

                        TempData["message"] = result.Value;
                        if (!result.Key)
                            TempData["messageType"] = "alert-danger";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                     
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "You cannot make approval for this application because your contact information has not been updated. Please contact the system administrator.";
                        return RedirectToAction("Index");
                    }


                }
             
                TempData["message"] = "Cannot apply action. Please make sure you enter all fields";
                return RedirectToAction("Index");
               
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult RegistrationStatus(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var store = Backbone.GetStore(db, Id);
                if (store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.store = store;
                model.addressList = Backbone.GetStorAddress(db, Id);
                model.approvalForm = new ApprovalForm();
                model.approvalForm.Id = Id;
                model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                model.store.OwnedBy = User.Identity.Name;
               
                model.documentPath = Properties.Settings.Default.DocumentPath;
                model.ActionLogs = model.store.StoreAction.ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }



       
        #endregion

    }
}
