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
    public class StateListController : Controller
    {
        //
        // GET: /Setup/StateList/
        private PROEntities db = new PROEntities();
        public ActionResult Index()
        {
            try
            {
                StateViewModel model = new StateViewModel();
                model.Rows = db.State.ToList();
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

        public ActionResult Edit(int Id)
        {
            try
            {
                var Getstate = db.State.Where(x => x.Id == Id).FirstOrDefault();
                if (Getstate == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                StateViewModel model = new StateViewModel();
                model.stateForm = new  StateForm();
                model.stateForm.Id = Getstate.Id;
                model.stateForm.Name = Getstate.Name;
                model.stateForm.StateTransit = Getstate.StateTransit;
                model.stateForm.Fee = Getstate.Fees;
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
        public ActionResult Edit(StateViewModel model)
        {
            try
            {
                var getstate = db.State.Where(x => x.Id == model.stateForm.Id).FirstOrDefault();
                if (getstate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }

                getstate.Name = model.stateForm.Name;
                getstate.Fees = model.stateForm.Fee;
                getstate.StateTransit = model.stateForm.StateTransit;
                getstate.ModifiedBy = User.Identity.Name;
                getstate.ModifiedDate = DateTime.Now;                
                db.SaveChanges();
                TempData["message"] = "The state " + model.stateForm.Name.ToUpper() + " has been updated successfully.";
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


        public ActionResult LGAList(int Id)
        {
            try
            {
                var Getstate = db.State.Where(x => x.Id == Id).FirstOrDefault();
                if (Getstate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                StateViewModel model = new StateViewModel();
                model.lgas = Getstate.LGA.ToList();
                model.state = Getstate;
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

        public ActionResult NewLga(int Id)
        {
            try
            {
                var Getstate = db.State.Where(x => x.Id == Id).FirstOrDefault();
                if (Getstate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                StateViewModel model = new StateViewModel();
                model.state = Getstate;
                model.lgaForm = new LgaForm();
                model.lgaForm.StateId = Getstate.Id;
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
        public ActionResult NewLga(StateViewModel model)
        {
            try
            {
                var Getstate = db.State.Where(x => x.Id == model.lgaForm.StateId).FirstOrDefault();
                if (Getstate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.state = Getstate;
                if (ModelState.IsValid)
                {
                    var validate = db.LGA.Where(x => x.Name == model.lgaForm.Name && x.StateId==model.lgaForm.StateId).ToList();
                    if (validate.Any())
                    {
                       
                        TempData["message"] = "The Lga " + model.lgaForm.Name.ToUpper() + " already exist for '"+model.state.Name+"'. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    LGA addnew = new LGA
                    {
                        Name = model.lgaForm.Name,
                        StateId = model.lgaForm.StateId,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.LGA.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "The Lga " + model.lgaForm.Name.ToUpper() + " has been added successful for " + model.state.Name + "";                 
                    return RedirectToAction("LGAList", "StateList", new {Id=model.lgaForm.StateId, area = "Setup" });
                }
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

    }
}
