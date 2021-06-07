using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class PaymentMethodViewModel
    {
        public PaymentMethodForm PaymentMethodform { get; set; }
        public List<PaymentMethod> paymentMethods { get; set; }
        public string documentPath { get; set; }
        public string documentValue { get; set; }
    }
    public class PaymentMethodForm
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter The Payment Method Name")]
        public string Name { get; set; }      
        [Display(Name = "Logo Path")]
        [Required(ErrorMessage = "Please Upload payment method Logo")]
        public HttpPostedFileBase LogoPath { get; set; }
        public bool IsDeleted { get; set; }
       
    }
}