using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
   

    public class StoreRegistrationController : Controller
    {
        Backbone services = new Backbone();
        private PROEntities db = new PROEntities();
        public ActionResult Index()
        {
           try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                string guidelines = (from m in db.Workflow where m.Id == Properties.Settings.Default.StoreRegistrationWorkFlowId select m.guideline).FirstOrDefault();
                TempData["GuideLines"] = guidelines;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Eror404", "Home", new { area = "" });
            }
        }       


        public ActionResult StoreInformation()
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();               
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Eror404", "Home", new { area = "" });
            }
        }
    }
}
