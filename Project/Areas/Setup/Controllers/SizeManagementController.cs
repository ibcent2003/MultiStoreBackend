using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SizeManagementController : Controller
    {
        //
        // GET: /Setup/ProductColor/
        private PROEntities db = new PROEntities();

        public ActionResult Index()
        {
            try
            {
                SizeManagementViewModel model = new SizeManagementViewModel();
                model.Rows = db.Size.ToList();               
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

        public ActionResult NewSize()
        {
            try
            {
                SizeManagementViewModel model = new SizeManagementViewModel();

                model.SizeTypeList = (from s in this.db.SizeType where s.IsDeleted == false select new IntegerSelectListItem(){Text = s.Name,Value = s.Id}).ToList<IntegerSelectListItem>();
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
        public ActionResult NewSize(SizeManagementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.Size.Where(x => x.Name == model.sizeForm.Name).ToList();
                    if (validate.Any())
                    {
                        model.SizeTypeList = (from s in this.db.SizeType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        TempData["message"] = "The Size Name " + model.sizeForm.Name.ToUpper() + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    Size add = new Size
                    {
                        Name = model.sizeForm.Name,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.sizeForm.IsDeleted,
                        SizeTypeId = model.sizeForm.SizeTypeId
                    };
                    db.Size.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.sizeForm.Name.ToUpper() + " has been added successful.";
                    return RedirectToAction("Index");

                }
                model.SizeTypeList = (from s in this.db.SizeType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter the Size Name.";
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



        public ActionResult EditSize(int Id)
        {
            try
            {
                var GetSize = db.Size.Where(x => x.Id == Id).FirstOrDefault();
                if (GetSize == null)
                {
                    
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                SizeManagementViewModel model = new SizeManagementViewModel();
                model.sizeForm = new   SizeForm();
                model.sizeForm.Id = GetSize.Id;
                model.sizeForm.Name = GetSize.Name;
                model.sizeForm.IsDeleted = GetSize.IsDeleted;
                model.sizeForm.SizeTypeId = GetSize.SizeTypeId;
                model.SizeTypeList = (from s in this.db.SizeType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
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
        public ActionResult EditSize(SizeManagementViewModel model)
        {
            try
            {
                var getSize = db.Size.Where(x => x.Id == model.sizeForm.Id).FirstOrDefault();
                if(getSize == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.SizeTypeList = (from s in this.db.SizeType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                getSize.Name = model.sizeForm.Name;
                getSize.IsDeleted = model.sizeForm.IsDeleted;
                getSize.ModifiedBy = User.Identity.Name;
                getSize.ModifiedDate = DateTime.Now;
                getSize.SizeTypeId = model.sizeForm.SizeTypeId;
                db.SaveChanges();
                TempData["message"] = "The Size name " + model.sizeForm.Name.ToUpper() + " has been updated successfully.";                
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

        public ActionResult SizeTypeIndex()
        {
            try
            {
                SizeManagementViewModel model = new SizeManagementViewModel();
                model.TypeList = db.SizeType.ToList();
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

        public ActionResult EditSizeType(int Id)
        {
            try
            {
                var GetSize = db.SizeType.Where(x => x.Id == Id).FirstOrDefault();
                if (GetSize == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                SizeManagementViewModel model = new SizeManagementViewModel();
                model.sizeTypeform = new  SizeTypeForm();
                model.sizeTypeform.Id = GetSize.Id;
                model.sizeTypeform.Name = GetSize.Name;
                model.sizeTypeform.IsDeleted = GetSize.IsDeleted;
                
               
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
        public ActionResult EditSizeType(SizeManagementViewModel model)
        {
            try
            {
                var getSize = db.SizeType.Where(x => x.Id == model.sizeTypeform.Id).FirstOrDefault();
                if (getSize == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                
                getSize.Name = model.sizeTypeform.Name;
                getSize.IsDeleted = model.sizeTypeform.IsDeleted;
                getSize.ModifiedBy = User.Identity.Name;
                getSize.ModifiedDate = DateTime.Now;
               
                db.SaveChanges();
                TempData["message"] = "The Size Type " + model.sizeTypeform.Name.ToUpper() + " has been updated successfully.";
                return RedirectToAction("SizeTypeIndex");
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
