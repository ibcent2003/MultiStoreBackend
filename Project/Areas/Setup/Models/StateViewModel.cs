using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class StateViewModel
    {
        public List<State> Rows { get; set; }
        public StateForm stateForm { get; set; }
        public LgaForm lgaForm { get; set; }
        public List<LGA> lgas { get; set; }
        public State state { get; set; }
    }

    public class StateForm
    {

        public int Id { get; set; }

        [Display(Name = "State Name")]
        [Required(ErrorMessage = "Please enter State Name")]
        public string Name { get; set; }

        [Display(Name = "Delivery Fee")]
        [Required(ErrorMessage = "Please enter Delivery Fees")]
        public decimal Fee { get; set; }

        [Display(Name = "No of Transit")]
        [Required(ErrorMessage = "Please enter transit")]
        public int StateTransit { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class LgaForm
    {

        public int Id { get; set; }

        [Display(Name = "LGA Name")]
        [Required(ErrorMessage = "Please enter LGA Name")]
        public string Name { get; set; }       
        public int StateId { get; set; }

        public bool IsDeleted { get; set; }
    }
}