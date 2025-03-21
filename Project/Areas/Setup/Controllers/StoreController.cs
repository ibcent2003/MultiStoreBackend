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
                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
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
                    var validate = db.Store.Where(x => x.Name == model.storeform.URL).ToList();
                    if (validate.Any())
                    {
                        model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        TempData["message"] = "The Store Name "+model.storeform.URL+" already exist. Please enter another name";
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
                    var filePassport = System.IO.Path.GetExtension(model.storeform.Logo.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";                                              
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);

                    }
                    else if (model.storeform.Logo.ContentLength > max_upload)
                    {
                        model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
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
                        IsDeleted = false,
                        OwnProcurement = model.storeform.OwnProcurement,
                        ProcessInstaceId = Guid.NewGuid(),
                        URL = model.storeform.URL,
                        WorkFlowId = Properties.Settings.Default.StoreRegistrationWorkFlowId,
                        Status = "draft",
                        OwnedBy = "Administrator",
                        CountryId = model.storeform.CountryId,
                        ThemesId = model.storeform.ThemesId,
                        Description = model.storeform.Description
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

        public ActionResult EditStore(int Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var GetStore = db.Store.Where(x => x.Id == Id).FirstOrDefault();
                if(GetStore== null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.storeform = new StoreForm();
                model.storeform.Name = GetStore.Name;
                model.storeform.Id = GetStore.Id;
                model.storeform.URL = GetStore.URL;
                model.storeform.Description = GetStore.Description;
                model.storeform.OwnProcurement = GetStore.OwnProcurement;
                model.documentValue = GetStore.Logo;
                model.documentPath = Properties.Settings.Default.DocumentPath;
                model.storeform.ThemesId = int.Parse(GetStore.ThemesId.ToString());
                model.storeform.IsDeleted = GetStore.IsDeleted;
                model.store = GetStore;
                
                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
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
        public ActionResult EditStore(StoreManagementViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.Id == model.storeform.Id).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                string url = Properties.Settings.Default.DocumentPath;
                System.IO.Directory.CreateDirectory(url);

                if (model.storeform.Logo != null && model.storeform.Logo.ContentLength > 0)
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
                    var filePassport = System.IO.Path.GetExtension(model.storeform.Logo.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        return View(model);
                    }
                    else if (model.storeform.Logo.ContentLength > max_upload)
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        return View(model);
                    }
                   

                    //delete passport
                    if (GetStore.Logo != null)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(url + GetStore.Logo);
                        fi.Delete();
                    }
                    //store logo
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.storeform.Logo.FileName);
                    model.storeform.Logo.SaveAs(url + pName);

                    #endregion

                GetStore.Logo = pName;
                }
                GetStore.Name = model.storeform.Name;
                GetStore.URL = model.storeform.URL;
                GetStore.ModifiedBy = User.Identity.Name;
                GetStore.ModifiedDate = DateTime.Now;
                GetStore.OwnProcurement = model.storeform.OwnProcurement;
                GetStore.Description = model.storeform.Description;
                GetStore.ThemesId = model.storeform.ThemesId;
                GetStore.IsDeleted = model.storeform.IsDeleted;
                db.SaveChanges();
                TempData["message"] = "The Store " + model.storeform.Name + " has been updated successfully.";
                return RedirectToAction("StoreDashboard", "Dashboard", new {Id=GetStore.ProcessInstaceId, area = "Admin" });
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

        public ActionResult SliderList(Guid Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                model.StoreSliderList = Backbone.GetStoreSlider(db, Id);
                model.documentPath = Properties.Settings.Default.SliderPath;
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                model.store = GetStore;
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

        public ActionResult NewSlider(Guid Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                model.store = GetStore;
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
        public ActionResult NewSlider(StoreManagementViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == model.store.ProcessInstaceId).FirstOrDefault();

                if (model.StoreSliderform.SliderPhoto == null)
                {
                    TempData["message"] = "ERROR: Please use the browse button to select photo from your computer.";
                    TempData["messageType"] = "danger";
                    return View(model);
                }

                if (ModelState.IsValid)
                {
                    string url = Properties.Settings.Default.SliderPath;
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
                    var filePassport = System.IO.Path.GetExtension(model.StoreSliderform.SliderPhoto.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                        model.documentPath = Properties.Settings.Default.SliderPath;
                        return View(model);

                    }
                    else if (model.StoreSliderform.SliderPhoto.ContentLength > max_upload)
                    {
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.SliderPath;
                        return View(model);
                    }

                    //store passport
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.StoreSliderform.SliderPhoto.FileName);
                    model.StoreSliderform.SliderPhoto.SaveAs(url + pName);

                    #endregion

                    StoreSlider addnew = new StoreSlider()
                    {
                        CaptionOne = model.StoreSliderform.CaptionOne,
                        CaptionTwo = model.StoreSliderform.CaptionTwo,
                        ButtonText = model.StoreSliderform.ButtonText,
                        Description = model.StoreSliderform.Description,
                        SliderPhoto = pName,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false,
                        StoreId = GetStore.Id,                        
                    };
                    db.StoreSlider.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "The Slider has been added successfully.";
                    return RedirectToAction("SliderList", "Store", new { area = "Setup", Id=model.store.ProcessInstaceId });

                }
                TempData["message"] = "ERROR: Please use the browse button to select photo from your computer.";
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

        public ActionResult EditSlider(Guid Id, int SliderId)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                var getSlider = db.StoreSlider.Where(x => x.Id == SliderId && x.StoreId == GetStore.Id).FirstOrDefault();
                model.StoreSliderform = new  StoreSliderForm();
                model.StoreSliderform.CaptionOne = getSlider.CaptionOne;
                model.StoreSliderform.CaptionTwo = getSlider.CaptionTwo;
                model.StoreSliderform.ButtonText = getSlider.ButtonText;
                model.StoreSliderform.Description = getSlider.Description;
                model.StoreSliderform.Id = getSlider.Id;
                model.documentValue = getSlider.SliderPhoto;
                model.StoreSliderform.StoreId = GetStore.Id;
                model.documentPath = Properties.Settings.Default.SliderPath;
                model.store = GetStore;

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
        public ActionResult EditSlider(StoreManagementViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.Id == model.StoreSliderform.StoreId).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                string url = Properties.Settings.Default.SliderPath;
                System.IO.Directory.CreateDirectory(url);
                var getSlider = db.StoreSlider.Where(x => x.Id == model.StoreSliderform.Id && x.StoreId == GetStore.Id).FirstOrDefault();
                if (model.StoreSliderform.SliderPhoto != null && model.StoreSliderform.SliderPhoto.ContentLength > 0)
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
                    var filePassport = System.IO.Path.GetExtension(model.StoreSliderform.SliderPhoto.FileName);
                    if (!supportedPassport.Contains(filePassport))
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for Slider photo";
                        model.documentPath = Properties.Settings.Default.SliderPath;
                        return View(model);
                    }
                    else if (model.StoreSliderform.SliderPhoto.ContentLength > max_upload)
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.SliderPath;
                        return View(model);
                    }


                    if (!new string[] { "slider1.jpg", "slider1.jpg", "slider1.jpg" }.Any(s => getSlider.SliderPhoto.Contains(s)))
                    {
                        //delete passport
                        if (getSlider.SliderPhoto != null)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + getSlider.SliderPhoto);
                            fi.Delete();
                        }
                    }
                      
                    //store logo
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.StoreSliderform.SliderPhoto.FileName);
                    model.StoreSliderform.SliderPhoto.SaveAs(url + pName);

                    #endregion

                    getSlider.SliderPhoto = pName;
                }
                getSlider.CaptionOne = model.StoreSliderform.CaptionOne;
                getSlider.CaptionTwo = model.StoreSliderform.CaptionTwo;
                getSlider.ButtonText = model.StoreSliderform.ButtonText;
                getSlider.Description = model.StoreSliderform.Description;

                getSlider.ModifiedBy = User.Identity.Name;
                getSlider.ModifiedDate = DateTime.Now;
                getSlider.IsDeleted = model.StoreSliderform.IsDeleted;
                db.SaveChanges();
                TempData["message"] = "The Slider has been updated successfully.";
                return RedirectToAction("SliderList", "Store", new { Id = GetStore.ProcessInstaceId, area = "Setup" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult CollectionList(Guid Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                model.ImageCollectionsList = Backbone.GetImageCollection(db, Id);
                model.documentPath = Properties.Settings.Default.CollectionPath;
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                model.store = GetStore;
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

        public ActionResult EditCollection(Guid Id, int CollectionId)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                var getCollection = db.StoreImageCollection.Where(x => x.ImageCollectionId == CollectionId && x.StoreId == GetStore.Id).FirstOrDefault();
                model.storeCollectionForm = new  StoreCollectionForm();
                model.storeCollectionForm.CollectionName = getCollection.ImageCollection.Name;
               
                model.storeCollectionForm.Id = getCollection.Id;
                model.documentValue = getCollection.CollectionPath;
                model.storeCollectionForm.StoreId = GetStore.Id;
                model.documentPath = Properties.Settings.Default.CollectionPath;
                model.store = GetStore;
                model.ImageCollection = getCollection;
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
        public ActionResult EditCollection(StoreManagementViewModel model)
        {
try
            {
                var GetStore = db.Store.Where(x => x.Id == model.storeCollectionForm.StoreId).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                if (ModelState.IsValid)
                {
                    string url = Properties.Settings.Default.CollectionPath;
                    System.IO.Directory.CreateDirectory(url);
                    var getCollection = db.StoreImageCollection.Where(x => x.Id == model.storeCollectionForm.Id && x.StoreId == GetStore.Id).FirstOrDefault();
                    if (model.storeCollectionForm.CollectionPhoto != null && model.storeCollectionForm.CollectionPhoto.ContentLength > 0)
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
                        var filePassport = System.IO.Path.GetExtension(model.storeCollectionForm.CollectionPhoto.FileName);
                        if (!supportedPassport.Contains(filePassport))
                        {
                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for Slider photo";
                            model.documentPath = Properties.Settings.Default.CollectionPath;
                            return View(model);
                        }
                        else if (model.storeCollectionForm.CollectionPhoto.ContentLength > max_upload)
                        {
                            TempData["messageType"] = "danger";
                            TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.CollectionPath;
                            return View(model);
                        }


                        //delete passport
                        if (getCollection.IsUpdated == true)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + getCollection.CollectionPath);
                            fi.Delete();
                          
                        
                        }
                        //store logo
                        int pp = 0;
                        string pName;
                        pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.storeCollectionForm.CollectionPhoto.FileName);
                        model.storeCollectionForm.CollectionPhoto.SaveAs(url + pName);

                        #endregion

                        getCollection.CollectionPath = pName;
                        getCollection.IsUpdated = true;
                        getCollection.ModifiedBy = User.Identity.Name;
                        getCollection.ModifiedDate = DateTime.Now;
                        getCollection.IsDeleted = model.storeCollectionForm.IsDeleted;
                        db.SaveChanges();
                        TempData["message"] = "The Collection has been updated successfully.";
                        return RedirectToAction("CollectionList", "Store", new { Id = GetStore.ProcessInstaceId, area = "Setup" });
                    }
                   

                   
                }
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
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


        public ActionResult TestimonialList(Guid Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();                
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                var GetTestimonial = GetStore.Testimonial.ToList();
                model.TestimonialList = GetTestimonial;
                model.store = GetStore;
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

        public ActionResult NewTestimonial(Guid Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                model.store = GetStore;
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
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewTestimonial(StoreManagementViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == model.store.ProcessInstaceId).FirstOrDefault();

                

                if (ModelState.IsValid)
                {

                    Testimonial addnew = new Testimonial()
                    {
                        ClientName = model.Testimonialform.ClientName,
                        Message = model.Testimonialform.Message,                                          
                        SentDate = DateTime.Now,
                        IsDeleted = false,
                        StoreId = GetStore.Id,
                    };
                    db.Testimonial.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "The Testimonial has been added successfully.";
                    return RedirectToAction("TestimonialList", "Store", new { area = "Setup", Id = model.store.ProcessInstaceId });

                }
                TempData["message"] = "ERROR: Please use the browse button to select photo from your computer.";
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

        public ActionResult EditTestimonial(Guid Id, int TestimonialId)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                var getTestimonial = db.Testimonial.Where(x => x.Id == TestimonialId && x.StoreId == GetStore.Id).FirstOrDefault();
                model.Testimonialform = new  TestimonialForm();               
                model.Testimonialform.Id = getTestimonial.Id;
                model.Testimonialform.ClientName = getTestimonial.ClientName;
                model.Testimonialform.StoreId = GetStore.Id;
                model.Testimonialform.Message = getTestimonial.Message;
                model.Testimonialform.IsDeleted = getTestimonial.IsDeleted;
                model.store = GetStore;

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

        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditTestimonial(StoreManagementViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.Id == model.Testimonialform.StoreId).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                if (ModelState.IsValid)
                {
                   
                    var getTestimonail = db.Testimonial.Where(x => x.Id == model.Testimonialform.Id && x.StoreId == GetStore.Id).FirstOrDefault();

                    getTestimonail.ClientName = model.Testimonialform.ClientName;
                    getTestimonail.IsDeleted = model.Testimonialform.IsDeleted; ;
                    getTestimonail.Message =model.Testimonialform.Message;                   
                    db.SaveChanges();
                    TempData["message"] = "The Testimonial has been updated successfully.";
                    return RedirectToAction("TestimonialList", "Store", new { Id = GetStore.ProcessInstaceId, area = "Setup" });


                }
                TempData["message"] = Settings.Default.GenericExceptionMessage;
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

        public ActionResult MenuList(Guid Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();               
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                model.MenuList = GetStore.Menu.ToList();
                model.store = GetStore;
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

        public ActionResult EditMenu(Guid Id, int MenuId)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                
                var getMenu = db.Menu.Where(x => x.Id == MenuId && x.StoreId == GetStore.Id).FirstOrDefault();
                model.Menuform = new  MenuForm();
                model.Menuform.Id = getMenu.Id;
                model.Menuform.Content = getMenu.Content;
                model.Menuform.StoreId = GetStore.Id;               
                model.Menuform.IsDeleted = getMenu.IsDeleted;
                model.Menuform.MenuTypeId = getMenu.MenuTypeId;
                model.store = GetStore;
                model.MenuTypeList = (from s in db.MenuType where s.IsDeleted == false && s.Id == model.Menuform.MenuTypeId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
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

        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditMenu(StoreManagementViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.Id == model.Menuform.StoreId).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                if (ModelState.IsValid)
                {
                    var getMenu = db.Menu.Where(x => x.Id == model.Menuform.Id && x.StoreId == GetStore.Id).FirstOrDefault();
                    getMenu.Content = model.Menuform.Content;
                    getMenu.IsDeleted = model.Menuform.IsDeleted; ;
                    getMenu.ModifiedBy = User.Identity.Name;
                    getMenu.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "The menu content has been updated successfully.";
                    return RedirectToAction("MenuList", "Store", new { Id = GetStore.ProcessInstaceId, area = "Setup" });


                }
                model.MenuTypeList = (from s in db.MenuType where s.IsDeleted == false && s.Id == model.Menuform.MenuTypeId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                TempData["message"] = Settings.Default.GenericExceptionMessage;
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


        public ActionResult StorePaymentMethod(Guid Id)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var getStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                if (getStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                List<int> list = (from x in getStore.PaymentMethod select x.Id).ToList<int>();
                model.PaymentMethodlist = db.PaymentMethod.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                var storepayment = getStore.PaymentMethod.ToList();
                model.StorePaymentMethod = storepayment;
                model.store = getStore;
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
        public ActionResult StorePaymentMethod(StoreManagementViewModel model)
        {
            try
            {
                var getpayment = db.PaymentMethod.Where(x => x.Id == model.PaymentMethodId).FirstOrDefault();
                if (getpayment == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StorePaymentMethod", "Store", new { Id = model.store.ProcessInstaceId, area = "Setup" });
                }
                var getStore = db.Store.Where(x => x.ProcessInstaceId == model.store.ProcessInstaceId).FirstOrDefault();
                if (getStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StorePaymentMethod", "Store", new { Id = model.store.ProcessInstaceId, area = "Setup" });
                }
                List<int> list = (from x in getStore.PaymentMethod select x.Id).ToList<int>();
                model.PaymentMethodlist = db.PaymentMethod.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

                var storepayment = getStore.PaymentMethod.ToList();
               // model.prductbrand = productbrand;
               // model.productcategory = getcategory;
                if (model.PaymentMethodId == 0)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "Please select a brand from the dropdownlist and click on the Add button";
                    return RedirectToAction("StorePaymentMethod", "Store", new { Id = model.store.ProcessInstaceId, area = "Setup" });
                }

               // var getbrand = db.PaymentMethod.Where(x => x.Id == model.PaymentMethodId).FirstOrDefault();
                getStore.PaymentMethod.Add(getpayment);
                db.SaveChanges();
                base.TempData["message"] = "The payment " + getpayment.Name + " has been added Successfully";
                return RedirectToAction("StorePaymentMethod", "Store", new { Id = model.store.ProcessInstaceId, area = "Setup" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult RemoveStorePaymentMethod(Guid Id, int PaymentId)
        {
            try
            {
                StoreManagementViewModel model = new StoreManagementViewModel();
                var getStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                var getpayment = db.PaymentMethod.Where(x => x.Id == PaymentId).FirstOrDefault();
                getStore.PaymentMethod.Remove(getpayment);
                db.SaveChanges();
                base.TempData["message"] = "The " + getpayment.Name + " has been deleted successfully.";
                return RedirectToAction("StorePaymentMethod", "Store", new { Id = Id, area = "Setup" });
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
