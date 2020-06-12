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

        public List<Size> Rows{get; set;}

        public List<IntegerSelectListItem> SizeTypeList{get;set;}

        public SizeTypeForm sizeTypeform { get; set; }

        public List<SizeType> TypeList { get; set; }

    }

    public class SizeForm
    {
       
        public int Id{get;set;}

        [Display(Name = "Size Name")]
        [Required(ErrorMessage = "Please enter Size Name")]
        public string Name{get;set;}       

        [Display(Name = "Size Type")]
        [Required(ErrorMessage = "Please select Size Type")]
        public int SizeTypeId{get;set;}

        public bool IsDeleted { get; set; }
    }

    public class SizeTypeForm
    {
        public int Id { get; set; }

        [Display(Name = "Size Name")]
        [Required(ErrorMessage = "Please enter Size Type")]
        public string Name { get; set; }       

        public bool IsDeleted { get; set; }
    }
}