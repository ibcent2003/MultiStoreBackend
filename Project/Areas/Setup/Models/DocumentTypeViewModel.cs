using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class DocumentTypeViewModel
    {
        public List<DocumentType> DocumentTypeList { get; set; }
        public List<DocumentFormat> DocumentFormatList { get; set; }

        public DocumentTypeForm documentTypeform { get; set; }

        public DocumentFormatform Documentformatform { get; set; }

        public DocumentCategoryForm DocumentCategoryform { get; set; }

        public DocumentType Documenttype { get; set; }
        public List<DocumentFormat> AvailableDocFormat { get; set; }
        public int DocumentFormatId {get; set;}

        public List<DocumentCategory> DocumentCategoryList { get; set; }
        public List<IntegerSelectListItem> CategoryList { get;set;}

    }

    public class DocumentTypeForm
    {
        public int Id { get; set; }


        [Display(Name = "Document Category")]
        [Required(ErrorMessage = "Please a Category")]
        public int DocumentCategoryId { get; set; }

        [Required(ErrorMessage = "Please enter document type name")]
        [Display(Name = "Document Type Name")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

    }

    public class DocumentFormatform
    {

        [Display(Name = "Document Category")]
        [Required(ErrorMessage = "Please a Category")]
        public int DocumentCategoryId{get;set;}

        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter document type name")]
        [Display(Name = "Document Format Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please enter document format extension")]
        public string extension { get; set; }

        public bool IsDeleted { get; set; }

    }

    public class DocumentCategoryForm
    {
        public int Id { get; set; }      
        [Required(ErrorMessage = "Please enter document category name")]    
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

    }
}