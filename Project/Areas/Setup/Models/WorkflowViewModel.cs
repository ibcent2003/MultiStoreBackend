using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project.DAL;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
 

namespace Project.Areas.Setup.Models
{
    public class WorkflowViewModel
    {
        public IList<Workflow> Rows
        {
            get;
            set;
        }

        public IList<WorkFlowActions> workflowActionList
        {
            get;
            set;
        }

        public IList<WorkflowSteps> workflowStepList
        {
            get;
            set;
        }

        public IList<WorkflowStepActions> WorkflowStepActionslist
        {
            get;
            set;
        }

        public List<SelectListItem> Roles
        {
            get;
            set;
        }

        public List<SelectListItem> ActionList
        {
            get;
            set;
        }

        public List<SelectListItem> AlertList
        {
            get;
            set;
        }

        public string workflowname
        {
            get;
            set;
        }
        public Workflow workflow { get;set; }


        public WorkflowSteps workflowsteps { get; set; }

        public WorkFlowForm workflowform
        {
            get;
            set;
        }

        public workflowAction workflowformaction
        {
            get;
            set;
        }     

        public workflowStep workflowstepsForm
        {
            get;
            set;
        }

        public workFlowStepActionForm workFlowStepActionform { get; set; }
       
    }
    public class WorkFlowForm
    {
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Please enter Code")]
        public string Code
        {
            get;
            set;
        }

        [Display(Name = "Feedback")]
        [Required(ErrorMessage = "Please enter feedback")]
        public string Feedback
        {
            get;
            set;
        }

        [Display(Name = "Guide Line")]
        [Required(ErrorMessage = "Please enter guideline")]
        public string guideline
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter the Name")]
        public string Name
        {
            get;
            set;
        }

        
    }

    public class workflowAction
    {
        public string Direction
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public bool IsMovable
        {
            get;
            set;
        }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter the Name")]
        public string Name
        {
            get;
            set;
        }

       
    }

   
    public class workflowStep
    {
        public int Id
        {
            get;
            set;
        }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter the Name")]
        public string Name
        {
            get;
            set;
        }

        [Display(Name = "Priority")]
        [Required(ErrorMessage = "Please enter Priority")]
        public int Priority
        {
            get;
            set;
        }

        [Display(Name = "RoleName")]
        [Required(ErrorMessage = "Please enter RoleName")]
        public string RoleName
        {
            get;
            set;
        }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Please enter Status")]
        public string Status
        {
            get;
            set;
        }

 
    }

    public class workFlowStepActionForm
    {
        [Display(Name = "Action Name")]
        [Required(ErrorMessage = "Please enter the Action Name")]
        public int ActionId
        {
            get;
            set;
        }

        [Display(Name = "Alert")]
        [Required(ErrorMessage = "Please enter the Alert")]
        public int AlertId
        {
            get;
            set;
        }

        [Display(Name = "Display Name")]
        [Required(ErrorMessage = "Please enter Display Name")]
        public string DisplayName
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        [Display(Name = "PresetReason")]
        [Required(ErrorMessage = "Please enter Preset Reason")]
        public string PresetReason
        {
            get;
            set;
        }


    }


}