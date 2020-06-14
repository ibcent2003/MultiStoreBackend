using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
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
                model.ProductList = model.store.StoreProduct.Where(x=>x.ProductCategoryId==CategoryId).ToList();
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

        public ActionResult NewProductColor()
        {
            try
            {
                ProductColorViewModel model = new ProductColorViewModel();
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
        public ActionResult NewProductColor(ProductColorViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    var validate = db.ProductColor.Where(x => x.Name == model.Productcolorform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Color Name " + model.Productcolorform.Name + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    ProductColor add = new ProductColor
                    {
                        Name = model.Productcolorform.Name,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.Productcolorform.IsDeleted
                    };
                    db.ProductColor.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.Productcolorform.Name + " has been added successful.";
                    return RedirectToAction("Index");

                }
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter the Color Name.";
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



        public ActionResult EditProductColor(int Id)
        {
            try
            {
                var GetColor = db.ProductColor.Where(x => x.Id == Id).FirstOrDefault();
                if (GetColor == null)
                {
                    
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                ProductColorViewModel model = new ProductColorViewModel();
                model.Productcolorform = new  ProductColorForm();
                model.Productcolorform.Id = GetColor.Id;
                model.Productcolorform.Name = GetColor.Name;
                model.Productcolorform.IsDeleted = GetColor.IsDeleted;
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
        public ActionResult EditProductColor(ProductColorViewModel model)
        {
            try
            {
                var getColor = db.ProductColor.Where(x => x.Id == model.Productcolorform.Id).FirstOrDefault();
                if(getColor==null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                getColor.Name = model.Productcolorform.Name;
                getColor.IsDeleted = model.Productcolorform.IsDeleted;
                getColor.ModifiedBy = User.Identity.Name;
                getColor.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "The Color name " + model.Productcolorform.Name + " has been updated successfully.";                
                return RedirectToAction("Index");
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

    }
}
