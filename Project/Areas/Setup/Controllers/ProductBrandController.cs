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
    public class ProductBrandController : Controller
    {
        //
        // GET: /Setup/ProductBrand/
        private PROEntities db = new PROEntities();

        public ActionResult Index()
        {
            try
            {
                ProductBrandViewModel model = new ProductBrandViewModel();
                model.brandList = db.ProductBrand.ToList();               
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

        public ActionResult NewBrand()
        {
            try
            {
                ProductBrandViewModel model = new ProductBrandViewModel();
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
        public ActionResult NewBrand(ProductBrandViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.ProductBrand.Where(x => x.Name == model.ProductBrandform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Brand Name " + model.ProductBrandform.Name + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    ProductBrand add = new ProductBrand
                    {
                        Name = model.ProductBrandform.Name,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.ProductBrandform.IsDeleted
                    };
                    db.ProductBrand.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.ProductBrandform.Name + " has been added successful.";
                    return RedirectToAction("Index");

                }
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter the Brand Name.";
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



        public ActionResult EditBrand(int Id)
        {
            try
            {
                var GetBrand = db.ProductBrand.Where(x => x.Id == Id).FirstOrDefault();
                if(GetBrand==null)
                {
                    
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                ProductBrandViewModel model = new ProductBrandViewModel();
                model.ProductBrandform = new ProductBrandForm();
                model.ProductBrandform.Id = GetBrand.Id;
                model.ProductBrandform.Name = GetBrand.Name;
                model.ProductBrandform.IsDeleted = GetBrand.IsDeleted;
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
        public ActionResult EditBrand(ProductBrandViewModel model)
        {
            try
            {
                var getBrand = db.ProductBrand.Where(x => x.Id == model.ProductBrandform.Id).FirstOrDefault();
                if(getBrand==null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                getBrand.Name = model.ProductBrandform.Name;
                getBrand.IsDeleted = model.ProductBrandform.IsDeleted;
                getBrand.ModifiedBy = User.Identity.Name;
                getBrand.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "The Brand name "+model.ProductBrandform.Name+" has been updated successfully.";                
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
