using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DocumentTypeController : Controller
    {
        //
        // GET: /Setup/DocumentType/
        private PROEntities db = new PROEntities();

        #region document type

        public ActionResult Index()
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                model.DocumentTypeList = db.DocumentType.OrderBy(x=>x.Name).ToList();              
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult NewDocumentType()
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                model.CategoryList = (
                       from s in this.db.DocumentCategory where s.IsDeleted==false
                       select new IntegerSelectListItem()
                       {
                           Text = s.Name,
                           Value = s.Id
                       }).ToList<IntegerSelectListItem>();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult NewDocumentType(DocumentTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.DocumentType.Where(x => x.Name == model.documentTypeform.Name).ToList();
                    if (validate.Any())
                    {
                        model.CategoryList = (
                     from s in this.db.DocumentCategory where s.IsDeleted==false
                     select new IntegerSelectListItem()
                     {
                         Text = s.Name,
                         Value = s.Id
                     }).ToList<IntegerSelectListItem>();

                        TempData["message"] = "The Document Type "+model.documentTypeform.Name+" already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    DocumentType add = new DocumentType
                    {
                        DocumentCategoryId = model.documentTypeform.DocumentCategoryId,
                        Name = model.documentTypeform.Name,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.documentTypeform.IsDeleted
                    };
                    db.DocumentType.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.documentTypeform.Name + " has been added successful.";
                    return RedirectToAction("Index");
                  
                }
                model.CategoryList = (
                     from s in this.db.DocumentCategory
                     where s.IsDeleted == false
                     select new IntegerSelectListItem()
                     {
                         Text = s.Name,
                         Value = s.Id
                     }).ToList<IntegerSelectListItem>();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult EditDocumentType(int Id)
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                model.CategoryList = (
                     from s in this.db.DocumentCategory
                     where s.IsDeleted == false
                     select new IntegerSelectListItem()
                     {
                         Text = s.Name,
                         Value = s.Id
                     }).ToList<IntegerSelectListItem>();
                var GetDocumentType = db.DocumentType.Where(x => x.Id == Id).FirstOrDefault();
                if (GetDocumentType != null)
                {
                    model.documentTypeform = new DocumentTypeForm();
                    model.documentTypeform.Name = GetDocumentType.Name;
                    model.documentTypeform.IsDeleted = GetDocumentType.IsDeleted;
                    model.documentTypeform.Id = GetDocumentType.Id;
                    model.documentTypeform.DocumentCategoryId = GetDocumentType.DocumentCategoryId;
                }
                return View(model);
                
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult EditDocumentType(DocumentTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CategoryList = (
                     from s in this.db.DocumentCategory
                     where s.IsDeleted == false
                     select new IntegerSelectListItem()
                     {
                         Text = s.Name,
                         Value = s.Id
                     }).ToList<IntegerSelectListItem>();

                    var GetDocumentType = db.DocumentType.Where(x => x.Id == model.documentTypeform.Id).FirstOrDefault();
                    if (GetDocumentType == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cant find document type object"));
                        TempData["message"] = Settings.Default.GenericExceptionMessage;
                        TempData["messageType"] = "danger";
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    GetDocumentType.Name = model.documentTypeform.Name;
                    GetDocumentType.IsDeleted = model.documentTypeform.IsDeleted;
                    GetDocumentType.ModifiedBy = User.Identity.Name;
                    GetDocumentType.ModifiedDate = DateTime.Now;
                    GetDocumentType.DocumentCategoryId = model.documentTypeform.DocumentCategoryId;
                    db.SaveChanges();
                    TempData["message"] = "" + model.documentTypeform.Name + " has been updated successful.";
      
                    return RedirectToAction("Index");

                }
                model.CategoryList = (
                     from s in this.db.DocumentCategory
                     where s.IsDeleted == false
                     select new IntegerSelectListItem()
                     {
                         Text = s.Name,
                         Value = s.Id
                     }).ToList<IntegerSelectListItem>();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        #endregion

        #region document format
        public ActionResult DocumentFormatList()
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                model.DocumentFormatList = db.DocumentFormat.OrderBy(x => x.Name).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        public ActionResult NewDocumentFormat()
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult NewDocumentFormat(DocumentTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.DocumentFormat.Where(x => x.Name == model.Documentformatform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Document Format " + model.Documentformatform.Name + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    DocumentFormat add = new DocumentFormat
                    {
                        Name = model.Documentformatform.Name,
                        Extension = model.Documentformatform.extension,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.Documentformatform.IsDeleted
                    };
                    db.DocumentFormat.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.Documentformatform.Name + " has been added successful.";
                    return RedirectToAction("DocumentFormatList");

                }
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult EditDocumentFormat(int Id)
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                var GetDocumentFormat = db.DocumentFormat.Where(x => x.Id == Id).FirstOrDefault();
                if (GetDocumentFormat == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cant find document format object"));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("DocumentFormatList");
                }
                model.Documentformatform = new DocumentFormatform();
                model.Documentformatform.Name = GetDocumentFormat.Name;
                model.Documentformatform.extension = GetDocumentFormat.Extension;
                model.Documentformatform.IsDeleted = GetDocumentFormat.IsDeleted;
                model.Documentformatform.Id = GetDocumentFormat.Id;
                return View(model);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("DocumentFormatList");
            }
        }

        [HttpPost]
        public ActionResult EditDocumentFormat(DocumentTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetDocumentFormat = db.DocumentFormat.Where(x => x.Id == model.Documentformatform.Id).FirstOrDefault();
                    if (GetDocumentFormat == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cant find document format object"));
                        TempData["message"] = Settings.Default.GenericExceptionMessage;
                        TempData["messageType"] = "danger";
                        return RedirectToAction("DocumentFormatList");
                    }
                    GetDocumentFormat.Name = model.Documentformatform.Name;
                    GetDocumentFormat.Extension = model.Documentformatform.extension;
                    GetDocumentFormat.IsDeleted = model.Documentformatform.IsDeleted;
                    GetDocumentFormat.ModifiedBy = User.Identity.Name;
                    GetDocumentFormat.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "" + model.Documentformatform.Name + " has been updated successful.";

                    return RedirectToAction("DocumentFormatList");

                }
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("DocumentFormatList");
            }
        }
        #endregion

        #region assign document format

        public ActionResult AssginDocumentFormat(int Id)
        {
            ActionResult action;
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                DocumentType documentType = (
                    from x in this.db.DocumentType
                    where x.Id == Id
                    select x).FirstOrDefault<DocumentType>();
                model.Documenttype = documentType;
                List<int> list = (
                    from x in documentType.DocumentFormat
                    select x.Id).ToList<int>();
                model.AvailableDocFormat = (
                    from d in this.db.DocumentFormat
                    where !list.Contains(d.Id) && d.IsDeleted==false
                    select d).ToList<DocumentFormat>();
                model.DocumentFormatList = documentType.DocumentFormat.ToList<DocumentFormat>();
                action = base.View(model);
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
        public ActionResult AssginDocumentFormat(DocumentTypeViewModel model)
        {
            ActionResult action;
            try
            {
                DocumentType documentType = (
                    from d in this.db.DocumentType
                    where d.Id == model.Documenttype.Id
                    select d).FirstOrDefault<DocumentType>();
                List<int> list = (
                    from x in documentType.DocumentFormat
                    select x.Id).ToList<int>();
                model.AvailableDocFormat = (
                    from d in this.db.DocumentFormat
                    where !list.Contains(d.Id) && d.IsDeleted==false
                    select d).ToList<DocumentFormat>();
                model.DocumentFormatList = documentType.DocumentFormat.ToList<DocumentFormat>();
                if (model.DocumentFormatId == 0)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "Please select a document format from the dropdownlist and click on the Add button";
                    action = base.RedirectToAction("AssginDocumentFormat", "DocumentType", new { Id = model.Documenttype.Id, area = "Setup" });
                }
                else if (!(
                    from x in documentType.DocumentFormat
                    where x.Id == model.DocumentFormatId
                    select x).ToList<DocumentFormat>().Any<DocumentFormat>())
                {
                    DocumentFormat documentFormat = (
                        from x in this.db.DocumentFormat
                        where x.Id == model.DocumentFormatId
                        select x).FirstOrDefault<DocumentFormat>();
                    documentType.DocumentFormat.FirstOrDefault<DocumentFormat>();
                    documentType.DocumentFormat.Add(documentFormat);
                    this.db.SaveChanges();
                    model.AvailableDocFormat = (
                        from d in this.db.DocumentFormat
                        where !list.Contains(d.Id)
                        select d).ToList<DocumentFormat>();
                    model.DocumentFormatList = documentType.DocumentFormat.ToList<DocumentFormat>();
                    base.TempData["message"] = "The format has been added successfully.";
                    action = base.RedirectToAction("AssginDocumentFormat", "DocumentType", new { Id = model.Documenttype.Id, area = "Setup" });
                }
                else
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "The Selected Document format already exist. Please select another format";
                    action = base.View(model);
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


        public ActionResult RemoveDocumentFormat(int Id, int FormatId)
        {
            ActionResult action;
            try
            {
                DocumentTypeViewModel documentManagementModel = new DocumentTypeViewModel();
                DocumentType documentType = (
                    from d in this.db.DocumentType
                    where d.Id == Id
                    select d).FirstOrDefault<DocumentType>();
                DocumentFormat documentFormat = (
                    from x in this.db.DocumentFormat
                    where x.Id == FormatId
                    select x).FirstOrDefault<DocumentFormat>();
                documentType.DocumentFormat.Remove(documentFormat);
                this.db.SaveChanges();
                base.TempData["message"] = "The format has been deleted successfully.";
                action = base.RedirectToAction("AssginDocumentFormat", "DocumentType", new { Id = Id, area = "Setup" });
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
        #endregion

        #region document category
        public ActionResult DocumentCategoryList()
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                model.DocumentCategoryList = db.DocumentCategory.OrderBy(x => x.Name).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult NewDocumentCategory()
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult NewDocumentCategory(DocumentTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.DocumentCategory.Where(x => x.Name == model.DocumentCategoryform.Name).ToList();
                    if (validate.Any())
                    {


                        TempData["message"] = "The Document Category " + model.DocumentCategoryform.Name + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    DocumentCategory add = new DocumentCategory
                    {

                        Name = model.DocumentCategoryform.Name,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.DocumentCategoryform.IsDeleted
                    };
                    db.DocumentCategory.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.DocumentCategoryform.Name + " has been added successful.";
                    return RedirectToAction("DocumentCategoryList");

                }               
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }



        public ActionResult EditDocumentCategory(int Id)
        {
            try
            {
                DocumentTypeViewModel model = new DocumentTypeViewModel();
                
                var GetDocumentCategory = db.DocumentCategory.Where(x => x.Id == Id).FirstOrDefault();
                if (GetDocumentCategory != null)
                {
                    model.DocumentCategoryform = new  DocumentCategoryForm();
                    model.DocumentCategoryform.Name = GetDocumentCategory.Name;
                    model.DocumentCategoryform.IsDeleted = GetDocumentCategory.IsDeleted;
                    model.DocumentCategoryform.Id = GetDocumentCategory.Id;
                    
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult EditDocumentCategory(DocumentTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetDocumentCategory = db.DocumentCategory.Where(x => x.Id == model.DocumentCategoryform.Id).FirstOrDefault();
                    if (GetDocumentCategory == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cant find document Category object"));
                        TempData["message"] = Settings.Default.GenericExceptionMessage;
                        TempData["messageType"] = "danger";
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    GetDocumentCategory.Name = model.DocumentCategoryform.Name;
                    GetDocumentCategory.IsDeleted = model.DocumentCategoryform.IsDeleted;
                    GetDocumentCategory.ModifiedBy = User.Identity.Name;
                    GetDocumentCategory.ModifiedDate = DateTime.Now;                    
                    db.SaveChanges();
                    TempData["message"] = "" + model.DocumentCategoryform.Name + " has been updated successful.";
                    return RedirectToAction("DocumentCategoryList");

                }               
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }
        #endregion

    }
}
