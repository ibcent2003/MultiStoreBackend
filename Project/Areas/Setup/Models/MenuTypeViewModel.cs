using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class MenuTypeViewModel
    {
       
        public MenuTypeForm MenuTypeform { get; set; }

        public List<MenuType> MenuTypeList { get; set; }
    }

    public class MenuTypeForm
    {
        public int Id { get; set; }

        [Display(Name = "Menu Type")]
        [Required(ErrorMessage = "Please enter menu Type")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}