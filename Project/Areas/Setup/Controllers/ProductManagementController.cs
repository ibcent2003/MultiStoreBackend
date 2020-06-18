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
using System.Web.Security;
using Roles = System.Web.Security.Roles;

namespace Project.Areas.Setup.Controllers
{
    [Authorize(Roles = "Administrator, Store Admin, Product Manager")]
    public class ProductManagementController : Controller
    {
        //
        // GET: /Setup/ProductManagement/
        Backbone services = new Backbone();
        private PROEntities db = new PROEntities();
        private ProcessUtility util = new ProcessUtility();

        private Store storeDetails()
        {
            try
            {
                var username = Membership.GetUser().UserName;
                var user = db.Users.Where(x => x.UserName == username).FirstOrDefault();

                return user.Store.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public ActionResult Index(Guid Id)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }



                ProductManagementViewModel model = new ProductManagementViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {

                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.CategoryList = model.store.ProductCategory.ToList();
                TempData["message"] = "Add Product using the Category below. To Add more Category use the back button.";
                TempData["messageType"] = "warning";
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

        public ActionResult ProductList(Guid Id, int CategoryId)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                ProductManagementViewModel model = new ProductManagementViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.ProductList = model.store.StoreProduct.Where(x=>x.ProductCategoryId== CategoryId).ToList();
                model.documentPath = Properties.Settings.Default.ProductImagePath;
                var cate = model.store.ProductCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                model.category = cate;
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

        public ActionResult NewProduct(Guid Id, int CategoryId)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }

                ProductManagementViewModel model = new ProductManagementViewModel();                
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var cate = model.store.ProductCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                model.Productform = new ProductForm();
                model.Productform.CategoryId = CategoryId;
                model.BrandList = (from s in cate.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId== CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();              
                model.category = cate;
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
        public ActionResult GetChildId(int SubCategoryId)
        {
            var storeDetail = storeDetails();
            if (Roles.IsUserInRole("Administrator"))
            {
                List<IntegerSelectListItem> list = (from d in db.ProductChildCategory where d.ProductSubCategoryId == SubCategoryId orderby d.Name select new IntegerSelectListItem() { Text = d.Name, Value = d.Id }).ToList<IntegerSelectListItem>();
                return base.Json(list);
            }
            else
            {
                List<IntegerSelectListItem> list = (from d in storeDetail.ProductChildCategory where d.ProductSubCategoryId == SubCategoryId orderby d.Name select new IntegerSelectListItem() { Text = d.Name, Value = d.Id }).ToList<IntegerSelectListItem>();
                return base.Json(list);
            }



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewProduct(ProductManagementViewModel model)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }

                model.store = storeDetail;
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    
                    var validate = db.StoreProduct.Where(x => x.Name == model.Productform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Name " + model.Productform.Name + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";

                        var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                        model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.category = cate1;
                       return View(model);
                    }
                    string url = Properties.Settings.Default.ProductImagePath;
                    System.IO.Directory.CreateDirectory(url);

                    if(null == model.Productform.Photo1)
                    {
                        var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                        model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.category = cate1;

                        TempData["messageType"] = "danger";
                        TempData["message"] = "ERROR: You have to upload at least photo 1 to proceed.";
                        model.documentPath = Properties.Settings.Default.ProductImagePath;
                        return View(model);

                    }

                    #region upload Photo1

                    int max_upload = 5242880;

                  
                    List<DocumentInfo> uploadedPhoto1 = new List<DocumentInfo>();

                    CodeGenerator CodePhoto1 = new CodeGenerator();
                    string EncKey1 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<DocumentFormat> Photo1types = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                    List<string> supportedPhoto1 = new List<string>();
                    foreach (var item in Photo1types)
                    {
                        supportedPhoto1.Add(item.Extension);
                    }
                    var filePhoto1 = System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                    if (!supportedPhoto1.Contains(filePhoto1))
                    {
                        var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                        model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.category = cate1;

                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto1) + " are supported for Photo 1";
                        model.documentPath = Properties.Settings.Default.ProductImagePath;
                        return View(model);

                    }
                    else if (model.Productform.Photo1.ContentLength > max_upload)
                    {

                        var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                        model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        model.category = cate1;

                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Photo 1 uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.ProductImagePath;
                        return View(model);
                    }

                    //store product
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                    model.Productform.Photo1.SaveAs(url + pName);

                    #endregion

                    if(model.Productform.Photo2 != null && model.Productform.Photo2.ContentLength > 0 )
                    {
                        #region upload Photo2

                        int max_upload2 = 5242880;


                       

                        CodeGenerator CodePhoto2 = new CodeGenerator();
                        string EncKey2 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo2types = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                        List<string> supportedPhoto2 = new List<string>();
                        foreach (var item in Photo2types)
                        {
                            supportedPhoto2.Add(item.Extension);
                        }
                        var filePhoto2 = System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        if (!supportedPhoto2.Contains(filePhoto2))
                        {
                            var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                            model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.category = cate1;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto2) + " are supported for Photo 2";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }
                        else if (model.Productform.Photo2.ContentLength > max_upload2)
                        {
                            var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                            model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.category = cate1;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 2 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }

                        //store product
                        int pp2 = 0;
                        string pName2;
                        pName2 = EncKey2 + pp2.ToString() + System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        model.Productform.Photo2.SaveAs(url + pName2);
                        model.p2 = pName2;

                        #endregion
                    }

