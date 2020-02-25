using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Areas.SecurityGuard.Models;
using SecurityGuard.Services;
using SecurityGuard.Interfaces;
using Project.DAL;
using System.Data;

namespace Project.Areas.SecurityGuard.Controllers
{
    public class RoleManagementController : Controller
    {
        private PROEntities db = new PROEntities();

         #region ctors

        private readonly IRoleService roleService;

        public RoleManagementController()
        {
            this.roleService = new RoleService(System.Web.Security.Roles.Provider);
        }

        #endregion

        public ActionResult Index()
        {
            try
            {
                Project.Areas.SecurityGuard.Models.RoleManagementViewModel model = new Project.Areas.SecurityGuard.Models.RoleManagementViewModel()
                {
                    RoleList = db.Roles.OrderBy(x => x.RoleName).ToList(),
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult NewRole()
        {
            try
            {
                RoleManagementViewModel model = new RoleManagementViewModel();
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
        public ActionResult NewRole(RoleManagementViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    //check for duplicate role.
                    var validate = db.Roles.Where(x => x.RoleName == model.roleForm.RoleName).ToList();
                    if (validate.Any())
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Role name " +model.roleForm.RoleName+" already exist. Please enter another name";
                        return View(model);
                    }                    
                    roleService.CreateRole(model.roleForm.RoleName);                    
                    TempData["message"] = "<b>" + model.roleForm.RoleName + "</b> was Successfully Created";              
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

        public ActionResult EditRole(string Id)
        {
            try
            {
                Guid roleId = new Guid(Id);
                var GetRole = db.Roles.Where(x => x.RoleId == roleId).FirstOrDefault();
                if (null == GetRole)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "An Error occur. Please try again later or contact the system administrator";                   
                    return RedirectToAction("Index");
                }
                RoleManagementViewModel model = new RoleManagementViewModel();
                model.roleForm = new RoleForm();
                model.roleForm.roleId = roleId;
                model.roleForm.RoleName = GetRole.RoleName;
                model.roleForm.Description = GetRole.Description;
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
        public ActionResult EditRole(RoleManagementViewModel model)
        {
            try
            {
              if (ModelState.IsValid)
              {
                  var GetRole = db.Roles.Where(x => x.RoleId == model.roleForm.roleId).FirstOrDefault();
                  GetRole.RoleName =model.roleForm.RoleName;
                  GetRole.Description = model.roleForm.Description;                 
                  db.SaveChanges();
                  TempData["message"] = "<b>" + model.roleForm.RoleName + "</b> was Successfully updated";
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

        public ActionResult RemoveRole(string Id)
        {
            try
            {
                RoleManagementViewModel model = new RoleManagementViewModel();
                Guid roleId = new Guid(Id);
                var GetRole = db.Roles.Where(x => x.RoleId == roleId).FirstOrDefault();
                db.Roles.DeleteObject(GetRole);
                db.SaveChanges();
                TempData["message"] = "<b>" + GetRole.RoleName + "</b> was Successfully removed";
                return RedirectToAction("Index");              
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index");
            }
        }



    }
}
