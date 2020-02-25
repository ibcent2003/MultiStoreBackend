using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.DAL;
using Project.Areas.Setup.Models;
using Project.Properties;

namespace Project.Areas.Setup.Controllers
{
    public class WorkFlowController : Controller
    {
        //
        // GET: /Setup/WorkFlow/
        private PROEntities db = new PROEntities();

        public ActionResult Index()
        {
            ActionResult action;
            try
            {
                List<Workflow> list = this.db.Workflow.ToList<Workflow>();
                action = base.View(new WorkflowViewModel()
                {
                    Rows = (
                        from x in list
                        orderby x.Name
                        select x).ToList<Workflow>()
                });
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        public ActionResult Create()
        {
            ActionResult action;
            try
            {
                action = base.View(new WorkflowViewModel());
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(WorkflowViewModel model)
        {
            ActionResult action;
            try
            {
                if (!base.ModelState.IsValid)
                {
                    action = base.View(model);
                }
                else if (!(
                    from m in this.db.Workflow
                    where m.Name == model.workflowform.Name
                    select m).ToList<Workflow>().Any<Workflow>())
                {
                    Workflow workflow = new Workflow()
                    {
                        Name = model.workflowform.Name,
                        Code = model.workflowform.Code,
                        guideline = model.workflowform.guideline,
                        feedback = model.workflowform.Feedback,

                        ModifiedBy = base.User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    this.db.Workflow.AddObject(workflow);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat("<b>", model.workflowform.Name, "</b> was Successfully created");
                    action = base.RedirectToAction("Index");
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    base.TempData["message"] = string.Concat("The Name", model.workflowform.Name, " already exist. Please try different Name");
                    action = base.View(model);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = Settings.Default.GenericExceptionMessage;
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }


        public ActionResult Edit(int Id)
        {
            ActionResult action;
            try
            {
                WorkflowViewModel workflowViewModel = new WorkflowViewModel();
                Workflow workflow = (
                    from x in this.db.Workflow
                    where x.Id == Id
                    select x).FirstOrDefault<Workflow>();
                workflowViewModel.workflowform = new WorkFlowForm()
                {
                    Name = workflow.Name,
                    Code = workflow.Code,
                    Feedback = workflow.feedback,
                    guideline = workflow.guideline,
                    Id = Id
                };
                action = base.View(workflowViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(WorkflowViewModel model)
        {
            ActionResult action;
            try
            {
                Workflow name = (
                    from x in this.db.Workflow
                    where x.Id == model.workflowform.Id
                    select x).FirstOrDefault<Workflow>();
                name.Name = model.workflowform.Name;
                name.Code = model.workflowform.Code;
                name.guideline = model.workflowform.guideline;
                name.feedback = model.workflowform.Feedback;
                name.ModifiedBy = base.User.Identity.Name;
                name.ModifiedDate = DateTime.Now;
                this.db.SaveChanges();
                base.TempData["message"] = string.Concat("<b>", model.workflowform.Name, "</b> was Successfully updated");
                action = base.RedirectToAction("Index");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        public ActionResult Details(int Id)
        {
            ActionResult action;
            try
            {
                WorkflowViewModel workflowViewModel = new WorkflowViewModel();
                List<Workflow> list = (
                    from x in this.db.Workflow
                    where x.Id == Id
                    select x).ToList<Workflow>();
                workflowViewModel.Rows = list;
                List<WorkFlowActions> workFlowActions = (
                    from x in this.db.WorkFlowActions
                    where x.WorkFlowId == Id
                    select x).ToList<WorkFlowActions>();
                workflowViewModel.workflowActionList = workFlowActions;
                workflowViewModel.workflowStepList = (
                    from x in this.db.WorkflowSteps
                    where x.WorkflowId == Id
                    orderby x.Priority
                    select x).ToList<WorkflowSteps>();
                workflowViewModel.workflowform = new WorkFlowForm()
                {
                    Id = Id
                };
                workflowViewModel.workflowname = list.FirstOrDefault<Workflow>().Name;
                workflowViewModel.Roles = (
                    from x in this.db.Roles.ToList<Roles>()
                    select new SelectListItem()
                    {
                        Text = x.RoleName,
                        Value = x.RoleName
                    }).ToList<SelectListItem>();
                action = base.View(workflowViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }


        public ActionResult EditWorkflowAction(int workflowId, int workflowActionId)
        {
            ActionResult action;
            try
            {
                WorkflowViewModel workflowViewModel = new WorkflowViewModel()
                {
                    Rows = (
                        from x in this.db.Workflow
                        where x.Id == workflowId
                        select x).ToList<Workflow>(),
                    workflowform = new WorkFlowForm()
                    {
                        Id = workflowId
                    }
                };
                WorkFlowActions workFlowAction = (
                    from x in this.db.WorkFlowActions
                    where x.WorkFlowId == workflowId && x.Id == workflowActionId
                    select x).FirstOrDefault<WorkFlowActions>();
                workflowViewModel.workflowformaction = new workflowAction()
                {
                    Id = workflowActionId,
                    Name = workFlowAction.Name,
                    Direction = workFlowAction.Direction,
                    IsMovable = workFlowAction.IsMovable
                };
                List<WorkFlowActions> list = (
                    from x in this.db.WorkFlowActions
                    where x.WorkFlowId == workflowId
                    select x).ToList<WorkFlowActions>();
                workflowViewModel.workflowActionList = list;
                action = base.View(workflowViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        public ActionResult EditWorkflowAction(WorkflowViewModel model)
        {
            ActionResult action;
            try
            {
                model.Rows = (
                    from x in this.db.Workflow
                    where x.Id == model.workflowform.Id
                    select x).ToList<Workflow>();
                WorkFlowActions name = (
                    from x in this.db.WorkFlowActions
                    where x.WorkFlowId == model.workflowform.Id && x.Id == model.workflowformaction.Id
                    select x).FirstOrDefault<WorkFlowActions>();
                name.Name = model.workflowformaction.Name;
                name.Direction = model.workflowformaction.Direction;
                name.IsMovable = model.workflowformaction.IsMovable;
                name.ModifiedBy = base.User.Identity.Name;
                name.ModifiedDate = DateTime.Now;
                this.db.SaveChanges();
                base.TempData["message"] = string.Concat("<b>", model.workflowformaction.Name, "</b> was Successfully created");
                action = base.RedirectToAction("Details", "WorkFlow", new { area = "Setup", Id = model.workflowform.Id });
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        public ActionResult WorkflowAction(WorkflowViewModel model)
        {
            ActionResult action;
            try
            {
                if (!(
                    from x in this.db.WorkFlowActions
                    where x.WorkFlowId == model.workflowform.Id && x.Name == model.workflowformaction.Name
                    select x).ToList<WorkFlowActions>().Any<WorkFlowActions>())
                {
                    WorkFlowActions workFlowAction = new WorkFlowActions()
                    {
                        WorkFlowId = model.workflowform.Id,
                        Name = model.workflowformaction.Name,
                        IsMovable = model.workflowformaction.IsMovable,
                        Direction = model.workflowformaction.Direction,
                        ModifiedBy = base.User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    this.db.WorkFlowActions.AddObject(workFlowAction);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat("<b>", model.workflowformaction.Name, "</b> was Successfully created");
                    action = base.RedirectToAction("Details", "WorkFlow", new { area = "Setup", Id = model.workflowform.Id });
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    base.TempData["message"] = string.Concat("The Name", model.workflowformaction.Name, " already exist. Please try different Name");
                    action = base.RedirectToAction("Details", "WorkFlow", new { area = "Setup", Id = model.workflowform.Id });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        public ActionResult WorkflowSteps(WorkflowViewModel model)
        {
            ActionResult action;
            try
            {
                if (!(
                    from x in this.db.WorkflowSteps
                    where x.WorkflowId == model.workflowform.Id && x.Name == model.workflowstepsForm.Name
                    select x).ToList<WorkflowSteps>().Any<WorkflowSteps>())
                {
                    WorkflowSteps workflowStep = new WorkflowSteps()
                    {
                        WorkflowId = model.workflowform.Id,
                        Name = model.workflowstepsForm.Name,
                        Priority = model.workflowstepsForm.Priority,
                        RoleName = model.workflowstepsForm.RoleName,
                        Status = model.workflowstepsForm.Status,
                        ModifiedBy = base.User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    this.db.WorkflowSteps.AddObject(workflowStep);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat("<b>", model.workflowstepsForm.Name, "</b> was Successfully created");
                    action = base.RedirectToAction("Details", "WorkFlow", new { area = "Setup", Id = model.workflowform.Id });
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    base.TempData["message"] = string.Concat("The Name", model.workflowstepsForm.Name, " already exist. Please try different Name");
                    action = base.RedirectToAction("Details", "WorkFlow", new { area = "Setup", Id = model.workflowform.Id });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        public ActionResult ConfigureSteps(int workflowId, int stepsId)
        {
            try
            {
                WorkflowViewModel model = new WorkflowViewModel();
               
                var getsteps = db.WorkflowSteps.Where(x => x.Id ==stepsId && x.WorkflowId==workflowId).FirstOrDefault();
                if (getsteps == null)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                    return Redirect("Index");
                }
                var GetWorkflow = db.Workflow.Where(x => x.Id == workflowId).FirstOrDefault();
                model.workflow = GetWorkflow;
                model.workflowsteps = getsteps;

                model.workflowStepList = (
                from x in this.db.WorkflowSteps
                where x.Id == stepsId               
                orderby x.Priority
                select x).ToList<WorkflowSteps>();

                model.ActionList = (
                    from x in this.db.Roles.ToList<Roles>()
                    select new SelectListItem()
                    {
                        Text = x.RoleName,
                        Value = x.RoleName
                    }).ToList<SelectListItem>();

                 model.ActionList = getsteps.Workflow.WorkFlowActions.ToList().Select(x=> new SelectListItem {Text = x.Name, Value=x.Id.ToString()}).ToList();
                 model.AlertList =db.Alert.Where(x=>x.WorkflowId==workflowId && x.IsDeleted==false).ToList().Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }).ToList();                  
                 model.WorkflowStepActionslist = getsteps.WorkflowStepActions.ToList();
                return View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult ConfigureSteps(WorkflowViewModel model)
        {
            try
            {
             
                
                    WorkflowStepActions stepAction = this.db.WorkflowStepActions.Where(x => x.StepId == model.workflowsteps.Id && x.AlertId == model.workFlowStepActionform.AlertId && x.ActionId == model.workFlowStepActionform.ActionId)
                   .FirstOrDefault();
                    if (null == stepAction)
                    {
                        WorkflowStepActions newStepAction = new WorkflowStepActions
                        {
                            StepId = model.workflowsteps.Id,
                            ActionId = model.workFlowStepActionform.ActionId,
                            AlertId = model.workFlowStepActionform.AlertId,
                            DisplayName = model.workFlowStepActionform.DisplayName,
                            PresetReason = model.workFlowStepActionform.PresetReason
                        };
                        db.WorkflowStepActions.AddObject(newStepAction);
                        db.SaveChanges();
                    }
                    WorkflowSteps step = this.db.WorkflowSteps.Where(x => x.Id == model.workflowsteps.Id && x.WorkflowId == model.workflow.Id).FirstOrDefault();
                    model.ActionList = step.Workflow.WorkFlowActions.ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                    model.AlertList = this.db.Alert.Where(x => x.WorkflowId == model.workflow.Id && x.IsDeleted==false).ToList().Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }).ToList();
                    base.TempData["message"] = "Workflow has been configured successfully.";
                    return RedirectToAction("ConfigureSteps", new { workflowId = model.workflow.Id, stepsId = model.workflowsteps.Id });
            }
            catch (Exception ex)
            {
                Exception exception = ex;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult EditConfigureSteps(int Id)
        {
            try
            {
                var GetWorkflowStepActions = db.WorkflowStepActions.Where(x => x.Id == Id).FirstOrDefault();
                WorkflowViewModel model = new WorkflowViewModel();
                model.workFlowStepActionform = new workFlowStepActionForm();

                return View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult RemoveConfigureSteps(int Id)
        {
            try
            {
                WorkflowViewModel model = new WorkflowViewModel();

                return View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


    }
}
