using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
   

    public class StoreRegistrationController : Controller
    {
        Backbone services = new Backbone();
        private PROEntities db = new PROEntities();
        private ProcessUtility util = new ProcessUtility();
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
                return RedirectToAction("Eror404", "Home", new { area = "" });
            }
        }       


        public ActionResult StoreInformation()
        {
            try
            {
                StoreRegistrationViewModel model = new StoreRegistrationViewModel();               
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Eror404", "Home", new { area = "" });
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
                                    TempData["messageType"] = "danger";
                                    TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                                    model.documentPath = Properties.Settings.Default.DocumentPath;
                                    return View(model);
                                }
                                else if (model.storeform.Logo.ContentLength > max_upload)
                                {
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

                            if(model.storeform.StoreCurrency== "1")
                            {
                                checkStore.CountryId = 1;
                                model.currency = "NGN";
                        }
                            else
                            {
                                checkStore.CountryId = 2;
                               model.currency = "GH";
                        }
                            checkStore.Name = model.storeform.Name;
                            checkStore.OwnProcurement = model.storeform.OwnProcurement;
                            checkStore.StoreCurrency = model.currency;
                            checkStore.URL = model.storeform.Name.Replace(" ", string.Empty);
                            checkStore.ModifiedBy =model.storeform.Name;
                            checkStore.ModifiedDate = DateTime.Now;                           
                           // db.SaveChanges();
                            TempData["message"] = "<b>" + model.storeform.Name + "</b> Information been successfully.";
                            return RedirectToAction("AddressList", "StoreRegistration", new { Id = checkStore.ProcessInstaceId, area = "" });
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
                                    TempData["messageType"] = "danger";
                                    TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPassport) + " are supported for logo";
                                    model.documentPath = Properties.Settings.Default.DocumentPath;
                                    return View(model);
                                }
                                else if (model.storeform.Logo.ContentLength > max_upload)
                                {
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


                            if (model.storeform.StoreCurrency == "1")
                            {
                                model.countryId = 1;
                                model.currency = "NGN";
                            }
                            else
                            {
                                model.countryId = 2;
                                model.currency = "GH";
                        }

                            Store addnew = new Store()
                            {
                                Name = model.storeform.Name,
                                Logo = model.logos,
                                CountryId = model.countryId,
                                StoreCurrency = model.storeform.StoreCurrency,
                                OwnProcurement = model.storeform.OwnProcurement,
                                ModifiedBy = model.storeform.Name,
                                ModifiedDate = DateTime.Now,
                                WorkFlowId= Properties.Settings.Default.StoreRegistrationWorkFlowId,
                                Status ="draft",
                                OwnedBy = "Administrator",
                                IsDeleted = false,
                                ProcessInstaceId = Guid.NewGuid(),
                                URL = model.storeform.Name.Replace(" ", string.Empty)
                            };
                            db.Store.AddObject(addnew);
                            db.SaveChanges();
                            TempData["message"] = "<b>" + model.storeform.Name + "</b> Information been successfully.";
                            return RedirectToAction("AddressList", "StoreRegistration", new { Id = addnew.ProcessInstaceId, area = "" });
                            #endregion 
                        }

                   }
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
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                //if(model.store.CountryId==1)
                //{
                //    model.StateList = (from s in this.db.State select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                //    model.LgaList = (from s in this.db.LGA select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                //}
                //else
                //{
                //    //implement ghana region here
                //}              
                // model.AddressTypeList = (from s in this.db.AddressType where !s.IsDeleted select new IntegerSelectListItem() { Text = s.Name, Value = s.Id } into x orderby x.Text select x).ToList<IntegerSelectListItem>();
                model.addressList = Backbone.GetStorAddress(db, Id);
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Eror404", "Home", new { area = "" });
            }
        }
    }
}
