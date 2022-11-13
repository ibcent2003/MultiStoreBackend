using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;

namespace Project.Areas.Setup.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ShippingCostController : Controller
    {
        //
        // GET: /Setup/ShippingCost/
        private PROEntities db = new PROEntities();

        public ActionResult Index()
        {
            try
            {
                ShippingCostViewModel model = new ShippingCostViewModel();
                model.Rows = db.ShippingCost.ToList();
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



        public ActionResult NewShippingCost()
        {
            try
            {
                ShippingCostViewModel model = new ShippingCostViewModel();
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
        public ActionResult NewShippingCost(ShippingCostViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.ShippingCost.Where(x => x.Name == model.shippingCostForm.Name).ToList();
                    if (validate.Any())
                    {
                      
                        TempData["message"] = "The Package Type " + model.shippingCostForm.Name.ToUpper() + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    ShippingCost add = new ShippingCost
                    {
                        Name = model.shippingCostForm.Name,
                        Fees = model.shippingCostForm.Fees,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.shippingCostForm.IsDeleted,
                       
                    };
                    db.ShippingCost.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.shippingCostForm.Name.ToUpper() + " has been added successful.";
                    return RedirectToAction("Index");

                }               
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter all fileds.";
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


        public ActionResult EditShippingCost(int Id)
        {
            try
            {
                var GetCost = db.ShippingCost.Where(x => x.Id == Id).FirstOrDefault();
                if (GetCost == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                ShippingCostViewModel model = new ShippingCostViewModel();
                model.shippingCostForm = new  ShippingCostForm();
                model.shippingCostForm.Id = GetCost.Id;
                model.shippingCostForm.Name = GetCost.Name;
                model.shippingCostForm.Fees = GetCost.Fees;
                model.shippingCostForm.IsDeleted = GetCost.IsDeleted;                
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
        public ActionResult EditShippingCost(ShippingCostViewModel model)
        {
            try
            {
                var GetCost = db.ShippingCost.Where(x => x.Id == model.shippingCostForm.Id).FirstOrDefault();
                if (GetCost == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                GetCost.Name = model.shippingCostForm.Name;
                GetCost.IsDeleted = model.shippingCostForm.IsDeleted;
                GetCost.ModifiedBy = User.Identity.Name;
                GetCost.ModifiedDate = DateTime.Now;               
                db.SaveChanges();
                TempData["message"] = "The package type " + model.shippingCostForm.Name.ToUpper() + " has been updated successfully.";
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
