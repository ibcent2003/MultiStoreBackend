using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MenuTypeController : Controller
    {
        private PROEntities db = new PROEntities();
        public ActionResult Index()
        {
            try
            {
                MenuTypeViewModel model = new MenuTypeViewModel();
                model.MenuTypeList = db.MenuType.ToList();
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

        public ActionResult NewMenuType()
        {
            try
            {
                MenuTypeViewModel model = new MenuTypeViewModel();              
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
        public ActionResult NewMenuType(MenuTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.MenuType.Where(x => x.Name == model.MenuTypeform.Name).ToList();
                    if (validate.Any())
                    {                      
                        TempData["message"] = "The Menu Type Name " + model.MenuTypeform.Name.ToUpper() + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    MenuType add = new MenuType
                    {
                        Name = model.MenuTypeform.Name,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.MenuTypeform.IsDeleted,
                      
                    };
                    db.MenuType.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.MenuTypeform.Name.ToUpper() + " has been added successful.";
                    return RedirectToAction("Index");

                }               
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter the menu type Name.";
                TempData["messageType"] = "danger";
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

        public ActionResult EditMenuType(int Id)
        {
            try
            {
                var GetMenuType = db.MenuType.Where(x => x.Id == Id).FirstOrDefault();
                if (GetMenuType == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                MenuTypeViewModel model = new MenuTypeViewModel();
                model.MenuTypeform = new MenuTypeForm();
                model.MenuTypeform.Id = GetMenuType.Id;
                model.MenuTypeform.Name = GetMenuType.Name;
                model.MenuTypeform.IsDeleted = GetMenuType.IsDeleted;
         
         
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
        public ActionResult EditMenuType(MenuTypeViewModel model)
        {
            try
            {
                var GetMenuType = db.MenuType.Where(x => x.Id == model.MenuTypeform.Id).FirstOrDefault();
                if (GetMenuType == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                GetMenuType.Name = model.MenuTypeform.Name;
                GetMenuType.IsDeleted = model.MenuTypeform.IsDeleted;
                GetMenuType.ModifiedBy = User.Identity.Name;
                GetMenuType.ModifiedDate = DateTime.Now;
               
                db.SaveChanges();
                TempData["message"] = "The Size name " + model.MenuTypeform.Name.ToUpper() + " has been updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

    }
}
