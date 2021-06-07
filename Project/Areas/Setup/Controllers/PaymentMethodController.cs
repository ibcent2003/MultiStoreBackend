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
    public class PaymentMethodController : Controller
    {

       
        private PROEntities db = new PROEntities();
        private ProcessUtility util = new ProcessUtility();
        public ActionResult Index()
        {
            try
            {
                PaymentMethodViewModel model = new PaymentMethodViewModel();
                model.paymentMethods = db.PaymentMethod.ToList();
                model.documentPath = Properties.Settings.Default.PaymentLogo;
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

        public ActionResult NewPaymentLogo()
        {
            try
            {
                PaymentMethodViewModel model = new PaymentMethodViewModel();
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
        public ActionResult NewPaymentLogo(PaymentMethodViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check duplicate store name
                    var validate = db.PaymentMethod.Where(x => x.Name == model.PaymentMethodform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Payment Method Name " + model.PaymentMethodform.Name + " already exist. Please enter another name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    string url = Properties.Settings.Default.PaymentLogo;
                    System.IO.Directory.CreateDirectory(url);


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
                    var filePassport = System.IO.Path.GetExtension(model.PaymentMethodform.LogoPath.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                        model.documentPath = Properties.Settings.Default.PaymentLogo;
                        return View(model);

                    }
                    else if (model.PaymentMethodform.LogoPath.ContentLength > max_upload)
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);
                    }

                    //store passport
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.PaymentMethodform.LogoPath.FileName);
                    model.PaymentMethodform.LogoPath.SaveAs(url + pName);

                    #endregion

                    PaymentMethod addnew = new PaymentMethod()
                    {
                        Name = model.PaymentMethodform.Name,
                        LogoPath = pName,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false,
                       
                    };
                    db.PaymentMethod.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "The Store " + model.PaymentMethodform.Name + " has been added successfully.";
                    return RedirectToAction("Index", "PaymentMethod", new { area = "Setup" });

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


        public ActionResult EditPaymentLogo(int Id)
        {
            try
            {
                PaymentMethodViewModel model = new PaymentMethodViewModel();
                var GetPaymentMethod = db.PaymentMethod.Where(x => x.Id == Id).FirstOrDefault();
                if (GetPaymentMethod == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.PaymentMethodform = new  PaymentMethodForm();
                model.PaymentMethodform.Name = GetPaymentMethod.Name;
                model.PaymentMethodform.Id = GetPaymentMethod.Id;
                model.documentValue = GetPaymentMethod.LogoPath;
                model.documentPath = Properties.Settings.Default.PaymentLogo;               
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
        public ActionResult EditPaymentLogo(PaymentMethodViewModel model)
        {
            try
            {
                var GetPaymentMethod = db.PaymentMethod.Where(x => x.Id == model.PaymentMethodform.Id).FirstOrDefault();
                if (GetPaymentMethod == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                string url = Properties.Settings.Default.PaymentLogo;
                System.IO.Directory.CreateDirectory(url);

                if (model.PaymentMethodform.LogoPath != null && model.PaymentMethodform.LogoPath.ContentLength > 0)
                {

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
                    var filePassport = System.IO.Path.GetExtension(model.PaymentMethodform.LogoPath.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                        model.documentPath = Properties.Settings.Default.PaymentLogo;
                        model.documentValue = GetPaymentMethod.LogoPath;
                        return View(model);
                    }
                    else if (model.PaymentMethodform.LogoPath.ContentLength > max_upload)
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentValue = GetPaymentMethod.LogoPath;
                        model.documentPath = Properties.Settings.Default.PaymentLogo;
                        return View(model);
                    }


                    //delete passport
                    if (GetPaymentMethod.LogoPath != null)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(url + GetPaymentMethod.LogoPath);
                        fi.Delete();
                    }
                    //store logo
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.PaymentMethodform.LogoPath.FileName);
                    model.PaymentMethodform.LogoPath.SaveAs(url + pName);

                    #endregion
                    GetPaymentMethod.LogoPath = pName;
                }
                GetPaymentMethod.Name = model.PaymentMethodform.Name;
                GetPaymentMethod.ModifiedBy = User.Identity.Name;
                GetPaymentMethod.ModifiedDate = DateTime.Now;
                GetPaymentMethod.IsDeleted = model.PaymentMethodform.IsDeleted;
                db.SaveChanges();
                TempData["message"] = "The Payment Method " + model.PaymentMethodform.Name + " has been updated successfully.";
                return RedirectToAction("Index", "PaymentMethod", new { area = "Setup" });
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