                    if (model.Productform.Photo3 != null && model.Productform.Photo3.ContentLength > 0)
                    {
                        #region upload Photo3

                        int max_upload3 = 5242880;


                      

                        CodeGenerator CodePhoto3 = new CodeGenerator();
                        string EncKey3 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo3types = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                        List<string> supportedPhoto3 = new List<string>();
                        foreach (var item in Photo3types)
                        {
                            supportedPhoto3.Add(item.Extension);
                        }
                        var filePhoto3 = System.IO.Path.GetExtension(model.Productform.Photo3.FileName);
                        if (!supportedPhoto3.Contains(filePhoto3))
                        {
                            var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                            model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.category = cate1;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto3) + " are supported for Photo 3";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }
                        else if (model.Productform.Photo3.ContentLength > max_upload3)
                        {
                            var cate1 = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                            model.BrandList = (from s in cate1.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.category = cate1;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 3 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }

                        //store product
                        int pp3 = 0;
                        string pName3;
                        pName3 = EncKey3 + pp3.ToString() + System.IO.Path.GetExtension(model.Productform.Photo3.FileName);
                        model.Productform.Photo3.SaveAs(url + pName3);
                        model.p3 = pName3;

                        #endregion
                    }

                    StoreProduct add = new StoreProduct
                    {
                        Name = model.Productform.Name,
                        BrandId = model.Productform.BrandId,
                        DiscountPrice = model.Productform.DiscountPrice,
                        AcutalPrice = model.Productform.AcutalPrice,
                        Quantity = model.Productform.Quantity,
                        ReorderLevel = model.Productform.ReorderLevel,
                        ProductCategoryId = model.Productform.CategoryId,
                        ProductSubCategoryId = model.Productform.ProductSubCategoryId,
                        ProductChildCategoryId = model.Productform.ProductChildCategoryId,
                        Photo1 = pName,
                        Photo2 = model.p2,
                        Photo3 = model.p3,
                        HasColor = model.Productform.HasColor,
                        HasSize = model.Productform.HasSize,
                        Description = model.Productform.Description,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.Productform.IsDeleted
                    };
                    db.StoreProduct.AddObject(add);
                    model.store.StoreProduct.Add(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.Productform.Name + " has been added successful.";
                    return RedirectToAction("ProductList", "ProductManagement", new {Id=model.store.ProcessInstaceId, CategoryId=model.Productform.CategoryId, area = "Setup" });

                }
               var cate = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                model.BrandList = (from s in cate.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.category = cate;
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter all fields with red sign.";
                TempData["messageType"] = "danger";              
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



        public ActionResult EditProduct(Guid Id, int PId)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }

