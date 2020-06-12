using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class SizeManagementViewModel
    {
        public SizeForm sizeForm{get;set;}

        public IList<Size> Rows{get; set;}

        public List<IntegerSelectListItem> SizeTypeList{get;set;}
    }

    public class SizeForm
    {
       
        public int Id{get;set;}

        [Display(Name = "Size Name")]
        [Required(ErrorMessage = "Please enter Size Name")]
        public string Name{get;set;}       

        [Display(Name = "Size Type")]
        [Required(ErrorMessage = "Please select Size Type")]
        public int WorkFlowId{get;set;}

        public bool IsDeleted { get; set; }
    }
}