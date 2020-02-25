using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class AlertViewModel
    {
        public AlertForm alertForm
        {
            get;
            set;
        }

        public IList<Alert> Rows
        {
            get;
            set;
        }

        public List<IntegerSelectListItem> WorkflowList
        {
            get;
            set;
        }
    }

    public class AlertForm
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please enter Email")]
        public string Email
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        [Display(Name = "SMS")]
        [Required(ErrorMessage = "Please enter SMS")]
        public string Sms
        {
            get;
            set;
        }

        [Display(Name = "Subject Email")]
        [Required(ErrorMessage = "Please enter Subject Email")]
        public string SubjectEmail
        {
            get;
            set;
        }

        [Display(Name = "Subject Sms")]
        [Required(ErrorMessage = "Please enter Subject Sms")]
        public string SubjectSms
        {
            get;
            set;
        }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please enter the Title")]
        public string Title
        {
            get;
            set;
        }

        [Display(Name = "Workflow")]
        [Required(ErrorMessage = "Please select workflow")]
        public int WorkFlowId
        {
            get;
            set;
        }

        public bool IsDeleted { get; set; }

        
    }
}