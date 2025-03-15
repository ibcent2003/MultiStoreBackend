using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Services;
using SecurityGuard.Interfaces;

namespace Project.Controllers
{
   

    public class StoreRegistrationController : Controller
    {
        Backbone services = new Backbone();
        public PROEntities db = new PROEntities();
        private ProcessUtility util = new ProcessUtility();

        private IMembershipService membershipService;
        private IAuthenticationService authenticationService;
        private IFormsAuthenticationService formsAuthenticationService;



        public StoreRegistrationController()
        {
            this.membershipService = new MembershipService(Membership.Provider);
            this.authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());
            this.formsAuthenticationService = new FormsAuthenticationService();         
        }


        public ActionResult Index()
        {
           try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                string guidelines = (from m in db.Workflow where m.Id == Properties.Settings.Default.StoreRegistrationWorkFlowId select m.guideline).FirstOrDefault();
                TempData["GuideLines"] = guidelines;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }       


        public ActionResult StoreInformation()
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();

                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult StoreInformation(StoreRegistrationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                        string url = Properties.Settings.Default.DocumentPath;
                        System.IO.Directory.CreateDirectory(url);

                        var checkStore = db.Store.Where(x => x.Name == model.storeform.Name).FirstOrDefault();
                    
                        if(checkStore != null)
                        {

                        var Approval = checkStore.WorkflowSteps.FirstOrDefault();
                        if (Approval != null)
                        {
                            model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id==1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                            model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                            model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                            TempData["MessageType"] = "danger";
                            TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                            return View(model);
                        }

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
                                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                TempData["messageType"] = "danger";
                                    TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                                    model.documentPath = Properties.Settings.Default.DocumentPath;
                                    return View(model);
                                }
                                else if (model.storeform.Logo.ContentLength > max_upload)
                                {
                                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                TempData["messageType"] = "danger";
                                    TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                                    model.documentPath = Properties.Settings.Default.DocumentPath;
                                    return View(model);
                                }



                                //store logo
                                int pp = 0;
                                string pName;
                                pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.storeform.Logo.FileName);
                                //delete passport
                                if (checkStore.Logo != null)
                                {
                                    System.IO.FileInfo fi = new System.IO.FileInfo(url + checkStore.Logo);
                                    fi.Delete();
                                }
                                model.storeform.Logo.SaveAs(url + pName);

                                #endregion

