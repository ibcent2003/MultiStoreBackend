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
    public class ProductColorController : Controller
    {
        //
        // GET: /Setup/ProductColor/
        private PROEntities db = new PROEntities();

        public ActionResult Index()
        {
            try
            {
                ProductColorViewModel model = new ProductColorViewModel();
                model.ProductColorlist = db.ProductColor.ToList();               
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

        public ActionResult NewProductColor()
        {
            try
            {
                ProductColorViewModel model = new ProductColorViewModel();
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
        public ActionResult NewProductColor(ProductColorViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.ProductColor.Where(x => x.Name == model.Productcolorform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Color Name " + model.Productcolorform.Name + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    ProductColor add = new ProductColor
                    {
                        Name = model.Productcolorform.Name,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.Productcolorform.IsDeleted
                    };
                    db.ProductColor.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.Productcolorform.Name + " has been added successful.";
                    return RedirectToAction("Index");

                }
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter the Color Name.";
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



        public ActionResult EditProductColor(int Id)
        {
            try
            {
                var GetColor = db.ProductColor.Where(x => x.Id == Id).FirstOrDefault();
                if (GetColor == null)
                {
                    
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                ProductColorViewModel model = new ProductColorViewModel();
                model.Productcolorform = new  ProductColorForm();
                model.Productcolorform.Id = GetColor.Id;
                model.Productcolorform.Name = GetColor.Name;
                model.Productcolorform.IsDeleted = GetColor.IsDeleted;
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
        public ActionResult EditProductColor(ProductColorViewModel model)
        {
            try
            {
                var getColor = db.ProductColor.Where(x => x.Id == model.Productcolorform.Id).FirstOrDefault();
                if(getColor==null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                getColor.Name = model.Productcolorform.Name;
                getColor.IsDeleted = model.Productcolorform.IsDeleted;
                getColor.ModifiedBy = User.Identity.Name;
                getColor.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "The Color name " + model.Productcolorform.Name + " has been updated successfully.";                
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

    }
}
