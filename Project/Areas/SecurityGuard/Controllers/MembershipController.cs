using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Core.Extensions;
using SecurityGuard.Services;
using SecurityGuard.Core.Attributes;
using routeHelpers = SecurityGuard.Core.RouteHelpers;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using Project.Controllers;
using viewModels = Project.Areas.SecurityGuard.ViewModels;
using Project.DAL;
using System.ComponentModel;
using System.Collections.Generic;
using Project.Properties;
using Project.Models;
using Project.Areas.SecurityGuard.Models;
using System.Data;


namespace Project.Areas.SecurityGuard.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    public partial class MembershipController : BaseController
    {

        #region ctors
        private PROEntities db = new PROEntities();
       
       // private ProcessUtility utility = new ProcessUtility();
        private IMembershipService membershipService;
        private readonly IRoleService roleService;
        protected IList<Users> rows;

        public MembershipController()
        {          
            this.roleService = new RoleService(System.Web.Security.Roles.Provider);
            this.membershipService = new MembershipService(Membership.Provider);
            this.rows = new List<Users>();
        }

        #endregion

        #region Index Method
        //[Authorize(Roles = "Administrator, Organisation Admin")]
        public virtual ActionResult Index(string filterby, string searchterm, [DefaultValue(1)] int page, [DefaultValue(12)] int pgsize)
        {
            try
            {               
                ManageUsersViewModel viewModel = new ManageUsersViewModel();
                viewModel.Users = null;
                viewModel.FilterBy = filterby;
                viewModel.SearchTerm = searchterm;

                if (!string.IsNullOrEmpty(filterby))
                {
                    if (filterby == "all")
                    {
                        rows = db.Users.ToList();
                    }
                    else if (!string.IsNullOrEmpty(searchterm))
                    {
                        string query = searchterm.Trim().ToUpper();
                        if (filterby == "email")
                        {
                            rows = (from u in db.Users where u.Memberships.Email.ToUpper().Contains(query) select u).ToList();
                        }
                        else if (filterby == "username")
                        {
                            rows = (from u in db.Users where u.UserName.ToUpper().Contains(query) select u).ToList();
                        }
                    }
                    else
                    {
                        rows = db.Users.ToList();
                    }
                                                 
                    viewModel.Rows = rows.Skip((page - 1) * pgsize).Take(pgsize).ToList();
                }
                viewModel.PagingInfo = new PagingInfo
                {
                    FirstItem = ((page - 1) * pgsize) + 1,
                    LastItem = page * pgsize,
                    CurrentPage = page,
                    ItemsPerPage = pgsize,
                    TotalItems = rows.Count()
                };
                viewModel.PageSize = pgsize;               
                return View(viewModel);

            }
            catch (Exception ex)
            {
                //ToDo: Log with Elmah
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }
        #endregion

        #region Create User Methods

        public virtual ActionResult CreateUser()
        {
            var model = new viewModels.RegisterViewModel()
            {
                RequireSecretQuestionAndAnswer = membershipService.RequiresQuestionAndAnswer
            };
            return View(model);
        }

        /// <summary>
        /// This method redirects to the GrantRolesToUser method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult CreateUser(viewModels.RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "You are missing required fields.");
                return RedirectToAction("CreateUser");
            }
            MembershipUser user;
            MembershipCreateStatus status;
            user = membershipService.CreateUser(model.UserName, model.Password, model.Email, model.SecretQuestion, model.SecretAnswer, model.Approve, out status);

            return routeHelpers.Actions.GrantRolesToUser(user.UserName);
        }

        /// <summary>
        /// An Ajax method to check if a username is unique.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckForUniqueUser(string userName)
        {
            MembershipUser user = membershipService.GetUser(userName);
            JsonResponse response = new JsonResponse();
            response.Exists = (user == null) ? false : true;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Delete User Methods

        [HttpPost]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "DeleteUser")]
        public ActionResult DeleteUser(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new ArgumentNullException("userName");
            }

            try
            {
                membershipService.DeleteUser(UserName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There was an error deleting this user. - " + ex.Message;
            }

            return RedirectToAction("Update", new { userName = UserName });
        }



        #endregion

        #region View User Details Methods

        [HttpGet]
        public ActionResult Update(UserViewModel userVM)
        {
            string userName = userVM.userName;
            MembershipUser user = membershipService.GetUser(userName);

            UserViewModel viewModel = new UserViewModel();
            viewModel.User = user;
            viewModel.RequiresSecretQuestionAndAnswer = membershipService.RequiresQuestionAndAnswer;
            viewModel.Roles = roleService.GetRolesForUser(userName);

            return View(viewModel);
        }

        [HttpPost]
        //[ActionName("Update")]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UpdateUser")]
        public ActionResult UpdateUser(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new ArgumentNullException("userName");
            }

            MembershipUser user = membershipService.GetUser(UserName);

            try
            {
                user.Comment = Request["User.Comment"];
                user.Email = Request["User.Email"];

                membershipService.UpdateUser(user);
                TempData["SuccessMessage"] = "The user was updated successfully!";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "There was an error updating this user.";
            }

            return RedirectToAction("Update", new { userName = user.UserName });
        }


        #region Ajax methods for Updating the user

        [HttpPost]
        public ActionResult Unlock(string userName)
        {
            JsonResponse response = new JsonResponse();

            MembershipUser user = membershipService.GetUser(userName);

            try
            {
                user.UnlockUser();
                response.Success = true;
                response.Message = "User unlocked successfully!";
                response.Locked = false;
                response.LockedStatus = (response.Locked) ? "Locked" : "Unlocked";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "User unlocked failed.";
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult ApproveDeny(string userName)
        {
            JsonResponse response = new JsonResponse();

            MembershipUser user = membershipService.GetUser(userName);

            try
            {
                user.IsApproved = !user.IsApproved;
                membershipService.UpdateUser(user);

                string approvedMsg = (user.IsApproved) ? "Approved" : "Denied";

                response.Success = true;
                response.Message = "User " + approvedMsg + " successfully!";
                response.Approved = user.IsApproved;
                response.ApprovedStatus = (user.IsApproved) ? "Approved" : "Not approved";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "User unlocked failed.";
            }

            return Json(response);
        }

        #endregion

        #endregion

        #region Cancel User Methods

        [HttpPost]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UserCancel")]
        public ActionResult Cancel()
        {
            return RedirectToAction("Index");
        }

        #endregion

        #region update user detail
        public ActionResult EditUser(string username)
        {
            try
            {
                UserManagementViewModel model = new UserManagementViewModel();
                model.userForm = new UserForm();               
                var GetUser = db.Users.Where(x => x.UserName == username).FirstOrDefault();
                var GetMembership = GetUser.Memberships;
                var GetUserDetail = GetUser.UserDetail.FirstOrDefault();
                if (GetUserDetail != null)
                {
                    model.userForm.FirstName = GetUserDetail.FirstName;
                    model.userForm.LastName = GetUserDetail.LastName;
                    model.userForm.MobileNumber = GetUserDetail.MobileNumber;
                    model.userForm.EmailAddress = GetUserDetail.EmailAddres;
                }
                model.userForm.EmailAddress = GetMembership.Email;
                model.userForm.Username = username;              
                model.userForm.IsApproved = GetMembership.IsApproved;
                model.userForm.IsLocked = GetMembership.IsLockedOut;
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


        [HttpPost]
        public ActionResult EditUser(UserManagementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetUser = db.Users.Where(x => x.UserName == model.userForm.Username).FirstOrDefault();
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
                        //db.UserDetail.Attach(GetUserDetail);
                        //db.Entry(GetUserDetail).State = EntityState.Modified;                      
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
                            ModifiedDate= DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            UserId = GetUser.UserId
                        };
                        db.UserDetail.AddObject(addnew);
                        db.SaveChanges();
                    }

                    GetMembership.IsLockedOut = model.userForm.IsLocked;
                    GetMembership.IsApproved = model.userForm.IsApproved;
                    GetMembership.Email = model.userForm.EmailAddress;
                    //db.Memberships.Attach(GetMembership);
                    //db.Entry(GetMembership).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.userForm.FirstName + " " + model.userForm.LastName + "</b> was Successfully updated";
                    return RedirectToAction("Index");

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
        #endregion



        #region Grant Users with Roles Methods

        /// <summary>
        /// Return two lists:
        ///   1)  a list of Roles not granted to the user
        ///   2)  a list of Roles granted to the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public virtual ActionResult GrantRolesToUser(UserViewModel userVM)
        {
            string username = userVM.userName;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index");
            }

            GrantRolesToUserViewModel model = new GrantRolesToUserViewModel();
            model.UserName = username;
            model.AvailableRoles = (string.IsNullOrEmpty(username) ? new SelectList(roleService.GetAllRoles()) : new SelectList(roleService.AvailableRolesForUser(username)));
            model.GrantedRoles = (string.IsNullOrEmpty(username) ? new SelectList(new string[] { }) : new SelectList(roleService.GetRolesForUser(username)));

            return View(model);
        }

        /// <summary>
        /// Grant the selected roles to the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleNames"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult GrantRolesToUser(string userName, string roles)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            string[] roleNames = roles.Substring(0, roles.Length - 1).Split(',');

            if (roleNames.Length == 0)
            {
                response.Success = false;
                response.Message = "No roles have been granted to the user.";
                return Json(response);
            }

            try
            {
                roleService.AddUserToRoles(userName, roleNames);

                response.Success = true;
                response.Message = "The Role(s) has been GRANTED successfully to " + userName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem adding the user to the roles.";
            }

            return Json(response);
        }

        /// <summary>
        /// Revoke the selected roles for the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleNames"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RevokeRolesForUser(string userName, string roles)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            if (string.IsNullOrEmpty(roles))
            {
                response.Success = false;
                response.Message = "Roles is missing";
                return Json(response);
            }

            string[] roleNames = roles.Substring(0, roles.Length - 1).Split(',');

            if (roleNames.Length == 0)
            {
                response.Success = false;
                response.Message = "No roles are selected to be revoked.";
                return Json(response);
            }

            try
            {
                roleService.RemoveUserFromRoles(userName, roleNames);

                response.Success = true;
                response.Message = "The Role(s) has been REVOKED successfully for " + userName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem revoking roles for the user.";
            }

            return Json(response);
        }

        #endregion


        #region Grant users to roles latest

        public ActionResult GrantRole(string Id)
        {
            try
            {
                GrantRoleViewModel model = new GrantRoleViewModel();
                model.AllUsers = (from a in db.Users orderby a.UserName select a).AsEnumerable().Select(x => new SelectListItem() { Value = x.UserId.ToString(), Text = x.UserName }).ToList();
                if (Id == null || Id == "")
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else
                {
                    model.Id = Id;
                    var me = Guid.Parse(Id);
                    var GetUser = db.Users.Where(x => x.UserId == me).FirstOrDefault();
                    var GetUserRole = GetUser.Roles.Select(x => x.RoleId).ToList();
                    var GetAllRole = (from r in db.Roles select r.RoleId).ToList().Except(GetUserRole);

                   model.AllGrantedRole = GetUser.Roles.OrderBy(x=>x.RoleName).ToList();
                
                    model.AllRole = (from r in db.Roles
                                          orderby r.RoleName
                                     where GetAllRole.Contains(r.RoleId)
                                          select r).ToList();
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

        [HttpPost]
        public ActionResult Grant(GrantRoleViewModel model)
        {
            try
            {
                var me = Guid.Parse(model.Id);
                var GetUser = db.Users.Where(x => x.UserId == me).FirstOrDefault();
                var GetUserRole = GetUser.Roles.Select(x => x.RoleId).ToList();
                var GetAllRole = (from r in db.Roles select r.RoleId).ToList().Except(GetUserRole);
               
                model.AllRole = (from r in db.Roles
                                 orderby r.RoleName                              
                                 select r).ToList();            
                if (model.RoleUsed == null)
                {                 
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Role to grant";
                    return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
                }
                string roletext = "";
                model.RoleUsedTex = new List<string> { };
                foreach (string roleId in model.RoleUsed)
                {
                    var RId = Guid.Parse(roleId.ToString());               
                    roletext = (from p in db.Roles where p.RoleId == RId select p.RoleName).FirstOrDefault();
                    model.RoleUsedTex.Add(roletext);
                }
                foreach (string p in model.RoleUsed.ToList())
                {
                    var RsId = Guid.Parse(p.ToString());
                    var RoleN = (from r in db.Roles where r.RoleId == RsId select r).FirstOrDefault();
                    if (model.RoleUsed != null)
                    {                      
                        GetUser.Roles.Add(RoleN);
                        db.SaveChanges();
                    }
                }
                TempData["message"] = "The Role(s) has been GRANTED successfully to " + GetUser.UserName.ToUpper() + "";
                return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }



        [HttpPost]
        public ActionResult Revoke(GrantRoleViewModel model)
        {
            try
            {
                var me = Guid.Parse(model.Id);              
                var GetUser = db.Users.Where(x => x.UserId == me).FirstOrDefault();
                var GetUserRole = GetUser.Roles.Select(x => x.RoleId).ToList();
                var GetAllRole = (from r in db.Roles select r.RoleId).ToList().Except(GetUserRole);
                model.AllGranted = GetUser.Roles.ToList();
                if (model.GrantedUsed == null)
                {                   
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Role to revoke";
                    return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
                }

                string roletext = "";
                model.RoleUsedTex = new List<string> { };

                foreach (string roleId in model.GrantedUsed)
                {
                    var RId = Guid.Parse(roleId.ToString());                            
                    roletext = (from p in db.Roles where p.RoleId == RId select p.RoleName).FirstOrDefault();
                    model.RoleUsedTex.Add(roletext);

                    var existingRole = GetUser.Roles.Where(x=>x.RoleId==RId).ToList();                      
                    if (existingRole != null)
                    {
                        foreach (var role in existingRole)
                        {                          
                            var RsId = Guid.Parse(role.RoleId.ToString());
                            var RoleN = (from r in db.Roles where r.RoleId == RsId select r).FirstOrDefault();
                            if (RoleN != null)
                            {
                            GetUser.Roles.Remove(RoleN);
                            db.SaveChanges();
                            }
                        }
                    }
                }
                TempData["message"] = "The Role(s) has been REVOKED successfully to " + GetUser.UserName.ToUpper() + "";
                return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }
        #endregion


    }
}