                ProductManagementViewModel model = new ProductManagementViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var product = model.store.StoreProduct.Where(x => x.Id == PId).FirstOrDefault();
                var cate = model.store.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                model.Productform = new ProductForm();
                model.Productform.Name = product.Name;
                model.Productform.BrandId = product.BrandId;
                model.Productform.DiscountPrice = product.DiscountPrice;
                model.Productform.AcutalPrice = product.AcutalPrice;
                model.Productform.Quantity = product.Quantity;
                model.Productform.ReorderLevel = product.ReorderLevel;
                model.Productform.CategoryId = product.ProductCategoryId;
                model.Productform.ProductSubCategoryId = product.ProductSubCategoryId;
                model.Productform.ProductChildCategoryId = product.ProductChildCategoryId;
                model.p1 = product.Photo1;
                if(product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }
                if(product.Photo3 != null)
                {
                    model.p3 = product.Photo3;
                }
                model.Productform.HasColor = product.HasColor;
                model.Productform.HasSize = product.HasSize;
                model.Productform.Id = PId;
                model.Productform.Description = product.Description;
                model.Productform.IsDeleted = product.IsDeleted;
                model.category = cate;
                model.BrandList = (from s in cate.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == product.ProductCategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.documentPath = Properties.Settings.Default.ProductImagePath;

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
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditProduct(ProductManagementViewModel model)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                model.store = storeDetail;
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == model.Productform.Id).FirstOrDefault();
                if(product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == model.Productform.CategoryId).FirstOrDefault();
                if (cate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                if (ModelState.IsValid)
                {
                    model.BrandList = (from s in cate.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                    model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                    model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                    model.category = cate;
                    string url = Properties.Settings.Default.ProductImagePath;
                    System.IO.Directory.CreateDirectory(url);
               
                    if (model.Productform.Photo1 != null && model.Productform.Photo1.ContentLength > 0)
                    {                       
                        #region upload Photo1

                        int max_upload = 5242880;
                       
                        CodeGenerator CodePhoto1 = new CodeGenerator();
                        string EncKey1 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo1types = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                        List<string> supportedPhoto1 = new List<string>();
                        foreach (var item in Photo1types)
                        {
                            supportedPhoto1.Add(item.Extension);
                        }
                        var filePhoto1 = System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                        if (!supportedPhoto1.Contains(filePhoto1))
                        {

                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            if (product.Photo3 != null)
                            {
                                model.p3 = product.Photo3;
                            }
                            model.documentPath = Properties.Settings.Default.ProductImagePath;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto1) + " are supported for Photo 1";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);

                        }
                        else if (model.Productform.Photo1.ContentLength > max_upload)
                        {

                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            if (product.Photo3 != null)
                            {
                                model.p3 = product.Photo3;
                            }
                            model.documentPath = Properties.Settings.Default.ProductImagePath;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 1 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }

                        if (product.Photo1 != null)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + product.Photo1);
                            fi.Delete();
                        }

                        //store product
                        int pp = 0;
                        string pName;
                        pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                        model.Productform.Photo1.SaveAs(url + pName);
                        product.Photo1 = pName;

                        #endregion
                    }

                    if (model.Productform.Photo2 != null && model.Productform.Photo2.ContentLength > 0)
                    {                       
                        #region upload Photo2

                        int max_upload2 = 5242880;

                        CodeGenerator CodePhoto2 = new CodeGenerator();
                        string EncKey2 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo2types = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                        List<string> supportedPhoto2 = new List<string>();
                        foreach (var item in Photo2types)
                        {
                            supportedPhoto2.Add(item.Extension);
                        }
                        var filePhoto2 = System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        if (!supportedPhoto2.Contains(filePhoto2))
                        {
                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            if (product.Photo3 != null)
                            {
                                model.p3 = product.Photo3;
                            }
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto2) + " are supported for Photo 2";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }
                        else if (model.Productform.Photo2.ContentLength > max_upload2)
                        {

                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            if (product.Photo3 != null)
                            {
                                model.p3 = product.Photo3;
                            }
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 2 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }

