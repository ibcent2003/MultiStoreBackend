using Project.Areas.Admin.Models;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
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
        // GET: /Admin/Dashboard/

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
        public ActionResult Index(Guid Id)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                if (Roles.GetRolesForUser(User.Identity.Name).Contains("ADMINISTRATOR"))
                 {
                 }
                
                model.store = Backbone.GetStore(db, Id);
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

    }
}
