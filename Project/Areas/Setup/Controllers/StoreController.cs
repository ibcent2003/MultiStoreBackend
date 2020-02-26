﻿using Project.Areas.Setup.Models;
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
    [Authorize]
    public class StoreController : Controller
    {
        Backbone services = new Backbone();
        private PROEntities db = new PROEntities();
        private ProcessUtility util = new ProcessUtility();

        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                model.storelist= Backbone.GetStoreList(db);
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

        public ActionResult NewStore()
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
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
        public ActionResult NewStore(StoreManagementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check duplicate store name
                    var validate = db.Store.Where(x => x.Name == model.storeform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Store Name "+model.storeform.Name+" already exist. Please enter another name";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    string url = Properties.Settings.Default.DocumentPath;
                    System.IO.Directory.CreateDirectory(url);


                    #region upload passport

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
                    var filePassport = System.IO.Path.GetExtension(model.storeform.Logo.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for passport photograph";
                       
                        string pathurl = "/../Content/CompanyDocuments/";
                        string downloadURL = "/../Content/CompanyDocuments/";
                       // model.DownloadPath = downloadURL;
                       // model.DocumentUploadPath = pathurl;
                      //  model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                        return View(model);

                    }
                    else if (model.storeform.Logo.ContentLength > max_upload)
                    {

                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The passport photograph uploaded is larger than the 5MB upload limit";
                        
                        string pathurl = "/../Content/CompanyDocuments/";
                        string downloadURL = "/../Content/CompanyDocuments/";
                       // model.DownloadPath = downloadURL;
                       // model.DocumentUploadPath = pathurl;
                       // model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                        return View(model);
                    }

                    //store passport
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.storeform.Logo.FileName);
                    model.storeform.Logo.SaveAs(url + pName);

                    #endregion

                    Store addnew = new Store()
                    {
                        Name = model.storeform.Name,
                        Logo = pName,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false
                        
                    };
                    db.Store.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "The Store "+model.storeform.Name+" has been added successfully.";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

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