                        if (product.Photo2 != null)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + product.Photo2);
                            fi.Delete();
                        }

                        //store product
                        int pp2 = 0;
                        string pName2;
                        pName2 = EncKey2 + pp2.ToString() + System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        model.Productform.Photo2.SaveAs(url + pName2);
                        model.p2 = pName2;
                        product.Photo2 = pName2;

                        #endregion
                    }

                    if (model.Productform.Photo3 != null && model.Productform.Photo3.ContentLength > 0)
                    {                      
                        #region upload Photo3

                        int max_upload3 = 5242880;

                        CodeGenerator CodePhoto3 = new CodeGenerator();
                        string EncKey3 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo3types = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                        List<string> supportedPhoto3 = new List<string>();
                        foreach (var item in Photo3types)
                        {
                            supportedPhoto3.Add(item.Extension);
                        }
                        var filePhoto3 = System.IO.Path.GetExtension(model.Productform.Photo3.FileName);
                        if (!supportedPhoto3.Contains(filePhoto3))
                        {
                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            if (product.Photo3 != null)
                            {
                                model.p3 = product.Photo3;
                            }
                            model.documentPath = Properties.Settings.Default.ProductImagePath;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto3) + " are supported for Photo 3";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }
                        else if (model.Productform.Photo3.ContentLength > max_upload3)
                        {
                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            if (product.Photo3 != null)
                            {
                                model.p3 = product.Photo3;
                            }
                            model.documentPath = Properties.Settings.Default.ProductImagePath;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 3 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.ProductImagePath;
                            return View(model);
                        }

                        if (product.Photo3 != null)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + product.Photo3);
                            fi.Delete();
                        }

                        //store product
                        int pp3 = 0;
                        string pName3;
                        pName3 = EncKey3 + pp3.ToString() + System.IO.Path.GetExtension(model.Productform.Photo3.FileName);
                        model.Productform.Photo3.SaveAs(url + pName3);
                        model.p3 = pName3;
                        product.Photo3 = pName3;

                        #endregion
                    }
                    product.Name = model.Productform.Name;
                    product.BrandId = model.Productform.BrandId;
                    product.DiscountPrice = model.Productform.DiscountPrice;
                    product.AcutalPrice = model.Productform.AcutalPrice;
                    product.ReorderLevel = model.Productform.ReorderLevel;
                    product.ProductSubCategoryId = model.Productform.ProductSubCategoryId;
                    product.ProductChildCategoryId = model.Productform.ProductChildCategoryId;
                    product.HasColor = model.Productform.HasColor;
                    product.HasSize = model.Productform.HasSize;
                    product.Description = model.Productform.Description;
                    product.IsDeleted = model.Productform.IsDeleted;
                    product.ModifiedBy = User.Identity.Name;
                    product.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "" + model.Productform.Name + " has been updated successful.";
                    return RedirectToAction("ProductList", "ProductManagement", new { Id = model.store.ProcessInstaceId, CategoryId = model.Productform.CategoryId, area = "Setup" });
                    }

                TempData["message"] = "Ops! Something went wrong. Please make sure you enter all fields with red sign.";
                TempData["messageType"] = "danger";

                model.BrandList = (from s in cate.ProductBrand where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.SubcategoryList = (from s in model.store.ProductSubCategory where s.IsDeleted == false && s.ProductCategoryId == model.Productform.CategoryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.ChildcategoryList = (from s in model.store.ProductChildCategory where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.category = cate;
                model.p1 = product.Photo1;
                if (product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }
                if (product.Photo3 != null)
                {
                    model.p3 = product.Photo3;
                }
                model.documentPath = Properties.Settings.Default.ProductImagePath;
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


        public ActionResult ReorderProduct(Guid Id, int PId)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }

                ProductManagementViewModel model = new ProductManagementViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                var product = model.store.StoreProduct.Where(x => x.Id == PId).FirstOrDefault();
                var cate = model.store.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                model.Productform = new ProductForm();
                //model.Productform.Name = product.Name;
                //model.Productform.Quantity = product.Quantity;
                model.p1 = product.Photo1;
                if (product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }
                if (product.Photo3 != null)
                {
                    model.p3 = product.Photo3;
                }               
                model.Productform.Id = PId;              
                model.category = cate;
                model.product = product;             
                model.documentPath = Properties.Settings.Default.ProductImagePath;

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
        public ActionResult ReorderProduct(ProductManagementViewModel model)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                model.store = storeDetail;
                if (model.store == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Store Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == model.Productform.Id).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                if (cate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product category Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }

                if(model.Productform.NewQuantity==0)
                {
                    TempData["message"] = "ERROR: Please enter the New Quantity you are redordering.";
                    TempData["messageType"] = "danger";
                   
                    model.category = cate;
                    model.p1 = product.Photo1;
                    if (product.Photo2 != null)
                    {
                        model.p2 = product.Photo2;
                    }
                    if (product.Photo3 != null)
                    {
                        model.p3 = product.Photo3;
                    }
                    model.documentPath = Properties.Settings.Default.ProductImagePath;
                    model.product = product;
                    return View(model);

                }

                model.category = cate;
                model.p1 = product.Photo1;
                if (product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }
                if (product.Photo3 != null)
                {
                    model.p3 = product.Photo3;
                }
                int old_qty = product.Quantity;
                int new_qty = model.Productform.NewQuantity;
                int total_qty = old_qty + new_qty;
                product.Quantity = total_qty;
                db.SaveChanges();
              
                model.documentPath = Properties.Settings.Default.ProductImagePath;
                model.product = product;
                TempData["message"] = "" + product.Name + " has been Re-Order successful.";
                return RedirectToAction("ProductList", "ProductManagement", new { Id = model.store.ProcessInstaceId, CategoryId = product.ProductCategoryId, area = "Setup" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult ProductColorList(Guid Id, int PId)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                ProductManagementViewModel model = new ProductManagementViewModel();
                model.store = storeDetail;
                if (model.store == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Store Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == PId).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                if (cate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product category Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                
                var GetUserRole = product.ProductColor.Select(x => x.Id).ToList();
                var GetAllRole = (from r in db.ProductColor select r.Id).ToList().Except(GetUserRole);

                model.AllSelectColor = product.ProductColor.OrderBy(x => x.Name).ToList();

                model.AllColor = (from r in db.ProductColor
                                  orderby r.Name
                                  where GetAllRole.Contains(r.Id) && r.IsDeleted == false
                                 select r).ToList();
                model.product = product;
                model.ProductList = storeDetail.StoreProduct.Where(x => x.Id == PId).ToList();
                model.documentPath = Properties.Settings.Default.ProductImagePath;
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
        public ActionResult Grant(ProductManagementViewModel model)
        {
            try
            {

                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                model.store = storeDetail;
                if (model.store == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Store Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == model.product.Id).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                if (cate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product category Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                             
                var GetUserRole = product.ProductColor.Select(x => x.Id).ToList();
                var GetAllRole = (from r in db.ProductColor where r.IsDeleted==false select r.Id).ToList().Except(GetUserRole);

                model.AllColor = (from r in db.ProductColor where r.IsDeleted==false
                                 orderby r.Id
                                 select r).ToList();
                if (model.ColorUsed == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Color to Add";
                    return RedirectToAction("ProductColorList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId=product.Id });
                }
                string roletext = "";
                model.ColorUsedTex = new List<string> { };
                foreach (string roleId in model.ColorUsed)
                {
                   // int RId = int.Parse(roleId);
                    roletext = (from p in db.ProductColor where p.Name == roleId select p.Name).FirstOrDefault();
                    model.ColorUsedTex.Add(roletext);
                }
                foreach (string p in model.ColorUsed.ToList())
                {
                    //var RsId = int.Parse(p.ToString());
                    var RoleN = (from r in db.ProductColor where r.Name == p select r).FirstOrDefault();
                    if (model.ColorUsed != null)
                    {
                        product.ProductColor.Add(RoleN);
                        db.SaveChanges();
                    }
                }
                TempData["message"] = "The Color(s) has been GRANTED successfully";
                return RedirectToAction("ProductColorList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId = product.Id });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult RevokeColor(ProductManagementViewModel model)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                model.store = storeDetail;
                if (model.store == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Store Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == model.product.Id).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                if (cate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product category Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
              
                var GetProductColor = product.ProductColor.Select(x => x.Id).ToList();
                var GetAllColor = (from r in db.ProductColor select r.Id).ToList().Except(GetProductColor);
                model.AllSelectColor = product.ProductColor.ToList();
                if (model.GrantedColorUsed == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Color to Remove";
                    return RedirectToAction("ProductColorList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId = product.Id });
                }

                string roletext = "";
                model.ColorUsedTex = new List<string> { };

                foreach (string pid in model.GrantedColorUsed)
                {
                    var RId = int.Parse(pid.ToString());
                    roletext = (from p in db.ProductColor where p.Id == RId select p.Name).FirstOrDefault();
                    model.ColorUsedTex.Add(roletext);

                    var existingRole = product.ProductColor.Where(x => x.Id == RId).ToList();
                    if (existingRole != null)
                    {
                        foreach (var role in existingRole)
                        {
                            var RsId = int.Parse(role.Id.ToString());
                            var RoleN = (from r in db.ProductColor where r.Id == RsId select r).FirstOrDefault();
                            if (RoleN != null)
                            {
                                product.ProductColor.Remove(RoleN);
                                db.SaveChanges();
                            }
                        }
                    }
                }

                TempData["message"] = "The Color(s) has been REMOVE successfully from the product.";
                return RedirectToAction("ProductColorList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId = product.Id });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult ProductSizeList(Guid Id, int PId, int SizeTypeId=0)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                ProductManagementViewModel model = new ProductManagementViewModel();
                model.store = storeDetail;
                if (model.store == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Store Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == PId).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                if (cate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product category Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }

                model.AllSizeType = (from s in db.SizeType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.product = product;
                model.ProductList = storeDetail.StoreProduct.Where(x => x.Id == PId).ToList();
                model.documentPath = Properties.Settings.Default.ProductImagePath;

                model.SizeTypeId = SizeTypeId;
                var sizeProduct = db.SizeType.Where(x => x.Id == SizeTypeId).FirstOrDefault();
                var GetUserRole = product.Size.Select(x => x.Id).ToList();
                var GetAllRole = (from r in db.Size select r.Id).ToList().Except(GetUserRole);

                model.AllSelectedSize = product.Size.OrderBy(x => x.Name).ToList();

                model.AllSize = (from r in db.Size where r.SizeTypeId==model.SizeTypeId
                                 orderby r.Id
                                 where GetAllRole.Contains(r.Id)
                                 select r).ToList();


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
        public ActionResult GrantSize(ProductManagementViewModel model)
        {
            try
            {

                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                model.store = storeDetail;
                if (model.store == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Store Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == model.product.Id).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                if (cate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product category Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }

                var GetUserRole = product.Size.Select(x => x.Id).ToList();
                var GetAllRole = (from r in db.Size where r.IsDeleted == false select r.Id).ToList().Except(GetUserRole);

                model.AllSize = (from r in db.Size
                                  where r.IsDeleted == false where r.SizeTypeId==model.SizeTypeId
                                  orderby r.Id
                                  select r).ToList();
                if (model.SizeUsed == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Color to Add";
                    return RedirectToAction("ProductSizeList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId = product.Id, SizeTypeId=model.SizeTypeId });
                }
                string roletext = "";
                model.sizeUsedTex = new List<string> { };
                foreach (string roleId in model.SizeUsed)
                {
                    // int RId = int.Parse(roleId);
                    roletext = (from p in db.Size where p.Name == roleId select p.Name).FirstOrDefault();
                    model.sizeUsedTex.Add(roletext);
                }
                foreach (string p in model.SizeUsed.ToList())
                {
                    //var RsId = int.Parse(p.ToString());
                    var RoleN = (from r in db.Size where r.Name == p select r).FirstOrDefault();
                    if (model.SizeUsed != null)
                    {
                        product.Size.Add(RoleN);
                        db.SaveChanges();
                    }
                }
                TempData["message"] = "The Color(s) has been GRANTED successfully";
                return RedirectToAction("ProductSizeList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId = product.Id, SizeTypeId = model.SizeTypeId });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult RevokeSize(ProductManagementViewModel model)
        {
            try
            {
                var storeDetail = storeDetails();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (storeDetail == null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unauthorised Access"));
                        TempData["message"] = "Unauthorised Access";
                        return RedirectToAction("Index", "Store", new { area = "Setup" });
                    }
                }
                model.store = storeDetail;
                if (model.store == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Store Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }

                var product = storeDetail.StoreProduct.Where(x => x.Id == model.product.Id).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var cate = db.ProductCategory.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();
                if (cate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product category Object is empty. Unlikely error."));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }

                var GetProductColor = product.Size.Select(x => x.Id).ToList();
                var GetAllColor = (from r in db.Size select r.Id).ToList().Except(GetProductColor);
                model.AllSelectedSize = product.Size.ToList();
                if (model.GrantedSizeUsed == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Color to Remove";
                    return RedirectToAction("ProductSizeList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId = product.Id, SizeTypeId = model.SizeTypeId });
                }

                string roletext = "";
                model.sizeUsedTex = new List<string> { };

                foreach (string pid in model.GrantedSizeUsed)
                {
                    var RId = int.Parse(pid.ToString());
                    roletext = (from p in db.Size where p.Id == RId select p.Name).FirstOrDefault();
                    model.sizeUsedTex.Add(roletext);

                    var existingRole = product.Size.Where(x => x.Id == RId).ToList();
                    if (existingRole != null)
                    {
                        foreach (var role in existingRole)
                        {
                            var RsId = int.Parse(role.Id.ToString());
                            var RoleN = (from r in db.Size where r.Id == RsId select r).FirstOrDefault();
                            if (RoleN != null)
                            {
                                product.Size.Remove(RoleN);
                                db.SaveChanges();
                            }
                        }
                    }
                }

                TempData["message"] = "The Size(s) has been REMOVE successfully from the product.";
                return RedirectToAction("ProductSizeList", "ProductManagement", new { area = "Setup", Id = model.store.ProcessInstaceId, PId = product.Id, SizeTypeId = model.SizeTypeId });
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