                                checkStore.Logo = pName;
                            }
                          
                            checkStore.Name = model.storeform.Name;
                            checkStore.OwnProcurement = model.storeform.OwnProcurement;
                            checkStore.CountryId = model.storeform.CountryId;
                            checkStore.ThemesId = model.storeform.ThemesId;
                            checkStore.BankId = model.storeform.BankId;
                            checkStore.AccountName = model.storeform.AccountName;
                            checkStore.AccountNumber = model.storeform.AccountNumber;
                            checkStore.URL = model.storeform.URL;
                            checkStore.URL2 = model.storeform.URL2;
                            checkStore.URL3 = model.storeform.URL3;
                            checkStore.ModifiedBy =model.storeform.Name;
                            checkStore.ModifiedDate = DateTime.Now;
                            checkStore.Description = model.storeform.Description;                       
                            db.SaveChanges();
                            TempData["message"] = "<b>" + model.storeform.Name + "</b> Information been successfully.";
                            return RedirectToAction("NewAddress", "StoreRegistration", new { Id = checkStore.ProcessInstaceId, area = "" });
                         }
                        else
                        {
                        #region If Store information is new entry

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
                                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                TempData["messageType"] = "danger";
                                    TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                                    model.documentPath = Properties.Settings.Default.DocumentPath;
                                    return View(model);
                                }
                                else if (model.storeform.Logo.ContentLength > max_upload)
                                {
                                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                                TempData["messageType"] = "danger";
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

                                model.logos = pName;
                            }                         
                            Store addnew = new Store()    
                            {
                                Name = model.storeform.Name,
                                Logo = model.logos,
                                CountryId = model.storeform.CountryId,   
                                ThemesId = model.storeform.ThemesId,
                                BankId = model.storeform.BankId,
                                AccountName = model.storeform.AccountName,
                                AccountNumber = model.storeform.AccountNumber,
                                OwnProcurement = model.storeform.OwnProcurement,
                                ModifiedBy = model.storeform.Name,
                                ModifiedDate = DateTime.Now,
                                WorkFlowId= Properties.Settings.Default.StoreRegistrationWorkFlowId,
                                Status ="draft",
                                OwnedBy = "Administrator",
                                IsDeleted = false,
                                ProcessInstaceId = Guid.NewGuid(),
                                URL = model.storeform.URL,
                                URL2 = model.storeform.URL2,
                                URL3 = model.storeform.URL3,
                                Description = model.storeform.Description
                            };
                            db.Store.AddObject(addnew);
                            db.SaveChanges();

                        #endregion

                        #region populate image collection table                        
                        var GetImageCollection = db.ImageCollection.Where(x=>x.IsDeleted==false).ToList();
                         foreach(var img in GetImageCollection)
                        {
                            StoreImageCollection newImage = new StoreImageCollection
                            {
                                StoreId = addnew.Id,
                                ImageCollectionId = img.Id,
                                CollectionPath = img.Name,
                                ModifiedBy = model.storeform.Name,
                                ModifiedDate = DateTime.Now,
                                IsDeleted = false,
                                IsUpdated = false
                               

                            };
                            db.StoreImageCollection.AddObject(newImage);
                            db.SaveChanges();
                        }
                        #endregion

                        #region populate menu items table

                        var getMenu = db.MenuType.Where(x => x.IsDeleted == false).ToList();
                        foreach(var menu in getMenu)
                        {
                            Menu newmenu = new Menu
                            {
                                StoreId = addnew.Id,
                                MenuTypeId = menu.Id,
                                HeaderType = menu.Name,
                                Content = menu.Name,
                                ModifiedBy = model.storeform.Name,
                                ModifiedDate = DateTime.Now,
                                IsDeleted = false,
                            };
                            db.Menu.AddObject(newmenu);
                            db.SaveChanges();
                        }
                        #endregion

                        #region populate payment method

                        var getPayment = db.PaymentMethod.Where(x => x.IsDeleted == false).ToList();
                       // var store = Backbone.GetStore(db, addnew.ProcessInstaceId);
                        foreach (var pay in getPayment)
                        {
                           addnew.PaymentMethod.Add(pay);
                           db.SaveChanges();
                        }
                        #endregion

                        TempData["message"] = "<b>" + model.storeform.Name + "</b> Information been successfully.";
                        return RedirectToAction("NewAddress", "StoreRegistration", new { Id = addnew.ProcessInstaceId, area = "" });
                       
                    }

                   }
                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                TempData["message"] = "Error: All fields marked with red * are mandatory";
                TempData["messageType"] = "danger";
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult ViewStoreInformation(Guid Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                model.documentPath = Properties.Settings.Default.DocumentPath;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult EditStoreInformation(Guid Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
                model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                
                model.storeform = new  Areas.Setup.Models.StoreForm();
                model.storeform.Name = GetStore.Name;
                model.storeform.Id = GetStore.Id;
                model.storeform.CountryId = GetStore.CountryId;
                model.storeform.ThemesId = int.Parse(GetStore.ThemesId.ToString());
                model.storeform.BankId = GetStore.BankId;
                model.storeform.AccountName = GetStore.AccountName;
                model.storeform.AccountNumber = GetStore.AccountNumber;
                model.storeform.URL = GetStore.URL;
                model.storeform.URL2 = GetStore.URL3;
                model.storeform.URL3 = GetStore.URL3;
                model.logos = GetStore.Logo;
                model.storeform.OwnProcurement = GetStore.OwnProcurement;
                model.documentPath = Properties.Settings.Default.DocumentPath;
                model.storeform.Description = GetStore.Description;
                model.store = GetStore;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult EditStoreInformation(StoreRegistrationViewModel model)
        {
            try
            {
                var GetStore = db.Store.Where(x => x.ProcessInstaceId == model.store.ProcessInstaceId).FirstOrDefault();
                if (GetStore == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
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
                        model.store = GetStore;
                        model.logos = GetStore.Logo;
                        model.CountryList = db.Country.Where(x => x.IsDeleted == false && x.Id == 1).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);
                    }
                    else if (model.storeform.Logo.ContentLength > max_upload)
                    {
                        model.store = GetStore;
                        model.logos = GetStore.Logo;
                        model.CountryList = db.Country.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.CurrencyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.ThemesList = db.Themes.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        model.BankList = db.Bank.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The logo uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.DocumentPath;
                        return View(model);
                    }


                   
                    //store logo
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.storeform.Logo.FileName);

                    //delete passport
                    if (GetStore.Logo != null)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(url + GetStore.Logo);
                        fi.Delete();
                    }

                    model.storeform.Logo.SaveAs(url + pName);

                    #endregion

                    GetStore.Logo = pName;
                }
                GetStore.Name = model.storeform.Name;
                GetStore.URL = model.storeform.URL;
                GetStore.URL2 = model.storeform.URL2;
                GetStore.URL3 = model.storeform.URL3;
                GetStore.ModifiedBy = model.storeform.Name;
                GetStore.OwnProcurement = model.storeform.OwnProcurement;
                GetStore.CountryId = model.storeform.CountryId;
                GetStore.ThemesId = model.storeform.ThemesId;
                GetStore.BankId = model.storeform.BankId;
                GetStore.AccountName = model.storeform.AccountName;
                GetStore.AccountNumber = model.storeform.AccountNumber;
                GetStore.ModifiedDate = DateTime.Now;
                GetStore.IsDeleted = model.storeform.IsDeleted;
                GetStore.Description = model.storeform.Description;
                db.SaveChanges();
                TempData["message"] = "The Store " + model.storeform.Name + " has been updated successfully.";
                return RedirectToAction("NewAddress", "StoreRegistration", new { Id = GetStore.ProcessInstaceId, area = "" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
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


        public ActionResult AddressList(Guid Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }               
                model.addressList = Backbone.GetStorAddress(db, Id);
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult NewAddress(Guid  Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                model.StateList = (from s in this.db.State where s.CountryId == model.store.CountryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.LgaList = (from s in this.db.LGA where s.State.CountryId == model.store.CountryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();

                model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.addressList = Backbone.GetStorAddress(db, Id);
               
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult NewAddress(StoreRegistrationViewModel model)
        {
            try
            {
                model.StateList = (from s in this.db.State where s.CountryId == model.store.CountryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.LgaList = (from s in this.db.LGA where s.State.CountryId == model.store.CountryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.addressList = Backbone.GetStorAddress(db, model.store.ProcessInstaceId);
                model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();

                if (ModelState.IsValid)
                {

                    if(model.store.CountryId==1)
                    {
                        if (!Regex.IsMatch(model.addressform.MobileNumber, "\\A[0-9]{11}\\z"))
                        {
                            // display error : 11 digit is required for NGN
                            base.TempData["messageType"] = "danger";
                            base.TempData["message"] = string.Concat("Invalid Mobile No: 11 digits are required. e.g 08000000000");
                            return View(model);
                        }
                    }
                    else if(model.store.CountryId==2)
                    {
                        if (!Regex.IsMatch(model.addressform.MobileNumber, "^([0-9]{10})$"))
                        {
                            // display error : 11 digit is required for GH
                            base.TempData["messageType"] = "danger";
                            base.TempData["message"] = string.Concat("Invalid Mobile No: 10 digits are required. e.g 056000000");
                            return View(model);
                        }
                    }


                 
                    string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                    //check first string
                    if (!Regex.IsMatch(model.addressform.EmailAddress, pattern))
                    {
                        //if email is invalid
                        base.TempData["messageType"] = "danger";
                        base.TempData["message"] = string.Concat("The Email Address entered is invalid. Please enter a valid email. eg. someone@gamil.com");
                        return View(model);
                    }
                    

                    //validate email
                    var check = db.AddressBook.Where(x => x.EmailAddress == model.addressform.EmailAddress).ToList();
                    if (check.Any())
                    {
                        model.StateList = (from s in this.db.State where s.CountryId==model.store.CountryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                        model.LgaList = (from s in this.db.LGA where s.State.CountryId == model.store.CountryId select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                        model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                        base.TempData["messageType"] = "danger";
                        base.TempData["message"] = string.Concat("The Email ", model.addressform.EmailAddress, " already exist. Please try different email");
                        model.addressList = Backbone.GetStorAddress(db, model.store.ProcessInstaceId);
                        return View(model);
                    }

                    var store = Backbone.GetStore(db, model.store.ProcessInstaceId);

                    AddressBook addressBook = new AddressBook()
                    {
                        AddressTypeId = model.addressform.AddressTypeId,
                        Street = model.addressform.Street,
                        LgaId = model.addressform.LgaId,
                        MobileNumber = model.addressform.MobileNumber,
                        EmailAddress = model.addressform.EmailAddress,
                        GoogleMapURL = model.addressform.GoogleMap,
                        ModifiedBy =store.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false,
                    };
                    this.db.AddressBook.AddObject(addressBook);
                    store.AddressBook.Add(addressBook);
                    this.db.SaveChanges();
                    model.addressList = Backbone.GetStorAddress(db, model.store.ProcessInstaceId);
                    base.TempData["message"] = string.Concat("The Address <b>", model.addressform.Street, "</b> has been added successfully. Add more address or click on the next button below to proceed.");
                    return RedirectToAction("NewAddress", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });

                }
               
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = string.Concat("Ops! Something went wrong. Please make sure you enter all fields with red*. Reload this page iff you encounter any issue.");
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }


        [HttpPost]
        public ActionResult GetLGAId(int stateId)
        {
            List<IntegerSelectListItem> list = (from d in this.db.LGA where d.StateId == stateId orderby d.Name select new IntegerSelectListItem() { Text = d.Name, Value = d.Id }).ToList<IntegerSelectListItem>();
            return base.Json(list);

        }

        public ActionResult ContactPerson(Guid Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult ContactPerson(StoreRegistrationViewModel model)
        {
            try
            {
                var store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                if (base.ModelState.IsValid)
                {

                    if (model.store.CountryId == 1)
                    {
                        if (!Regex.IsMatch(model.contactform.MobileNumber, "\\A[0-9]{11}\\z"))
                        {
                            // display error : 11 digit is required for NGN
                            base.TempData["messageType"] = "danger";
                            base.TempData["message"] = string.Concat("Invalid Mobile No: 11 digits are required. e.g 08000000000");
                            model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                            return View(model);
                        }
                    }
                    else if (model.store.CountryId == 2)
                    {
                        if (!Regex.IsMatch(model.contactform.MobileNumber, "^([0-9]{10})$"))
                        {
                            // display error : 11 digit is required for GH
                            base.TempData["messageType"] = "danger";
                            base.TempData["message"] = string.Concat("Invalid Mobile No: 10 digits are required. e.g 056000000");
                            model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                            return View(model);
                        }
                    }

                    //validate email address
                    var getemail = db.ContactInfo.Where(x => x.EmailAddress == model.contactform.EmailAddress).ToList();
                    if (getemail.Any())
                    {
                        TempData["message"] = "The Email address <b>" + model.contactform.EmailAddress + "</b> has already been used by someone. Please enter another email address";
                        TempData["messageType"] = "danger";
                        model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                        return View(model);
                    }
                    ContactInfo contactInfo = new ContactInfo()
                    {
                        FirstName = model.contactform.FirstName,
                        LastName = model.contactform.LastName,
                        EmailAddress = model.contactform.EmailAddress,
                        MobileNo = model.contactform.MobileNumber,
                        ModifiedBy = model.store.Name,
                        ModifiedDate = DateTime.Now
                    };
                    this.db.ContactInfo.AddObject(contactInfo);
                    store.ContactInfo.Add(contactInfo);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat(new string[] { "<b>", model.contactform.FirstName, "</b> <b>", model.contactform.LastName, "</b>  was Successfully Added" });
                    return RedirectToAction("ContactPerson", "StoreRegistration", new { Id = store.ProcessInstaceId, area = "" });
                }
                model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                TempData["message"] = "<b>Ops!</b> something went wrong. Please make sure you enter all fields";
                TempData["messageType"] = "danger";
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult ProductCategory(Guid Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                List<int> list = (from x in model.store.ProductCategory select x.Id).ToList<int>();
                model.Categorylist = db.ProductCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult ProductCategory(StoreRegistrationViewModel model)
        {
            try
            {

                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }


                var getcategory = db.ProductCategory.Where(x => x.Id == model.ProductCategoryId).FirstOrDefault();
                if (getcategory == null)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "Please select a Category from the dropdownlist and click on the Add button";
                    return RedirectToAction("ProductCategory", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });
                }
                List<int> list = (from x in model.store.ProductCategory select x.Id).ToList<int>();
                model.Categorylist = db.ProductCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

                model.StoreProductCategory = model.store.ProductCategory.ToList();

                if (model.ProductCategoryId == 0)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "Please select a Category from the dropdownlist and click on the Add button";
                    return RedirectToAction("ProductCategory", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });
                }

                model.store.ProductCategory.Add(getcategory);
                db.SaveChanges();
                base.TempData["message"] = "The Category " + getcategory.Name.ToUpper() + " has been added Successfully";
                return RedirectToAction("ProductCategory", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult RemoveStoreCategory(Guid Id, int CategoryId)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();

                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                var getcategory = db.ProductCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                if (getcategory == null)
                {
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("ProductCategory", "StoreRegistration", new { Id = Id, area = "" });
                }

                //check if product has been uploaded for this category
                var validateProduct = model.store.StoreProduct.Where(x => x.ProductCategoryId == CategoryId).ToList();
                if (validateProduct.Any())
                {
                    TempData["message"] = "You can not remove this category from your store because there is an existing product attacted to the category " + getcategory.Name.ToUpper() + ". Kindly delete the product before deleting the category or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("ProductCategory", "StoreRegistration", new { Id = Id, area = "" });
                }

                model.store.ProductCategory.Remove(getcategory);
                db.SaveChanges();
                base.TempData["message"] = "The " + getcategory.Name + " has been deleted successfully.";
                return RedirectToAction("ProductCategory", "StoreRegistration", new { Id = Id, area = "" });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult SubCategory1(Guid Id, int CategoryId)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" }); ;
                }
                var GetCate = db.ProductCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                if (GetCate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";                    
                    return RedirectToAction("ProductCategory", "StoreRegistration", new { Id = Id, area = "" });
                }
                model.storeCate = GetCate;
                List<int> list = (from x in model.store.ProductSubCategory select x.Id).ToList<int>();
                model.SubCategorylist = db.ProductSubCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductCategoryId == CategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

                model.StoreProductSubCategory = model.store.ProductSubCategory.Where(x => x.ProductCategoryId == CategoryId).ToList();
                if (model.SubCategorylist.Count() == 0)
                {
                    model.HasAllSubCategory = false;
                }
                else
                {
                    model.HasAllSubCategory = true;
                }
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" }); ;
            }
        }


        [HttpPost]
        public ActionResult GetSubCatId(int CategoryId)
        {
            //StoreRegistrationViewModel model = new StoreRegistrationViewModel();
            //model.store = Backbone.GetStore(db, Guid.Parse(Session["Id"].ToString()));

            // var  store = db.Store.Where(x => x.ProcessInstaceId == Guid.Parse(Session["Id"].ToString()));
            // var list = (List<int>)Session["Id"];     
            //List<int> list = (from x in db.ProductSubCategory
            //                  join s in db.
            //                  select x.Id).ToList<int>();
            List<IntegerSelectListItem> sublist = (from d in db.ProductSubCategory where d.ProductCategoryId == CategoryId orderby d.Name select new IntegerSelectListItem() { Text = d.Name, Value = d.Id }).ToList<IntegerSelectListItem>();
            return base.Json(sublist);

        }

        public ActionResult SubCategory(Guid Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreInformation");
                }
              
                
                List<int> list = (from x in model.store.ProductSubCategory select x.Id).ToList<int>();
                if (Session["Id"] != null)
                {
                    Session["Id"] = list;
                }
                model.Categorylist = model.store.ProductCategory.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

                model.SubCategorylist = model.store.ProductSubCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.StoreProductSubCategory = model.store.ProductSubCategory.ToList();
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("StoreInformation");
            }
        }

        [HttpPost]
        public ActionResult SubCategory(StoreRegistrationViewModel model)
        {
            try
            {
                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                var getSubcategory = db.ProductSubCategory.Where(x => x.Id == model.ProductSubCategoryId).FirstOrDefault();
                if (getSubcategory == null)
                {
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("SubCategory", "StoreRegistration", new { Id = model.store.ProcessInstaceId,CategoryId=model.storeCate.Id, area = "" });
                }
                //List<int> list = (from x in model.store.ProductSubCategory select x.Id).ToList<int>();
                //model.SubCategorylist = db.ProductSubCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductCategoryId == getSubcategory.ProductCategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

                List<int> list = (from x in model.store.ProductSubCategory select x.Id).ToList<int>();
                model.Categorylist = model.store.ProductCategory.Where(x => x.IsDeleted == false).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

                model.SubCategorylist = model.store.ProductSubCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id)).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

              


                //model.StoreProductSubCategory = model.store.ProductSubCategory.Where(x => x.ProductCategoryId == getSubcategory.ProductCategoryId).ToList();
                model.StoreProductSubCategory = model.store.ProductSubCategory.ToList();
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                model.store.ProductSubCategory.Add(getSubcategory);
                db.SaveChanges();
                base.TempData["message"] = "The Category " + getSubcategory.Name.ToUpper() + " has been added Successfully";
                return View(model);
                //return RedirectToAction("SubCategory", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" }); ;
            }
        }


        public ActionResult RemoveStoreSubCategory(Guid Id, int SubCategoryId)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                var getSubcategory = db.ProductSubCategory.Where(x => x.Id == SubCategoryId).FirstOrDefault();
                if (getSubcategory == null)
                {
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("SubCategory", "StoreRegistration", new { Id = Id, CategoryId = getSubcategory.ProductCategoryId, area = "" });
                }
                //check if product has been uploaded for this subcategory
                var validateProduct = model.store.StoreProduct.Where(x => x.ProductSubCategoryId == SubCategoryId).ToList();
                if (validateProduct.Any())
                {
                    TempData["message"] = "You can not remove this category from your store because there is an existing product attacted to the category " + getSubcategory.Name.ToUpper() + ". Kindly delete the product before deleting the category or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("SubCategory", "StoreRegistration", new { Id = Id, CategoryId = getSubcategory.ProductCategoryId, area = "" });
                }
                model.store.ProductSubCategory.Remove(getSubcategory);
                db.SaveChanges();
                base.TempData["message"] = "The " + getSubcategory.Name + " has been deleted successfully.";
                return RedirectToAction("SubCategory", "StoreRegistration", new { Id = Id, CategoryId = getSubcategory.ProductCategoryId, area = "" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }


        public ActionResult ChildCategory(Guid Id, int SubCategoryId)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                var GetSubCate = db.ProductSubCategory.Where(x => x.Id == SubCategoryId).FirstOrDefault();
                if (GetSubCate == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                model.storesubCate = GetSubCate;
                List<int> list = (from x in model.store.ProductChildCategory select x.Id).ToList<int>();
                model.ChildCategorylist = db.ProductChildCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductSubCategoryId == SubCategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.StoreProductChildCategory = model.store.ProductChildCategory.Where(x => x.ProductSubCategoryId == SubCategoryId).ToList();
                if (model.ChildCategorylist.Count() == 0)
                {
                    model.HasAllChildCategory = false;
                }
                else
                {
                    model.HasAllChildCategory = true;
                }
                var GetCate = db.ProductCategory.Where(x => x.Id == GetSubCate.ProductCategoryId).FirstOrDefault();
                model.storeCate = GetCate;
                model.storesubCate = GetSubCate;

                return View(model);

            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult ChildCategory(StoreRegistrationViewModel model)
        {
            try
            {
                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                var getChildbcategory = db.ProductChildCategory.Where(x => x.Id == model.ProductChildCategoryId).FirstOrDefault();
                if (getChildbcategory == null)
                {
                    TempData["message"] = "ERROR: Please make sure you have selected a child category from the dropdown before clicking on the Add button.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("ChildCategory", "StoreRegistration", new { Id = model.store.ProcessInstaceId, SubCategoryId = model.storesubCate.Id, area = "" });
                }
                List<int> list = (from x in model.store.ProductChildCategory select x.Id).ToList<int>();

                model.ChildCategorylist = db.ProductChildCategory.Where(x => x.IsDeleted == false && !list.Contains(x.Id) && x.ProductSubCategoryId == getChildbcategory.ProductSubCategoryId).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                if (model.ChildCategorylist == null)
                {
                    model.HasAllSubCategory = true;
                }
                else
                {
                    model.HasAllSubCategory = false;
                }


                model.StoreProductChildCategory = model.store.ProductChildCategory.Where(x => x.ProductSubCategoryId == getChildbcategory.ProductSubCategoryId).ToList();
                model.store.ProductChildCategory.Add(getChildbcategory);
                db.SaveChanges();
                base.TempData["message"] = "The Category " + getChildbcategory.Name.ToUpper() + " has been added Successfully";
                return RedirectToAction("ChildCategory", "StoreRegistration", new { Id = model.store.ProcessInstaceId, SubCategoryId = getChildbcategory.ProductSubCategoryId, area = "" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult RemoveStoreChildCategory(Guid Id, int ChildCategoryId)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                var getSubcategory = db.ProductChildCategory.Where(x => x.Id == ChildCategoryId).FirstOrDefault();
                if (getSubcategory == null)
                {
                    TempData["message"] = "Error: Something went wrong. Please try again later or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("ChildCategory", "StoreRegistration", new { Id = Id, SubCategoryId = getSubcategory.ProductSubCategoryId, area = "" });
                }
                //check if product has been uploaded for this subcategory
                var validateProduct = model.store.StoreProduct.Where(x => x.ProductChildCategoryId == ChildCategoryId).ToList();
                if (validateProduct.Any())
                {
                    TempData["message"] = "You can not remove this Child category from your store because there is an existing product attacted to " + getSubcategory.Name.ToUpper() + ". Kindly delete the product before deleting the child category or contact the system administrator. ";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("ChildCategory", "StoreRegistration", new { Id = Id, SubCategoryId = getSubcategory.ProductSubCategoryId, area = "" });
                }
                model.store.ProductChildCategory.Remove(getSubcategory);
                db.SaveChanges();
                base.TempData["message"] = "The " + getSubcategory.Name + " has been deleted successfully.";
                return RedirectToAction("ChildCategory", "StoreRegistration", new { Id = Id, SubCategoryId = getSubcategory.ProductSubCategoryId, area = "" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult UserAccount(Guid Id)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }

                var GetTempUser = model.store.TempUser.FirstOrDefault();
                if (GetTempUser != null)
                {                   
                    model.tempUser = GetTempUser;
                    model.TempUseradded = true;                                      
                    return View(model);
                }
                else
                {
                    var user = model.store.Users.FirstOrDefault();
                    if(user != null)
                    {
                        model.LoginDetails = db.Memberships.Where(x=>x.UserId==user.UserId).FirstOrDefault();
                        model.TempUseradded = false;
                    }                                     
                    return View(model);
                }
                
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult UserAccount(StoreRegistrationViewModel model)
        {
            try
            {
                model.store = Backbone.GetStore(db, model.store.ProcessInstaceId);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                //var getSubcategory = model.store.ProductSubCategory.Where(x => x.Id == model.ProductSubCategoryId).FirstOrDefault();
                //if(getSubcategory==null)
                //{
                //    TempData["message"] = Settings.Default.GenericExceptionMessage;
                //    TempData["messageType"] = "danger";
                //    return RedirectToAction("Error404", "Home", new { area = "" });
                //}
                if (base.ModelState.IsValid)
                {
                    #region validations               
                    //check temp user
                    var validateTempUser = db.TempUser.Where(x => x.Username == model.tempUserform.Username).ToList();
                    if (validateTempUser.Any())
                    {
                        model.TempUseradded = false;
                       // model.storesubCate = getSubcategory;
                        TempData["message"] = "Error: The username entered has already been used by another user. Please enter another username. ";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    //check temp email
                    var validateTempEmail = db.TempUser.Where(x => x.EmailAddres == model.tempUserform.EmailAddress).ToList();
                    if (validateTempUser.Any())
                    {
                        model.TempUseradded = false;
                       // model.storesubCate = getSubcategory;
                        TempData["message"] = "Error: The email address entered has already been used by another user. Please enter another email address. ";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }

                    //check membership username
                    var validateMemUser = db.Users.Where(x => x.UserName == model.tempUserform.Username).ToList();
                    if (validateMemUser.Any())
                    {
                        model.TempUseradded = false;
                       // model.storesubCate = getSubcategory;
                        TempData["message"] = "Error: The username entered has already been used by another user. Please enter another username. ";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }

                    //check membership email
                    var validateMemEmail = db.Memberships.Where(x => x.Email == model.tempUserform.EmailAddress).ToList();
                    if (validateMemUser.Any())
                    {

                        model.TempUseradded = false;
                       // model.storesubCate = getSubcategory;
                        TempData["message"] = "Error: The email address entered has already been used by another user. Please enter another email address. ";
                        TempData["messageType"] = "danger";
                       return View(model);
                    }
                    #endregion

                    //insert here
                    TempUser addnew = new TempUser
                    {
                    Username = model.tempUserform.Username,
                    EmailAddres = model.tempUserform.EmailAddress,
                    StoreId = model.store.Id,
                    Password = model.tempUserform.Password,                                      
                    };
                    db.TempUser.AddObject(addnew);
                    db.SaveChanges();
                    base.TempData["message"] = "Your account information has been added successfully.";
                    return RedirectToAction("Review", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });
                }
                model.TempUseradded = false;
               
                TempData["message"] = "Error: Something went wrong. Please make sure you enter all field with red *. ";
                TempData["messageType"] = "danger";
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult RemoveTem(Guid Id, int TempId)
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }

                //var getSubcategory = model.store.ProductSubCategory.Where(x => x.Id == SubCategoryId).FirstOrDefault();
                //if(null== getSubcategory)
                //{
                //    TempData["message"] = Settings.Default.GenericExceptionMessage;
                //    TempData["messageType"] = "danger";
                //    return RedirectToAction("Error404", "Home", new { area = "" });
                //}
               // model.storesubCate = getSubcategory;

                var GetTempUser = model.store.TempUser.Where(x=>x.Id==TempId).FirstOrDefault();
                if (GetTempUser != null)
                {
                    db.TempUser.DeleteObject(GetTempUser);
                    db.SaveChanges();
                    TempData["message"] = "The account has been removed. Please enter new account and proceed";
                    return RedirectToAction("UserAccount", "StoreRegistration", new { Id = Id, area = "" });
                }
                else
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("UserAccount", "StoreRegistration", new { Id = Id, area = "" });
                }
               
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }


        public ActionResult Review(Guid Id)
        {
            try
            {                
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Error404", "Home", new { area = "" });
                }
                              
                model.documentPath = Properties.Settings.Default.DocumentPath;                
                model.addressList = Backbone.GetStorAddress(db, Id);
                model.contactInfoList = Backbone.GetStoreContactInfo(db, model.store.ProcessInstaceId);
                model.StoreProductCategory = model.store.ProductCategory.ToList();
                var GetTempUser = model.store.TempUser.FirstOrDefault();
                if (GetTempUser != null)
                {
                    model.tempUser = GetTempUser;
                    model.TempUseradded = true;
                
                }
                else
                {
                    var user = model.store.Users.FirstOrDefault();
                    if (user != null)
                    {
                        model.LoginDetails = db.Memberships.Where(x => x.UserId == user.UserId).FirstOrDefault();
                        model.TempUseradded = false;
                    }
                }
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult SubmitRegistration(Guid Id)
        {
            try
            {
                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No Process Instance Id sent to submit registration action: Store Registration Controller"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                }

                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreInformation");
                }
                var tempUser = model.store.TempUser.Where(x => x.StoreId==model.store.Id).FirstOrDefault();
                if (tempUser == null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Temp User table in db is null"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                    #endregion
                }
                MembershipCreateStatus createStatus;
                var Contact = model.store.ContactInfo.FirstOrDefault();
                if (Contact == null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find store contact during final submission. The Object model.store.ContactInfo.FirstOrDefault()"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                    #endregion
                }

                var GetRoleAdmin = db.Roles.Where(x => x.RoleName == "Store Admin").FirstOrDefault();
                if (GetRoleAdmin == null)
                {
                    //throw expection                   
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find role name store admin"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                    #endregion
                }

                var GetWorkFlowId = db.WorkflowSteps.Where(x => x.WorkflowId == model.store.WorkFlowId && x.Priority == 1).FirstOrDefault();

                membershipService.CreateUser(tempUser.Username, tempUser.Password, tempUser.EmailAddres, null, null, true, out createStatus);
                if(model.store.Status == "Registration Verification")
                {
                  
                }
                var user = db.Users.Where(x => x.UserName == tempUser.Username).FirstOrDefault();
                if (user == null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find  user membershipService.CreateUser"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                    #endregion
                }

                string str = (from r in this.db.Roles where r.RoleName == GetWorkFlowId.RoleName  select r.RoleName).FirstOrDefault<string>();
                string str1 = str;
                WorkflowSteps workflowStep1 = model.store.WorkflowSteps.FirstOrDefault<WorkflowSteps>();
                if (workflowStep1 != null)
                {
                    model.store.WorkflowSteps.Remove(workflowStep1);
                    this.db.SaveChanges();
                }
                CodeGenerator CodeGen = new CodeGenerator();
                model.store.OwnedBy = str1;
                model.store.Status = "Registration Verification";
                model.store.RegistrationNo = CodeGen.PadZeroes(10, CodeGen.GetNextID("StoreRegId"));
                model.store.ModifiedBy = user.UserName;
                model.store.ModifiedDate = DateTime.Now;
                model.store.SubmissionDate = new DateTime?(DateTime.Now);
                model.store.WorkflowSteps.Add(GetWorkFlowId);
                model.store.Roles.Add(GetRoleAdmin);
                user.Roles.Add(GetRoleAdmin);
                model.store.Users.Add(user);
                this.db.SaveChanges();
                           
                #region Generate slider
                var getSlider = db.SliderTemplate.ToList();
                foreach(var s in getSlider)
                {
                    StoreSlider addnew = new StoreSlider()
                    {
                        CaptionOne = s.CaptionOne,
                        CaptionTwo = s.CaptionTwo,
                        ButtonText = s.ButtonText,
                        Description = s.Description,
                        SliderPhoto = s.SliderPhoto,
                        ModifiedBy = model.store.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false,
                        StoreId = model.store.Id,
                    };
                    db.StoreSlider.AddObject(addnew);
                    db.SaveChanges();
                }
                    #endregion
               
                Alert alert = (from x in this.db.Alert where x.Id == 4 select x).FirstOrDefault<Alert>();
                Backbone.SendEmailNotificationToUser(db,alert.SubjectEmail, alert.Email.Replace("%Store_Name%", model.store.Name).Replace("%year%", DateTime.Now.Year.ToString()), model.store.ContactInfo.FirstOrDefault<ContactInfo>().EmailAddress, Settings.Default.EmailReplyTo, alert.Id);
                Backbone.SendSMSNotificationToUser(db,alert.SubjectSms, alert.Sms.Replace("%Store_Name%", model.store.Name), model.store.ContactInfo.FirstOrDefault().MobileNo, alert.SubjectSms, alert.Id);
               // services.SendEmail(db, model.store.ContactInfo.FirstOrDefault<ContactInfo>().EmailAddress);
                #region send message to approval admin
                var poolUser = new List<Users>();
                var rl = db.Roles.SingleOrDefault(x => x.RoleName == str1);

                var GetUser = rl.Users.ToList();
                var GetDirectorMsg = db.Alert.Where(x => x.Id == 5).FirstOrDefault();
                foreach (var u in GetUser)
                {
                    var GetUserInformation = db.UserDetail.Where(x => x.UserId == u.UserId).FirstOrDefault();
                    if (GetUserInformation != null)
                    {
                        if (GetUserInformation.MobileNumber != null)
                        {
                            services.SendSMSNotificationToAdmin(db,GetDirectorMsg.SubjectSms, GetDirectorMsg.Sms.Replace("%First_Name%", GetUserInformation.FirstName), GetUserInformation.MobileNumber, GetDirectorMsg.SubjectSms, GetDirectorMsg.Id);
                        }
                    }
                    services.SendEmailNotificationToAdmin(db,GetDirectorMsg.Title, GetDirectorMsg.Email.Replace("%First_Name%", u.UserName).Replace("%Store_Name%", model.store.Name).Replace("%Country%", model.store.Country.Name), GetUserInformation.EmailAddres, Settings.Default.EmailReplyTo, GetDirectorMsg.Id);
                }
                #endregion                

                return RedirectToAction("Feedback", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("StoreInformation");
            }
        }


        public ActionResult SubmitedRejected(Guid Id)
        {
            try
            {
                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No Process Instance Id sent to submit registration action: Store Registration Controller"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                }

                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreInformation");
                }
                var tempUser = model.store.Users.FirstOrDefault();
                if (tempUser == null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Temp User table in db is null"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                    #endregion
                }
               // MembershipCreateStatus createStatus;
                var Contact = model.store.ContactInfo.FirstOrDefault();
                if (Contact == null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find store contact during final submission. The Object model.store.ContactInfo.FirstOrDefault()"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                    #endregion
                }

                var GetRoleAdmin = db.Roles.Where(x => x.RoleName == "Store Admin").FirstOrDefault();
                if (GetRoleAdmin == null)
                {
                    //throw expection                   
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find role name store admin"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                    #endregion
                }

                var GetWorkFlowId = db.WorkflowSteps.Where(x => x.WorkflowId == model.store.WorkFlowId && x.Priority == 1).FirstOrDefault();

              
              
                string str = (from r in this.db.Roles where r.RoleName == GetWorkFlowId.RoleName select r.RoleName).FirstOrDefault<string>();
                string str1 = str;
                WorkflowSteps workflowStep1 = model.store.WorkflowSteps.FirstOrDefault<WorkflowSteps>();
                if (workflowStep1 != null)
                {
                    model.store.WorkflowSteps.Remove(workflowStep1);
                    this.db.SaveChanges();
                }
                var getStore = db.Store.Where(x => x.Id == model.store.Id).FirstOrDefault();
              
                model.store.OwnedBy = str1;
                model.store.Status = "Registration Verification";
              
                model.store.ModifiedBy = tempUser.UserName;
                model.store.ModifiedDate = DateTime.Now;
                model.store.SubmissionDate = new DateTime?(DateTime.Now);
                model.store.WorkflowSteps.Add(GetWorkFlowId);               
               // getStore.Users.Add(tempUser);
                this.db.SaveChanges();

                Alert alert = (from x in this.db.Alert where x.Id == 4 select x).FirstOrDefault<Alert>();
                Backbone.SendEmailNotificationToUser(db, alert.SubjectEmail, alert.Email.Replace("%Store_Name%", model.store.Name).Replace("%year%", DateTime.Now.Year.ToString()), model.store.ContactInfo.FirstOrDefault<ContactInfo>().EmailAddress, Settings.Default.FromEmail, alert.Id);
                Backbone.SendSMSNotificationToUser(db, alert.SubjectSms, alert.Sms.Replace("%Store_Name%", model.store.Name), model.store.ContactInfo.FirstOrDefault().MobileNo, alert.SubjectSms, alert.Id);
                //Send Message
               // services.SendEmail(db, model.store.ContactInfo.FirstOrDefault<ContactInfo>().EmailAddress);


                #region send message to approval admin
                var poolUser = new List<Users>();
                var rl = db.Roles.SingleOrDefault(x => x.RoleName == str1);

                var GetUser = rl.Users.ToList();
                var GetDirectorMsg = db.Alert.Where(x => x.Id == 5).FirstOrDefault();
                foreach (var u in GetUser)
                {
                    var GetUserInformation = db.UserDetail.Where(x => x.UserId == u.UserId).FirstOrDefault();
                    if (GetUserInformation != null)
                    {
                        if (GetUserInformation.MobileNumber != null)
                        {
                            services.SendSMSNotificationToAdmin(db, GetDirectorMsg.SubjectSms, GetDirectorMsg.Sms.Replace("%First_Name%", GetUserInformation.FirstName), GetUserInformation.MobileNumber, GetDirectorMsg.SubjectSms, GetDirectorMsg.Id);
                        }
                    }
                    services.SendEmailNotificationToAdmin(db, GetDirectorMsg.Title, GetDirectorMsg.Email.Replace("%First_Name%", u.UserName).Replace("%Store_Name%", model.store.Name).Replace("%Country%", model.store.Country.Name), GetUserInformation.EmailAddres, Settings.Default.FromEmail, GetDirectorMsg.Id);
                   // services.SendEmail(db, GetUserInformation.EmailAddres);
                }
                #endregion                
                return RedirectToAction("Feedback", "StoreRegistration", new { Id = model.store.ProcessInstaceId, area = "" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("StoreInformation");
            }
        }


        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Feedback(Guid Id)
        {
            try
            {
                if (Id == null)
                {

                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No Process Id sent to the feedback action: Store Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("StoreInformation");
                }
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();
                model.store = Backbone.GetStore(db, Id);
                if (model.store == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("StoreInformation");
                }
                var tempUser = model.store.TempUser.FirstOrDefault();
                if (tempUser != null)
                {
                    //throw expection
                    #region Delete User login from temp table    
                    TempUser del = new TempUser { };
                    db.TempUser.DeleteObject(tempUser);
                    db.SaveChanges();
                    #endregion
                }
                string rawFeedback = (from m in db.Workflow where m.Id == model.store.WorkFlowId select m.feedback).FirstOrDefault();
                TempData["Feedback"] = rawFeedback.Replace("%Store_Name%", model.store.Name);                
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("StoreInformation");
            }
        }


        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";


                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
