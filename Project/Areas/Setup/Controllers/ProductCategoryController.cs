using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProductCategoryController : Controller
    {
        //
        // GET: /Setup/ProductCategory/
        Backbone services = new Backbone();
        private PROEntities db = new PROEntities();
        private ProcessUtility util = new ProcessUtility();

        public ActionResult Index()
        {
            try
            {
                ProductCategoryModel model = new ProductCategoryModel();
                model.ProductCategorylist = db.ProductCategory.ToList();
                model.documentPath = Properties.Settings.Default.DocumentPath;
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

        public ActionResult NewProductCategory()
        {
            try
            {
                ProductCategoryModel model = new ProductCategoryModel();
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
        public ActionResult NewProductCategory(ProductCategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check duplicate product category name
                    var validate = db.ProductCategory.Where(x => x.Name == model.ProductCategoryform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Product Category Name " + model.ProductCategoryform.Name + " already exist. Please enter another name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    string url = Properties.Settings.Default.DocumentPath;
                    System.IO.Directory.CreateDirectory(url);

                    #region upload logo

                    int max_upload = 5242880;

                    //   string Passportpath = Server.MapPath(this.filepath);
                    List<DocumentInfo> uploadedPassport = new List<DocumentInfo>();

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey1 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<DocumentFormat> Passporttypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedPassport = new List<string>();
                    foreach (var item in Passporttypes)
                    {
                        supportedPassport.Add(item.Extension);
                    }
                    var filePassport = System.IO.Path.GetExtension(model.ProductCategoryform.SampleImage.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for Product Photo sample";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);

                    }
                    else if (model.ProductCategoryform.SampleImage.ContentLength > max_upload)
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);
                    }

                    //store passport
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.ProductCategoryform.SampleImage.FileName);
                    model.ProductCategoryform.SampleImage.SaveAs(url + pName);

                    #endregion

                    ProductCategory addnew = new ProductCategory()
                    {
                        Name = model.ProductCategoryform.Name,
                        SampleImage = pName,
                        Icon = model.ProductCategoryform.Icon,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false,
                       
                    };
                    db.ProductCategory.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "The Product Category " + model.ProductCategoryform.Name + " has been added successfully.";
                    return RedirectToAction("Index", "ProductCategory", new { area = "Setup" });

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


        public ActionResult EditProductCategory(int Id)
        {
            try
            {
                ProductCategoryModel model = new ProductCategoryModel();
                var GetProductCategory = db.ProductCategory.Where(x => x.Id == Id).FirstOrDefault();
                if (GetProductCategory == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.ProductCategoryform = new  ProductCategoryForm();
                model.ProductCategoryform.Name = GetProductCategory.Name;
                model.ProductCategoryform.Id = GetProductCategory.Id;
                model.ProductCategoryform.Icon = GetProductCategory.Icon;
                model.PhotoValue = GetProductCategory.SampleImage;
                model.documentPath = Properties.Settings.Default.DocumentPath;
                model.ProductCategoryform.IsDeleted = GetProductCategory.IsDeleted;
              

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
        public ActionResult EditProductCategory(ProductCategoryModel model)
        {
            try
            {
                var GetProductCategory = db.ProductCategory.Where(x => x.Id == model.ProductCategoryform.Id).FirstOrDefault();
                if (GetProductCategory == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }

                string url = Properties.Settings.Default.DocumentPath;
                System.IO.Directory.CreateDirectory(url);

                if (model.ProductCategoryform.SampleImage != null && model.ProductCategoryform.SampleImage.ContentLength > 0)
                {
                    //delete passport
                    if (GetProductCategory.SampleImage != null)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(url + GetProductCategory.SampleImage);
                        fi.Delete();
                    }
                    #region upload logo

                    int max_upload = 5242880;

                    List<DocumentInfo> uploadedPassport = new List<DocumentInfo>();
                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey1 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<DocumentFormat> Passporttypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedPassport = new List<string>();
                    foreach (var item in Passporttypes)
                    {
                        supportedPassport.Add(item.Extension);
                    }
                    var filePassport = System.IO.Path.GetExtension(model.ProductCategoryform.SampleImage.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for Product Photo sample";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);

                    }
                    else if (model.ProductCategoryform.SampleImage.ContentLength > max_upload)
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The Sample Photo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);
                    }

                    //store passport
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.ProductCategoryform.SampleImage.FileName);
                    model.ProductCategoryform.SampleImage.SaveAs(url + pName);

                    #endregion
                    GetProductCategory.SampleImage = pName;
                }
                GetProductCategory.Name = model.ProductCategoryform.Name;
                GetProductCategory.Icon = model.ProductCategoryform.Icon;
                GetProductCategory.ModifiedBy = User.Identity.Name;
                GetProductCategory.ModifiedDate = DateTime.Now;
                GetProductCategory.IsDeleted = model.ProductCategoryform.IsDeleted;
                db.SaveChanges();
                TempData["message"] = "The Product Category " + model.ProductCategoryform.Name + " has been updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        [Authorize]
        public ActionResult DocumentsUploadedPath(string path)
        {
            try
            {
                var filepath = new Uri(path);
                if (System.IO.File.Exists(filepath.AbsolutePath))
                {
                    byte[] filedata = System.IO.File.ReadAllBytes(filepath.AbsolutePath);
                    string contentType = MimeMapping.GetMimeMapping(filepath.AbsolutePath);

                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = path,
                        Inline = true,
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(filedata, contentType);
                }
                else
                {
                    return null;
                    throw new Exception("ERROR: System could not generate report.");
                }
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }


        public ActionResult SubCategoryList(int Id)
        {
            try
            {
                ProductCategoryModel model = new ProductCategoryModel();
                var getcategory = db.ProductCategory.Where(x => x.Id == Id).FirstOrDefault();
                model.productcategory = getcategory;
                model.ProductSubCategorylist = getcategory.ProductSubCategory.ToList();
                model.ProductSubCategoryform = new ProductSubCategoryForm();
                model.ProductSubCategoryform.ProductCategoryId = Id;
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

        //public ActionResult NewSubCategory(int Id)
        //{
        //    try
        //    {
        //        ProductCategoryModel model = new ProductCategoryModel();
        //        var getcategory = db.ProductCategory.Where(x => x.Id == Id).FirstOrDefault();
        //        model.productcategory = getcategory;

        //        model.ProductSubCategorylist = getcategory.ProductSubCategory.ToList();
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        TempData["message"] = Settings.Default.GenericExceptionMessage;
        //        TempData["messageType"] = "danger";
        //        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        //    }
        //}

        [HttpPost]
        public ActionResult NewSubCategory(ProductCategoryModel model)
        {
            try
            {
             
                var getcategory = db.ProductCategory.Where(x => x.Id == model.ProductSubCategoryform.ProductCategoryId).FirstOrDefault();
                model.productcategory = getcategory;

                model.ProductSubCategorylist = getcategory.ProductSubCategory.ToList();
                if (ModelState.IsValid)
                {
                    var validate = getcategory.ProductSubCategory.Where(x => x.Name == model.ProductSubCategoryform.Name && x.ProductCategoryId == model.ProductSubCategoryform.ProductCategoryId).ToList();
                    if(validate.Any())
                    {
         
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Sub Category " + model.ProductSubCategoryform.Name + " already exist for "+model.productcategory.Name+". Please enter another name";
                        return RedirectToAction("SubCategoryList", "ProductCategory", new {Id=model.ProductSubCategoryform.ProductCategoryId, area = "Setup" });
                    }
                    ProductSubCategory add = new ProductSubCategory
                    {
                        ProductCategoryId = model.ProductSubCategoryform.ProductCategoryId,
                        Name = model.ProductSubCategoryform.Name,
                        IsDeleted = model.ProductSubCategoryform.IsDeleted,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.ProductSubCategory.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "The sub category "+model.ProductSubCategoryform.Name+" has been added successfully.";                  
                    return RedirectToAction("SubCategoryList", "ProductCategory", new { Id = model.ProductSubCategoryform.ProductCategoryId, area = "Setup" });

                }
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter the Sub Category.";
                TempData["messageType"] = "danger";
                return RedirectToAction("SubCategoryList", "ProductCategory", new { Id = model.ProductSubCategoryform.ProductCategoryId, area = "Setup" });

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
        public ActionResult EditSubCategory(ProductCategoryModel model)
        {
            try
            {

                var getcategory = db.ProductCategory.Where(x => x.Id == model.ProductSubCategoryform.ProductCategoryId).FirstOrDefault();
                model.productcategory = getcategory;

                model.ProductSubCategorylist = getcategory.ProductSubCategory.ToList();
                if (ModelState.IsValid)
                {
                    var validate = getcategory.ProductSubCategory.Where(x => x.Id == model.ProductSubCategoryform.Id && x.ProductCategoryId == model.ProductSubCategoryform.ProductCategoryId).FirstOrDefault();
                    if(validate != null)
                    {
                        validate.Name = model.ProductSubCategoryform.Name;
                        validate.IsDeleted = model.ProductSubCategoryform.IsDeleted;
                        validate.ModifiedBy = User.Identity.Name;
                        validate.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        TempData["message"] = "The sub category " + model.ProductSubCategoryform.Name + " has been updated successfully.";
                        return RedirectToAction("SubCategoryList", "ProductCategory", new { Id = model.ProductSubCategoryform.ProductCategoryId, area = "Setup" });
                    }                   
                }
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter the Sub Category.";
                TempData["messageType"] = "danger";
                return RedirectToAction("SubCategoryList", "ProductCategory", new { Id = model.ProductSubCategoryform.ProductCategoryId, area = "Setup" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult ChildCategoryList(int CategoryId, int SubId)
        {
           try
            {
                ProductCategoryModel model = new ProductCategoryModel();
                var GetCategory = db.ProductCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                if(GetCategory != null)
                {
                    model.productcategory = GetCategory;

                    var getsub = GetCategory.ProductSubCategory.Where(x=>x.Id==SubId).FirstOrDefault();
                    if(getsub != null)
                    {
                        model.productsubcategory = getsub;
                        model.ProductChildCategoryList = getsub.ProductChildCategory.Where(x=>x.ProductSubCategoryId==SubId).ToList();

                        model.ProductSubCategoryform = new ProductSubCategoryForm();

                        model.ProductSubCategoryform.Id = SubId;
                        model.ProductSubCategoryform.ProductCategoryId = CategoryId;
                        return View(model);

                    }
                }
               
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

        }


        [HttpPost]
        public ActionResult NewChildCategory(ProductCategoryModel model)
        {
            try
            {
                var sub = db.ProductSubCategory.Where(x => x.Id == model.ProductSubCategoryform.Id).FirstOrDefault();
                var getcategory = db.ProductCategory.Where(x => x.Id == sub.ProductCategoryId).FirstOrDefault();
                model.productcategory = getcategory;
                model.ProductChildCategoryform.ProductSubCategoryId = sub.Id;

                //var getSubCategory = getcategory.ProductSubCategory.Where(x => x.Id == model.ProductSubCategoryform.Id).FirstOrDefault();
                //model.productsubcategory = getSubCategory;

                //model.ProductSubCategorylist = getcategory.ProductSubCategory.ToList();

                //model.ProductChildCategoryform.ProductSubCategoryId = getSubCategory.Id;
                //if (ModelState.IsValid)
                //{
                    
                    var validate = db.ProductChildCategory.Where(x => x.Name == model.ProductChildCategoryform.Name && x.ProductSubCategoryId == model.ProductSubCategoryform.Id).ToList();
                    if (validate.Any())
                    {

                        TempData["messageType"] = "danger";
                        TempData["message"] = "The child Category " + model.ProductChildCategoryform.Name + " already exist for " + model.productsubcategory.Name + ". Please enter another name";
                        return RedirectToAction("ChildCategoryList", "ProductCategory", new { CategoryId = getcategory.Id, SubId = sub.Id, area = "Setup" });
                    }

                    ProductChildCategory add = new ProductChildCategory
                    {
                        ProductSubCategoryId = model.ProductChildCategoryform.ProductSubCategoryId,
                        Name = model.ProductChildCategoryform.Name,
                        IsDeleted = model.ProductChildCategoryform.IsDeleted,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.ProductChildCategory.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "The child category " + model.ProductChildCategoryform.Name + " has been added successfully.";
                    return RedirectToAction("ChildCategoryList", "ProductCategory", new { CategoryId = getcategory.Id, SubId = sub.Id, area = "Setup" });

                //}
                //TempData["message"] = ModelState.Values.SelectMany(v => v.Errors).ElementAt(0); 
                //TempData["messageType"] = "danger";
                //return RedirectToAction("ChildCategoryList", "ProductCategory", new { CategoryId = getcategory.Id, SubId = sub.Id, area = "Setup" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult EditChildCategory(ProductCategoryModel model)
        {
            try
            {
                var validate = db.ProductChildCategory.Where(x => x.Id == model.ProductChildCategoryform.Id).FirstOrDefault();
                var sub = db.ProductSubCategory.Where(x => x.Id == validate.ProductSubCategoryId).FirstOrDefault();
                var getcategory = db.ProductCategory.Where(x => x.Id == sub.ProductCategoryId).FirstOrDefault();

                  

                        validate.Name = model.ProductChildCategoryform.Name;
                        validate.IsDeleted = model.ProductChildCategoryform.IsDeleted;
                        validate.ModifiedBy = User.Identity.Name;
                        validate.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        TempData["message"] = "The child category " + model.ProductChildCategoryform.Name + " has been updated successfully.";
                        return RedirectToAction("ChildCategoryList", "ProductCategory", new { CategoryId = getcategory.Id, SubId=sub.Id, area = "Setup" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }



    }
}
