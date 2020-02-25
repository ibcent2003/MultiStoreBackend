using Elmah;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class AlertManagementController : Controller
    {
        //
        // GET: /Setup/AlertManagement/
        private PROEntities db = new PROEntities();

        public ActionResult Index()
        {
            ActionResult action;
            try
            {
                List<Alert> list = this.db.Alert.ToList<Alert>();
                action = base.View(new AlertViewModel()
                {
                    Rows = (
                        from x in list
                        orderby x.Title
                        select x).ToList<Alert>()
                });
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }


        public ActionResult Create()
        {
            ActionResult action;
            try
            {
                AlertViewModel alertViewModel = new AlertViewModel()
                {
                    WorkflowList = (
                        from s in this.db.Workflow where s.IsDeleted==false
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.Id
                        }).ToList<IntegerSelectListItem>()
                };
                action = base.View(alertViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(AlertViewModel model)
        {
            ActionResult action;
            try
            {
                if (!base.ModelState.IsValid)
                {
                    action = base.View(model);
                }
                else if (!(
                    from m in this.db.Alert
                    where m.Title == model.alertForm.Title && m.WorkflowId == model.alertForm.WorkFlowId
                    select m).ToList<Alert>().Any<Alert>())
                {
                    Alert alert = new Alert()
                    {
                        Title = model.alertForm.Title,
                        WorkflowId = model.alertForm.WorkFlowId,
                        SubjectEmail = model.alertForm.SubjectEmail,
                        SubjectSms = model.alertForm.SubjectSms,
                        Email = model.alertForm.Email,
                        Sms = model.alertForm.Sms,
                        ModifiedBy = base.User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.alertForm.IsDeleted,
                    };
                    this.db.Alert.AddObject(alert);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat("<b>", model.alertForm.Title, "</b> was Successfully created");
                    action = base.RedirectToAction("Index");
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    base.TempData["message"] = string.Concat("The Name", model.alertForm.Title, " already exist. Please try different Name");
                    model.WorkflowList = (
                        from s in this.db.Workflow
                        where s.IsDeleted == false
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.Id
                        }).ToList<IntegerSelectListItem>();
                    action = base.View(model);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                ErrorSignal.FromCurrentContext().Raise(exception);
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = Settings.Default.GenericExceptionMessage;
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }


        public ActionResult Edit(int Id)
        {
            ActionResult action;
            try
            {
                AlertViewModel alertViewModel = new AlertViewModel()
                {
                    WorkflowList = (
                        from s in this.db.Workflow
                        where s.IsDeleted == false
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.Id
                        }).ToList<IntegerSelectListItem>()
                };
                Alert alert = (
                    from x in this.db.Alert
                    where x.Id == Id
                    select x).FirstOrDefault<Alert>();
                alertViewModel.alertForm = new AlertForm()
                {
                    Title = alert.Title,
                    WorkFlowId = alert.WorkflowId,
                    SubjectSms = alert.SubjectSms,
                    Sms = alert.Sms,
                    SubjectEmail = alert.SubjectEmail,
                    Email = alert.Email,
                    Id = Id,
                    IsDeleted = alert.IsDeleted,
                };
                action = base.View(alertViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(AlertViewModel model)
        {
            ActionResult action;
            try
            {
                model.WorkflowList = (
                    from s in this.db.Workflow
                    where s.IsDeleted == false
                    select new IntegerSelectListItem()
                    {
                        Text = s.Name,
                        Value = s.Id
                    }).ToList<IntegerSelectListItem>();
                Alert title = (
                    from x in this.db.Alert
                    where x.Id == model.alertForm.Id
                    select x).FirstOrDefault<Alert>();
                title.Title = model.alertForm.Title;
                title.WorkflowId = model.alertForm.WorkFlowId;
                title.SubjectSms = model.alertForm.SubjectSms;
                title.Sms = model.alertForm.Sms;
                title.SubjectEmail = model.alertForm.SubjectEmail;
                title.Email = model.alertForm.Email;
                title.ModifiedBy = base.User.Identity.Name;
                title.ModifiedDate = DateTime.Now;
                title.IsDeleted = model.alertForm.IsDeleted;
                this.db.SaveChanges();
                base.TempData["message"] = string.Concat("<b>", model.alertForm.Title, "</b> was Successfully updated");
                action = base.RedirectToAction("Index");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }
    }
}
