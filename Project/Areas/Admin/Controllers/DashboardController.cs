using Project.Areas.Admin.Models;
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
           // this.roleService = new RoleService(Roles.Provider);
            this.membershipService = new MembershipService(Membership.Provider);
           // this.rows = new List<Users>();
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
                if (Roles.GetRolesForUser(User.Identity.Name).Contains("ADMINISTRATOR"))
                 {
                    return View(model);
                }
                else if (Roles.GetRolesForUser(User.Identity.Name).Contains("Store Admin"))
                {
                    var store = storeDetails();
                    return RedirectToAction("StoreDashboard", "Dashboard", new { area = "Setup", Id=store.ProcessInstaceId });
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

        public ActionResult StoreDashboard(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
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


        public ActionResult StoreUserList(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var GetStore = Backbone.GetStore(db, Id);
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
                        this.services.SendEmailNotificationToUser(alert.SubjectEmail, alert.Email.Replace("%Email%", model.userAccount.EmailAddress).Replace("%uname%", model.userAccount.Username).Replace("%pwd%", model.userAccount.Password).Replace("%First_Name%",model.userAccount.FirstName), model.userAccount.EmailAddress, Settings.Default.EmailReplyTo, alert.Id);
                        this.services.SendSMSNotificationToUser(alert.SubjectSms, alert.Sms.Replace("%First_Name%", model.userAccount.FirstName), model.userAccount.MobileNumber, "FORTRESS", alert.Id);
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




    }
}
